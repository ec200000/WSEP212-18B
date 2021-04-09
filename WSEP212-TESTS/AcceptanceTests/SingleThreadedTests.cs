﻿using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.Result;
using WSEP212.ServiceLayer;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class SingleThreadedTests
    {
        private User user1;
        private User user2;
        private User systemManager;
        private Store store;
        private Item item;
        
        [TestInitialize]
        public void testInit()
        {
            systemManager = new User("big manager"); //system manager
            systemManager.changeState(new SystemManagerState(systemManager));
            
            user1 = new User("a"); //guest
            user2 = new User("b"); //logged
            user2.changeState(new LoggedBuyerState(user2));
            UserRepository.Instance.users.TryAdd(user1, false);
            UserRepository.Instance.usersInfo.TryAdd("a", Authentication.Instance.encryptPassword("123"));
            UserRepository.Instance.users.TryAdd(user2, true);
            UserRepository.Instance.usersInfo.TryAdd("b", Authentication.Instance.encryptPassword("123456"));
            
            ConcurrentLinkedList<PurchaseType> purchaseRoutes = new ConcurrentLinkedList<PurchaseType>();
            purchaseRoutes.TryAdd(PurchaseType.ImmediatePurchase);
            SalesPolicy salesPolicy = new SalesPolicy("DEFAULT", new ConcurrentLinkedList<PolicyRule>());
            PurchasePolicy purchasePolicy = new PurchasePolicy("DEFAULT", purchaseRoutes, new ConcurrentLinkedList<PolicyRule>());
            store = new Store("t","bb",salesPolicy,purchasePolicy,user2);
            
            item = new Item(30, "shoko", "taim retzah!", 12, "milk products");
            ConcurrentDictionary<int, PurchaseType> itemsPurchaseType = new ConcurrentDictionary<int, PurchaseType>();
            itemsPurchaseType.TryAdd(item.itemID, PurchaseType.ImmediatePurchase);
            store.storage.TryAdd(item.itemID, item);
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(item.itemID, 1);
            store.applyPurchasePolicy(user2, items, itemsPurchaseType);
            StoreRepository.Instance.stores.TryAdd(store.storeID, store);
        }
        
        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            UserRepository.Instance.usersInfo.Clear();
            StoreRepository.Instance.stores.Clear();
            user1.purchases.Clear();
            user2.purchases.Clear();
            user1.shoppingCart.shoppingBags.Clear();
            user2.shoppingCart.shoppingBags.Clear();
        }

        [TestMethod]
        public void registerTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.register("abcd", "1234");
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(UserRepository.Instance.users.Count, 3);
            
            RegularResult result2 = controller.register("a", "123");
            Assert.IsFalse(result2.getTag());
            Assert.AreEqual(UserRepository.Instance.users.Count, 3);
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException), "user that is logged in, can perform this action again")]
        public void loginTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.login("a", "123");
            Assert.IsTrue(result.getTag());
            UserRepository.Instance.users.TryGetValue(user1, out var res); //is saved as logged in
            Assert.IsTrue(res);
            Assert.AreEqual(UserRepository.Instance.users.Count, 2);
            
            RegularResult result2 = controller.login("b", "123456"); //already logged
            //should  throw exception
            Assert.IsFalse(true); //if it gets here - no exception was thrown
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void logoutTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.logout("b");
            Assert.IsTrue(result.getTag());
            UserRepository.Instance.users.TryGetValue(user2, out var res); //is saved as logged out
            Assert.IsFalse(res);
            Assert.AreEqual(UserRepository.Instance.users.Count, 2);
            
            RegularResult result2 = controller.logout("a"); //not logged in
            //should  throw exception
            Assert.IsFalse(true); //if it gets here - no exception was thrown
        }
        
        [TestMethod]
        public void addItemToShoppingCartTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.addItemToShoppingCart("b",store.storeID, item.itemID, 2); //logged user
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 1);
            user2.shoppingCart.shoppingBags.Clear();
            
            result = controller.addItemToShoppingCart("a",store.storeID, item.itemID, 8); //guest user
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user1.shoppingCart.shoppingBags.Count, 1);
            user1.shoppingCart.shoppingBags.Clear();
            
            result = controller.addItemToShoppingCart("b",store.storeID, item.itemID, 100); //over quantity
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 0);
            
            result = controller.addItemToShoppingCart("b",store.storeID, -1, 1); //item does not exists
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 0);
            
            result = controller.addItemToShoppingCart("b",-1, item.itemID, 1); //store doest not exists
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 0);
        }
        
        [TestMethod]
        public void removeItemFromShoppingCartTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.addItemToShoppingCart("b",store.storeID, item.itemID, 2);//adding an item
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 1);
            
            result = controller.removeItemFromShoppingCart("b",-1, item.itemID); //wrong store id
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 1);
            
            result = controller.removeItemFromShoppingCart("b",store.storeID, -1); //wrong item id
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 1);
            
            result = controller.removeItemFromShoppingCart("b",store.storeID, item.itemID); //removing it
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 0);
            
            Console.WriteLine($"before: {user1.shoppingCart.shoppingBags.Count}");
            result = controller.removeItemFromShoppingCart("a",store.storeID, item.itemID); //nothing in the cart
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user1.shoppingCart.shoppingBags.Count, 0);
        }
        
        [TestMethod]
        public void purchaseItemsTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.addItemToShoppingCart("b",store.storeID, item.itemID, 2);//adding an item
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 1);
            
            result = controller.purchaseItems("a","beer sheva"); //nothing in the cart
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user1.purchases.Count, 0);
            
            result = controller.purchaseItems("b",null); //wrong item id
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.purchases.Count, 0);
            
            result = controller.purchaseItems("b","ashdod"); 
            Console.WriteLine(result.getMessage());
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user2.purchases.Count, 1);
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void openStoreTest()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> result = controller.openStore("b",store.storeName,store.storeAddress,"DEFAULT","DEFAULT"); 
            Assert.IsFalse(result.getTag()); //store already exists
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 1);
            
            result = controller.openStore(null,store.storeName,store.storeAddress,"DEFAULT","DEFAULT"); 
            Assert.IsFalse(result.getTag()); //null user name
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 1);
            
            controller.openStore("b",null,store.storeAddress,"DEFAULT","DEFAULT"); 
            Assert.IsFalse(result.getTag()); //null store name
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 1);
            
            controller.openStore("b",store.storeName,null,"DEFAULT","DEFAULT"); 
            Assert.IsFalse(result.getTag()); //null address
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 1);
            
            controller.openStore("b",store.storeName,store.storeAddress,null,"DEFAULT"); 
            Assert.IsFalse(result.getTag()); //null purchase policy
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 1);
            
            controller.openStore("b",store.storeName,store.storeAddress,"DEFAULT",null); 
            Assert.IsFalse(result.getTag()); //null sales policy
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 1);
            
            controller.openStore("b","HAMAMA","Ashdod","DEFAULT","DEFAULT"); 
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 2);
            
            ResultWithValue<int> res = controller.openStore("a","gg", "kk", "default", "default"); //guest cant open
            Assert.IsFalse(true); //should throw exception - if its here, nothing was thrown
        }

        [TestMethod]
        public void itemReviewTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.itemReview("b","wow",item.itemID,store.storeID); //logged
            RegularResult res = controller.addItemToShoppingCart("b", store.storeID, item.itemID, 2);
            Assert.IsTrue(res.getTag());
            res = controller.purchaseItems("b", "ashdod");
            Assert.IsTrue(res.getTag());
            Assert.IsTrue(result.getTag()); 
            Assert.AreEqual(item.reviews.Count, 1);
            
            result = controller.itemReview("a","boo",item.itemID,store.storeID); //guest
            res = controller.addItemToShoppingCart("a", store.storeID, item.itemID, 2);
            Assert.IsTrue(res.getTag());
            res = controller.purchaseItems("a", "ashdod");
            Assert.IsTrue(res.getTag());
            Assert.IsTrue(result.getTag()); 
            Assert.AreEqual(item.reviews.Count, 2);
            
            result = controller.itemReview(null,"boo",item.itemID,store.storeID); 
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(item.reviews.Count, 2);
            
            result = controller.itemReview("b",null,item.itemID,store.storeID);
            Assert.IsFalse(result.getTag()); 
            Assert.AreEqual(item.reviews.Count, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void addItemToStorageTest()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> storeId = controller.openStore("b", "HAMAMA", "Beer Sheva", "deault", "default");
            Assert.IsTrue(storeId.getTag());
            Store store = StoreRepository.Instance.stores[storeId.getValue()];
            ItemDTO itemDto = new ItemDTO(store.storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, string>(), 1.34, "snacks");
            
            ResultWithValue<int> res = controller.addItemToStorage("b", store.storeID, itemDto);
            Assert.IsTrue(res.getTag());
            Assert.AreNotEqual(-1,res.getValue());
            Assert.AreEqual(1, store.storage.Count);
            
            res = controller.addItemToStorage("b", store.storeID, itemDto); //already in storage
            Assert.IsTrue(res.getTag());
            Assert.AreNotEqual(-1,res.getValue());
            Assert.AreEqual(1, store.storage.Count);
            
            res = controller.addItemToStorage("b", store.storeID, null);
            Assert.IsFalse(res.getTag());
            Assert.AreEqual(-1,res.getValue());
            Assert.AreEqual(1, store.storage.Count);
            
            controller.addItemToStorage("a", store.storeID, itemDto);//guest user - can't perform this action
            
        }
    }
}