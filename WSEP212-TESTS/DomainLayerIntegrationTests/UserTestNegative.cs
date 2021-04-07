using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS
{
    [TestClass]
    public class UserTestNegative
    {
        private User user1;
        private User user2;
        [TestInitialize]
        public void testInit()
        {
            user1 = new User("a"); //guest
            user2 = new User("b"); //logged
            user2.changeState(new LoggedBuyerState(user2));
            UserRepository.Instance.users.TryAdd(user1, false);
            UserRepository.Instance.usersInfo.TryAdd("a", Authentication.Instance.encryptPassword("123"));
            UserRepository.Instance.users.TryAdd(user2, true);
            UserRepository.Instance.usersInfo.TryAdd("b", Authentication.Instance.encryptPassword("123456"));
            
            Store store2 = new Store("t", new SalesPolicy("default", null), new PurchasePolicy("default", null,null), user2);
            Item item = new Item(3, "shoko", "taim retzah!", 12, "milk products");
            store2.storage.TryAdd(1, item);
            StoreRepository.Instance.stores.TryAdd(1,store2);
        }
        
        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            UserRepository.Instance.usersInfo.Clear();
            StoreRepository.Instance.stores.Clear();
        }

        [TestMethod]
        public void registerTest()
        {
            User user = new User("c");
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = user.userName;
            list[1] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            Assert.IsTrue((bool)parameters.result);
            Assert.AreEqual(UserRepository.Instance.users.Count, 3);
            
            User user2 = new User(user.userName); //the user can't register again - already in the system
            ThreadParameters parameters2 = new ThreadParameters();
            object[] list2 = new object[2];
            list2[0] = user.userName;
            list2[1] = "5678";
            parameters2.parameters = list2;
            user.register(parameters2);
            Assert.IsFalse((bool)parameters2.result);
            Assert.AreEqual(UserRepository.Instance.users.Count, 3);
        }

        [TestMethod]
        public void loginTest()
        {
            ThreadParameters parameters3 = new ThreadParameters(); //the user is already logged in
            object[] list3 = new object[2];
            list3[0] = user2.userName;
            list3[1] = "123456";
            parameters3.parameters = list3;
            user2.login(parameters3);
            Assert.IsTrue(parameters3.result is NotImplementedException);
        }

        [TestMethod]
        public void logoutTest()
        {
            ThreadParameters parameters3 = new ThreadParameters();
            object[] list3 = new object[1];
            list3[0] = user2.userName;
            parameters3.parameters = list3;
            user2.logout(parameters3);
            Assert.IsTrue((bool)parameters3.result);

            ThreadParameters parameters4 = new ThreadParameters(); //the user is already logged out
            object[] list4 = new object[1];
            list4[0] = user2.userName;
            parameters4.parameters = list4;
            user2.logout(parameters4);
            Assert.IsTrue(parameters4.result is NotImplementedException);
        }

        [TestMethod]
        public void openStoreTest()
        {
            User u = new User("k"); //the user is not registered to the system
            String name = "store";
            SalesPolicy salesPolicy = new SalesPolicy("default", null);
            PurchasePolicy purchasePolicy = new PurchasePolicy("default", null, null);
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = name;
            list[1] = purchasePolicy;
            list[2] = salesPolicy;
            parameters.parameters = list;
            u.openStore(parameters);
            Assert.IsTrue(parameters.result is NotImplementedException);
            Assert.AreEqual(1, StoreRepository.Instance.stores.Count);
        }

        [TestMethod]
        public void TestAddItemToStorageUserNotRegistered()
        {
            User u = new User("k"); //the user is not registered to the system
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[6];
            list[0] = 1;
            list[1] = 3;
            list[2] = "shoko";
            list[3] = "taim retzah!";
            list[4] = (double)12;
            list[5] = "milk products";
            parameters.parameters = list;
            u.addItemToStorage(parameters);
            Assert.IsTrue(parameters.result is NotImplementedException);
            Assert.AreEqual(1, StoreRepository.Instance.stores[1].storage.Count);
        }
        
        [TestMethod]
        public void TestAddItemToStorageUserWithoutStore()
        {
            user1.changeState(new LoggedBuyerState(user1));
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[6];
            list[0] = 7;//no such store
            list[1] = 3;
            list[2] = "bamba";
            list[3] = "taim retzah!";
            list[4] = (double)12;
            list[5] = "milk products";
            parameters.parameters = list;
            user1.addItemToStorage(parameters);
            Assert.IsFalse((bool)parameters.result);
            Assert.AreEqual(1, StoreRepository.Instance.stores.Count);
            Assert.AreEqual(1, StoreRepository.Instance.stores[1].storage.Count);
        }
        
        [TestMethod]
        public void TestAddExistingItemToStorage()
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[6];
            list[0] = 1;
            list[1] = 3;
            list[2] = "shoko";
            list[3] = "taim retzah!";
            list[4] = (double)12;
            list[5] = "milk products";
            parameters.parameters = list;
            user2.addItemToStorage(parameters);
            Assert.IsFalse((bool)parameters.result);
            Assert.AreEqual(1, StoreRepository.Instance.stores[1].storage.Count);
        }
        
        [TestMethod]
        public void itemReviewTestUserNotRegistered()
        {
            User u = new User("k"); //the user is not registered to the system
            String review = "best shoko ever!!";
            int storeID = 1;
            int itemID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = review;
            list[1] = itemID;
            list[2] = storeID;
            parameters.parameters = list;
            u.itemReview(parameters);
            Assert.IsTrue(parameters.result is NotImplementedException);
        }
        
        [TestMethod]
        public void itemReviewTestUserWithoutStore()
        {
            user1.changeState(new LoggedBuyerState(user1));
            String review = "best shoko ever!!";
            int storeID = 7; //no such store
            int itemID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = review;
            list[1] = itemID;
            list[2] = storeID;
            parameters.parameters = list;
            user1.itemReview(parameters);
            Assert.IsFalse((bool)parameters.result);
        }
        
        [TestMethod]
        public void itemReviewTestNoSuchItem()
        {
            String review = "best shoko ever!!";
            int storeID = 1; 
            int itemID = 99; //no such item
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = review;
            list[1] = itemID;
            list[2] = storeID;
            parameters.parameters = list;
            user2.itemReview(parameters);
            Assert.IsFalse((bool)parameters.result);
        }
        
        [TestMethod]
        public void removeItemFromStorageTestUserNotRegistered()
        {
            User u = new User("k"); //the user is not registered to the system
            int storeID = 1;
            int itemID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = storeID;
            list[1] = itemID;
            parameters.parameters = list;
            u.removeItemFromStorage(parameters);
            Assert.IsTrue(parameters.result is NotImplementedException);
            Assert.AreEqual(1, StoreRepository.Instance.stores[1].storage.Count);
        }
        
        [TestMethod]
        public void removeItemFromStorageTestUserWithoutStore()
        {
            user1.changeState(new LoggedBuyerState(user1));
            int storeID = 6; //no such store
            int itemID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = storeID;
            list[1] = itemID;
            parameters.parameters = list;
            user1.removeItemFromStorage(parameters);
            Assert.IsFalse((bool)parameters.result);
            Assert.AreEqual(1, StoreRepository.Instance.stores[1].storage.Count);
                    
        }
        
        [TestMethod]
        public void removeItemFromStorageTestNoSuchItem()
        {
            int storeID = 1;
            int itemID = 99;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = storeID;
            list[1] = itemID;
            parameters.parameters = list;
            user2.removeItemFromStorage(parameters);
            Assert.IsFalse((bool)parameters.result);
            Assert.AreEqual(1, StoreRepository.Instance.stores[1].storage.Count);
        }
        
        [TestMethod]
        public void editItemDetailsTestUserNotRegistered()
        {
            User u = new User("k"); //the user is not registered to the system
            Item item = new Item(2,"shoko","milk",8.90,"milk");
            item.itemName = "shoko moka";
            int storeID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[7];
            list[0] = storeID;
            list[1] = item.itemID;
            list[2] = item.quantity;
            list[3] = item.itemName;
            list[4] = item.description;
            list[5] = item.price;
            list[6] = item.category;
            parameters.parameters = list;
            u.editItemDetails(parameters);
            Assert.IsTrue(parameters.result is NotImplementedException);
        }
        
        [TestMethod]
        public void editItemDetailsTestUserWithoutStore()
        {
            user1.changeState(new LoggedBuyerState(user1));
            int storeID = 6; //no such store
            Item item = new Item(2,"shoko","milk",8.90,"milk");
            item.itemName = "shoko moka";
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[7];
            list[0] = storeID;
            list[1] = item.itemID;
            list[2] = item.quantity;
            list[3] = item.itemName;
            list[4] = item.description;
            list[5] = item.price;
            list[6] = item.category;
            parameters.parameters = list;
            user1.editItemDetails(parameters);
            Assert.IsFalse((bool)parameters.result);
        }

        [TestMethod]
        public void editItemDetailsTestUserNoSuchItem()
        {
            int storeID = 1;
            Item item = new Item(2, "shoko", "milk", 8.90, "milk");
            item.itemName = "shoko moka";
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[7];
            list[0] = storeID;
            list[1] = 250;
            list[2] = item.quantity;
            list[3] = item.itemName;
            list[4] = item.description;
            list[5] = item.price;
            list[6] = item.category;
            parameters.parameters = list;
            user2.editItemDetails(parameters);
            Assert.IsFalse((bool) parameters.result);
        }

        [TestMethod]
        public void TestAppointStoreManager()
        {
            int storeID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = "new user";
            list[1] = storeID;
            parameters.parameters = list;
            user1.appointStoreManager(parameters);
            Assert.IsTrue(parameters.result is NotImplementedException);
        }
        
        [TestMethod]
        public void TestAppointStoreManager2()
        {
            int storeID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = "new user";
            list[1] = storeID;
            parameters.parameters = list;
            user2.appointStoreManager(parameters);
            Assert.IsFalse((bool)parameters.result);
        }
        
        [TestMethod]
        public void TestAppointStoreOwner()
        {
            int storeID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = "new user";
            list[1] = storeID;
            parameters.parameters = list;
            user1.appointStoreOwner(parameters);
            Assert.IsTrue(parameters.result is NotImplementedException);
        }
        
        [TestMethod]
        public void TestAppointStoreOwner2()
        {
            int storeID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = "new user"; //non existing user
            list[1] = storeID;
            parameters.parameters = list;
            user2.appointStoreOwner(parameters);
            Assert.IsFalse((bool)parameters.result);
        }

        [TestMethod]
        public void TestEditManagerPermissions()
        {
            
            int storeID = 1;
            ConcurrentLinkedList<Permissions> permissions = new ConcurrentLinkedList<Permissions>();
            permissions.TryAdd(Permissions.GetOfficialsInformation);
            permissions.TryAdd(Permissions.GetStorePurchaseHistory);
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = "new user";
            list[1] = permissions;
            list[2] = storeID;
            parameters.parameters = list;
            user1.editManagerPermissions(parameters);
            Assert.IsTrue(parameters.result is NotImplementedException);
        }
        
        [TestMethod]
        public void TestEditManagerPermissions2()
        {
            
            int storeID = 1;
            ConcurrentLinkedList<Permissions> permissions = new ConcurrentLinkedList<Permissions>();
            permissions.TryAdd(Permissions.GetOfficialsInformation);
            permissions.TryAdd(Permissions.GetStorePurchaseHistory);
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = "new user"; //non existing manager
            list[1] = permissions;
            list[2] = storeID;
            parameters.parameters = list;
            user2.editManagerPermissions(parameters);
            Assert.IsFalse((bool)parameters.result);
        }
        
        [TestMethod]
        public void TestRemoveStoreManager()
        {
            int storeID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = "new user";
            list[1] = storeID;
            parameters.parameters = list;
            user1.removeStoreManager(parameters);
            Assert.IsTrue(parameters.result is NotImplementedException);
        }
        
        [TestMethod]
        public void TestRemoveStoreManager2()
        {
            int storeID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = "new user"; // non existing manager
            list[1] = storeID;
            parameters.parameters = list;
            user2.removeStoreManager(parameters);
            Assert.IsFalse((bool)parameters.result);
        }
        
        [TestMethod]
        public void TestGetOfficialsInformation()
        {
            int storeID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[1];
            list[0] = storeID;
            parameters.parameters = list;
            user1.getOfficialsInformation(parameters);
            Assert.IsTrue(parameters.result is NotImplementedException);        
        }
        
        [TestMethod]
        public void TestGetOfficialsInformation2()
        {
            int storeID = 2; //non existing store
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[1];
            list[0] = storeID;
            parameters.parameters = list;
            user2.getOfficialsInformation(parameters);
            Assert.IsNull(parameters.result);     
        }

        [TestMethod]
        public void purchaseItemsTestUserWithEmptyCart()
        {
            string address = "moshe levi 3 beer sheva";
            ThreadParameters parameters2 = new ThreadParameters();
            object[] list2 = new object[1];
            list2[0] = address;
            parameters2.parameters = list2;
            user2.purchaseItems(parameters2);
            Assert.IsFalse((bool)parameters2.result);
            Assert.AreEqual(0, this.user2.purchases.Count);
            Assert.AreEqual(0, StoreRepository.Instance.stores[1].purchasesHistory.Count);
        }
        
        [TestMethod]
        public void purchaseItemsTestNoAddressWasGiven()
        {
            ThreadParameters parameters2 = new ThreadParameters();
            object[] list2 = new object[1];
            list2[0] = null;
            parameters2.parameters = list2;
            user2.purchaseItems(parameters2);
            Assert.IsFalse((bool)parameters2.result);
            Assert.AreEqual(0, this.user2.purchases.Count);
            Assert.AreEqual(0, StoreRepository.Instance.stores[1].purchasesHistory.Count);
        }
    }
}