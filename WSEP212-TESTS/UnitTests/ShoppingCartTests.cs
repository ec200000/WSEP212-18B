﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using WebApplication;
using WSEP212;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class 
    ShoppingCartTests
    {
        private static Store storeA;
        private static Store storeB;
        private static int itemAID;
        private static int itemBID;
        private static ShoppingCart shoppingCart;
        private static ItemPurchaseType purchaseType;

        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
            Startup.readConfigurationFile();
            SystemDBAccess.mock = true;
            
            SystemDBAccess.Instance.Bids.RemoveRange(SystemDBAccess.Instance.Bids);
            SystemDBAccess.Instance.Carts.RemoveRange(SystemDBAccess.Instance.Carts);
            SystemDBAccess.Instance.Invoices.RemoveRange(SystemDBAccess.Instance.Invoices);
            SystemDBAccess.Instance.Items.RemoveRange(SystemDBAccess.Instance.Items);
            SystemDBAccess.Instance.Permissions.RemoveRange(SystemDBAccess.Instance.Permissions);
            SystemDBAccess.Instance.Stores.RemoveRange(SystemDBAccess.Instance.Stores);
            SystemDBAccess.Instance.Users.RemoveRange(SystemDBAccess.Instance.Users);
            SystemDBAccess.Instance.DelayedNotifications.RemoveRange(SystemDBAccess.Instance.DelayedNotifications);
            SystemDBAccess.Instance.ItemReviewes.RemoveRange(SystemDBAccess.Instance.ItemReviewes);
            SystemDBAccess.Instance.UsersInfo.RemoveRange(SystemDBAccess.Instance.UsersInfo);
            SystemDBAccess.Instance.SaveChanges();

            ConcurrentLinkedList<PurchaseType> purchaseRoutes = new ConcurrentLinkedList<PurchaseType>();
            purchaseRoutes.TryAdd(PurchaseType.ImmediatePurchase);
            SalePolicyMock salesPolicy = new SalePolicyMock();
            PurchasePolicyMock purchasePolicy = new PurchasePolicyMock();
            
            UserRepository.Instance.initRepo();
            User user = new User("admin", 80);
            UserRepository.Instance.insertNewUser(user, "123456");

            ResultWithValue<int> addStoreARes = StoreRepository.Instance.addStore("SUPER PHARAM", "Tel-Aviv", salesPolicy, purchasePolicy, user);
            ResultWithValue<int> addStoreBRes = StoreRepository.Instance.addStore("SUPER PHARAM", "Haifa", salesPolicy, purchasePolicy, user);
            storeA = StoreRepository.Instance.getStore(addStoreARes.getValue()).getValue();
            storeB = StoreRepository.Instance.getStore(addStoreBRes.getValue()).getValue();

            itemAID = storeA.addItemToStorage(500, "black masks", "protects against infection of covid-19", 10, ItemCategory.Health).getValue();
            itemBID = storeB.addItemToStorage(500, "black masks", "protects against infection of covid-19", 10, ItemCategory.Health).getValue();
            purchaseType = new ItemImmediatePurchase(10);

            User buyer = new User("Sagiv", 21);
            UserRepository.Instance.insertNewUser(buyer, "123456");
            shoppingCart = new ShoppingCart(buyer.userName);
        }

        [TestCleanup]
        public void afterTests()
        {
            shoppingCart.clearShoppingCart();
        }

        [TestMethod]
        public void shoppingCartTest()
        {
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void isEmptyTest()
        {
            Assert.IsTrue(shoppingCart.isEmpty());
            shoppingCart.addItemToShoppingBag(storeA.storeID, itemAID, 5, purchaseType);
            Assert.IsFalse(shoppingCart.isEmpty());

            shoppingCart.clearShoppingCart();
        }

        [TestMethod]
        public void addItemToShoppingBagTest()
        {
            int storeAID = storeA.storeID;
            int storeBID = storeB.storeID;

            Assert.IsTrue(shoppingCart.addItemToShoppingBag(storeAID, itemAID, 10, purchaseType).getTag());
            Assert.IsTrue(shoppingCart.shoppingBags.ContainsKey(storeAID));
            Assert.IsTrue(shoppingCart.shoppingBags[storeAID].items.ContainsKey(itemAID));
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeAID].items[itemAID]);

            Assert.IsTrue(shoppingCart.addItemToShoppingBag(storeBID, itemBID, 20, purchaseType).getTag());
            Assert.AreEqual(2, shoppingCart.shoppingBags.Count);
            Assert.IsTrue(shoppingCart.shoppingBags.ContainsKey(storeBID));
            Assert.IsTrue(shoppingCart.shoppingBags[storeBID].items.ContainsKey(itemBID));
            Assert.AreEqual(20, shoppingCart.shoppingBags[storeBID].items[itemBID]);
        }

        [TestMethod]
        public void addItemToBagNoSuchStoreTest()
        {
            int storeAID = storeA.storeID;

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(-1, itemAID, 5, purchaseType).getTag());   // should fail because there is no such store ID
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void addItemToBagNoSuchItemTest()
        {
            int storeAID = storeA.storeID;

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(storeAID, itemBID, 5, purchaseType).getTag());   // should fail because there is no such item ID in the store
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void addItemToBagNotEnoughInStorageTest()
        {
            int storeAID = storeA.storeID;

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(storeAID, itemAID, 600, purchaseType).getTag());   // should fail because there is no enough of the item in storage
            Assert.IsTrue(shoppingCart.isEmpty());

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(storeAID, itemAID, -1, purchaseType).getTag());   // should fail because it is not possible to add a item with negative quantity
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void addItemToBagWithNegQuantityTest()
        {
            int storeAID = storeA.storeID;

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(storeAID, itemAID, -1, purchaseType).getTag());   // should fail because it is not possible to add a item with negative quantity
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void removeItemFromShoppingBagTest()
        {
            int storeAID = storeA.storeID;
            int storeBID = storeB.storeID;
            shoppingCart.addItemToShoppingBag(storeAID, itemAID, 10, purchaseType);
            shoppingCart.addItemToShoppingBag(storeBID, itemBID, 20, purchaseType);

            Assert.IsTrue(shoppingCart.removeItemFromShoppingBag(storeAID, itemAID).getTag());
            Assert.AreEqual(1, shoppingCart.shoppingBags.Count);
            Assert.IsFalse(shoppingCart.shoppingBags.ContainsKey(storeAID));
            Assert.IsFalse(shoppingCart.shoppingBags[storeBID].isEmpty());

            Assert.IsTrue(shoppingCart.removeItemFromShoppingBag(storeBID, itemBID).getTag());
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void removeItemFromBagNoSuchStoreTest()
        {
            int storeAID = storeA.storeID;
            int storeBID = storeB.storeID;
            shoppingCart.addItemToShoppingBag(storeAID, itemAID, 10, purchaseType);
            shoppingCart.addItemToShoppingBag(storeBID, itemBID, 20, purchaseType);

            Assert.IsFalse(shoppingCart.removeItemFromShoppingBag(-1, itemAID).getTag());   // should fail because there is no such store ID
            Assert.IsFalse(shoppingCart.shoppingBags[storeAID].isEmpty());
            Assert.IsFalse(shoppingCart.shoppingBags[storeBID].isEmpty());
        }

        [TestMethod]
        public void removeItemFromBagNoSuchItemTest()
        {
            int storeAID = storeA.storeID;
            int storeBID = storeB.storeID;
            shoppingCart.addItemToShoppingBag(storeAID, itemAID, 10, purchaseType);
            shoppingCart.addItemToShoppingBag(storeBID, itemBID, 20, purchaseType);

            Assert.IsFalse(shoppingCart.removeItemFromShoppingBag(storeAID, itemBID).getTag());   // should fail because there is no such item ID in the store
            Assert.IsFalse(shoppingCart.shoppingBags[storeAID].isEmpty());
            Assert.IsFalse(shoppingCart.shoppingBags[storeBID].isEmpty());
        }

        [TestMethod]
        public void changeItemQuantityInShoppingBagTest()
        {
            int storeID = storeA.storeID, itemID = itemAID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10, purchaseType);

            Assert.IsTrue(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, 5).getTag()); 
            Assert.AreEqual(5, shoppingCart.shoppingBags[storeID].items[itemID]);

            Assert.IsTrue(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, 0).getTag());
            Assert.IsFalse(shoppingCart.shoppingBags.ContainsKey(storeID));
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void changeItemQuantityInBagNoSuchStoreTest()
        {
            int storeID = storeA.storeID, itemID = itemAID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10, purchaseType);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(-1, itemID, 5).getTag());   // should fail because there is no such store ID
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);
        }

        [TestMethod]
        public void changeItemQuantityInBagNoSuchItemTest()
        {
            int storeID = storeA.storeID, itemID = itemAID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10, purchaseType);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(storeID, -1, 5).getTag());    // should fail because there is no such item ID in the store
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);
        }

        [TestMethod]
        public void changeItemQuantityInBagNotEnoughStorageTest()
        {
            int storeID = storeA.storeID, itemID = itemAID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10, purchaseType);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, 600).getTag());   // should fail because there is no enough of the item in storage
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, -1).getTag());   // should fail because -1 is not a valid quantity
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);
        }

        [TestMethod]
        public void changeItemNegQuantityInBagTest()
        {
            int storeID = storeA.storeID, itemID = itemAID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10, purchaseType);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, -1).getTag());   // should fail because -1 is not a valid quantity
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);
        }

        [TestMethod]
        public void clearShoppingCartTest()
        {
            int storeID = storeA.storeID, itemID = itemAID;
            RegularResult res = shoppingCart.addItemToShoppingBag(storeID, itemID, 10, purchaseType);
            Assert.IsFalse(shoppingCart.isEmpty());   // should not be empty - 1 shopping bag with 10 items

            shoppingCart.clearShoppingCart();
            Assert.IsTrue(shoppingCart.isEmpty());   // should be empty after clearing the shopping cart
        }
        
        [TestMethod]
        public void purchaseItemsInCartTest()
        {
            shoppingCart.addItemToShoppingBag(storeA.storeID, itemAID, 3, purchaseType);
            shoppingCart.addItemToShoppingBag(storeB.storeID, itemBID, 5, purchaseType);
            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
            StoreRepository.Instance.stores[storeA.storeID].deliverySystem = DeliverySystemMock.Instance;
            StoreRepository.Instance.stores[storeB.storeID].deliverySystem = DeliverySystemMock.Instance;

            ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> result = shoppingCart.purchaseItemsInCart();
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(10 * 3, result.getValue()[storeA.storeID].getPurchaseTotalPrice());
            Assert.AreEqual(10 * 5, result.getValue()[storeB.storeID].getPurchaseTotalPrice());
        }
    }
}