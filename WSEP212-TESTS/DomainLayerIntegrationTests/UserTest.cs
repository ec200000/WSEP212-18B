using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS
{
    [TestClass]
    public class UserTest
    {
        User user;
        [TestInitialize]
        public void testInit()
        {
            this.user = new User("check name");
        }

        public bool registerAndLogin()
        {
            String password = "1234";
            if (UserRepository.Instance.insertNewUser(this.user, password))
            {
                user.changeState(new LoggedBuyerState(user));
                return true;
            }
            return false;
        }

        public bool logout()
        {
            this.user.changeState(new GuestBuyerState(this.user));
            return true;
        }
        
        public bool openStore()
        {
            String name = "store";
            SalesPolicy salesPolicy = new SalesPolicy("default", null);
            PurchasePolicy purchasePolicy = new PurchasePolicy("default", null, null);
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = name;
            list[1] = purchasePolicy;
            list[2] = salesPolicy;
            parameters.parameters = list;
            user.openStore(parameters);
            return (bool)parameters.result;
        }
        
        public bool addItemToStorage()
        {
            int storeID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[6];
            list[0] = storeID;
            list[1] = 3;
            list[2] = "shoko";
            list[3] = "taim retzah!";
            list[4] = 12.0;
            list[5] = "milk products";
            parameters.parameters = list;
            user.addItemToStorage(parameters);
            return (bool)parameters.result;
        }
        
        public bool addNewUser()
        {
            User user2 = new User("new user");
            String password = "1234";
            return UserRepository.Instance.insertNewUser(this.user, password);
        }
        
        public bool addNewManager()
        {
            int storeID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = "new user";
            list[1] = storeID;
            parameters.parameters = list;
            user.appointStoreManager(parameters);
            return (bool) parameters.result;
        }

        public void switchToSystemManager()
        {
            user.changeState(new SystemManagerState(this.user));
        }

        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            UserRepository.Instance.usersInfo.Clear();
            foreach (Store store in StoreRepository.Instance.stores.Values)
            {
                store.storage.Clear();
            }
            StoreRepository.Instance.stores.Clear();
        }

        [TestMethod]
        public void TestConstructor()
        {
            Assert.AreEqual("check name", user.userName);
            Assert.IsTrue(typeof(GuestBuyerState).IsInstanceOfType(user.state));
        }

        [TestMethod]
        public void TestChangeState()
        {
            user.changeState(new LoggedBuyerState(user));
            Assert.IsTrue(typeof(LoggedBuyerState).IsInstanceOfType(user.state));
        }

        [TestMethod]
        public void TestRegister()
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = user.userName;
            list[1] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            Assert.IsTrue((bool)parameters.result);
            Assert.AreEqual(UserRepository.Instance.users.Count, 1);
        }

        [TestMethod]
        //[ExpectedException (typeof(NotImplementedException))]
        public void TestLogin()
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = user.userName;
            list[1] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            if ((bool)parameters.result)
            {
                ThreadParameters parameters2 = new ThreadParameters();
                object[] list2 = new object[2];
                list2[0] = user.userName;
                list2[1] = "1234";
                parameters2.parameters = list2;
                user.login(parameters2);
                Assert.IsTrue((bool)parameters2.result);
                Assert.IsTrue(user.state is LoggedBuyerState);
            }
        }

        [TestMethod]
        public void TestLoginWrongPassword() // wrong password
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = user.userName;
            list[1] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            if ((bool)parameters.result)
            {
                ThreadParameters parameters2 = new ThreadParameters();
                object[] list2 = new object[2];
                list2[0] = user.userName;
                list2[1] = "3456";
                parameters2.parameters = list2;
                user.login(parameters2);
                Assert.IsFalse((bool)parameters2.result);
                Assert.IsTrue(user.state is GuestBuyerState);

            }
        }

        [TestMethod]
        public void TestLogout()
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = user.userName;
            list[1] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            if ((bool)parameters.result)
            {
                ThreadParameters parameters2 = new ThreadParameters();
                object[] list2 = new object[2];
                list2[0] = user.userName;
                list2[1] = "1234";
                parameters2.parameters = list2;
                user.login(parameters2);
                if ((bool)parameters2.result)
                {
                    ThreadParameters parameters3 = new ThreadParameters();
                    object[] list3 = new object[1];
                    list3[0] = user.userName;
                    parameters3.parameters = list3;
                    user.logout(parameters3);
                    Assert.IsTrue((bool)parameters3.result);
                }
            }
        }

        [TestMethod]
        public void TestOpenStore()
        {
            if (registerAndLogin())
            {
                String name = "store";
                SalesPolicy salesPolicy = new SalesPolicy("default", null);
                PurchasePolicy purchasePolicy = new PurchasePolicy("default", null, null);
                ThreadParameters parameters = new ThreadParameters();
                object[] list = new object[3];
                list[0] = name;
                list[1] = purchasePolicy;
                list[2] = salesPolicy;
                parameters.parameters = list;
                user.openStore(parameters);
                Assert.IsTrue((bool)parameters.result);
                Assert.AreEqual(1, StoreRepository.Instance.stores.Count);
            }
        }
        
        [TestMethod]
        public void TestAddItemToStorage()
        {
            if (registerAndLogin())
            {
                if (openStore())
                {
                    int storeID = 1;
                    ThreadParameters parameters = new ThreadParameters();
                    object[] list = new object[6];
                    list[0] = storeID;
                    list[1] = 3;
                    list[2] = "shoko";
                    list[3] = "taim retzah!";
                    list[4] = 12.0;
                    list[5] = "milk products";
                    parameters.parameters = list;
                    user.addItemToStorage(parameters);
                    Assert.IsTrue((bool)parameters.result);
                    Assert.AreEqual(1, StoreRepository.Instance.stores[1].storage.Count);
                }
            }
        }
        
        [TestMethod]
        public void TestItemReview()
        {
            if (registerAndLogin())
            {
                if (openStore())
                {
                    if (addItemToStorage())
                    {
                        String review = "best shoko ever!!";
                        int storeID = 1;
                        int itemID = 1;
                        ThreadParameters parameters = new ThreadParameters();
                        object[] list = new object[3];
                        list[0] = review;
                        list[1] = itemID;
                        list[2] = storeID;
                        parameters.parameters = list;
                        user.itemReview(parameters);
                        Assert.IsTrue((bool)parameters.result);
                        Assert.AreEqual(1, StoreRepository.Instance.stores[1].storage[1].reviews.Count);
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestRemoveItemFromStorage()
        {
            if (registerAndLogin())
            {
                if (openStore())
                {
                    if (addItemToStorage())
                    {
                        int storeID = 1;
                        int itemID = 1;
                        ThreadParameters parameters = new ThreadParameters();
                        object[] list = new object[2];
                        list[0] = storeID;
                        list[1] = itemID;
                        parameters.parameters = list;
                        user.removeItemFromStorage(parameters);
                        Assert.IsTrue((bool)parameters.result);
                        Assert.AreEqual(0, StoreRepository.Instance.stores[1].storage.Count);
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestEditItemDetails()
        {
            if (registerAndLogin())
            {
                if (openStore())
                {
                    if (addItemToStorage())
                    {
                        Item item = StoreRepository.Instance.stores[1].getItemById(1);
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
                        user.editItemDetails(parameters);
                        Assert.IsTrue((bool)parameters.result);
                        Assert.AreEqual("shoko moka", StoreRepository.Instance.stores[1].storage[1].itemName);
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestAddItemToShoppingCart()
        {
            if (registerAndLogin())
            {
                Store store = new Store("store", new SalesPolicy("default", null), new PurchasePolicy("default", null, null), this.user);
                StoreRepository.Instance.addStore(store);
                int itemID = store.addItemToStorage(3, "shoko", "taim retzah!", 12, "milk products");
                int storeID = store.storeID;
                int quantity = 2;
                ThreadParameters parameters = new ThreadParameters();
                object[] list = new object[3];
                list[0] = storeID;
                list[1] = itemID;
                list[2] = quantity;
                parameters.parameters = list;
                user.addItemToShoppingCart(parameters);
                Assert.IsTrue((bool)parameters.result);
                Assert.AreEqual(1, user.shoppingCart.shoppingBags[storeID].items.Count);
            }
        }

        [TestMethod]
        public void TestRemoveItemFromShoppingCart()
        {
            if (registerAndLogin())
            {
                Store store = new Store("store", new SalesPolicy("default", null), new PurchasePolicy("default", null, null), this.user);
                StoreRepository.Instance.addStore(store);
                int itemID = store.addItemToStorage(3, "shoko", "taim retzah!", 12, "milk products");
                int storeID = store.storeID;
                int quantity = 2;
                ThreadParameters parameters = new ThreadParameters();
                object[] list = new object[3];
                list[0] = storeID;
                list[1] = itemID;
                list[2] = quantity;
                parameters.parameters = list;
                user.addItemToShoppingCart(parameters);
                if ((bool) parameters.result)
                {
                    ThreadParameters parameters2 = new ThreadParameters();
                    object[] list2 = new object[2];
                    list2[0] = storeID;
                    list2[1] = itemID;
                    parameters2.parameters = list2;
                    user.removeItemFromShoppingCart(parameters2);
                    Assert.IsTrue((bool)parameters2.result);
                    Assert.AreEqual(0, user.shoppingCart.shoppingBags.Count);
                }
            }
        }
        
        [TestMethod]
        public void TestAppointStoreManager()
        {
            if (registerAndLogin())
            {
                if (openStore())
                {
                    if (addNewUser())
                    {
                        int storeID = 1;
                        ThreadParameters parameters = new ThreadParameters();
                        object[] list = new object[2];
                        list[0] = "new user";
                        list[1] = storeID;
                        parameters.parameters = list;
                        user.appointStoreManager(parameters);
                        Assert.IsTrue((bool)parameters.result);
                        Assert.AreEqual(2, StoreRepository.Instance.stores[1].storeSellersPermissions.Count);
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestAppointStoreOwner()
        {
            if (registerAndLogin())
            {
                if (openStore())
                {
                    if (addNewUser())
                    {
                        int storeID = 1;
                        ThreadParameters parameters = new ThreadParameters();
                        object[] list = new object[2];
                        list[0] = "new user";
                        list[1] = storeID;
                        parameters.parameters = list;
                        user.appointStoreOwner(parameters);
                        Assert.IsTrue((bool)parameters.result);
                        Assert.AreEqual(2, StoreRepository.Instance.stores[1].storeSellersPermissions.Count);                    }
                }
            }
        }
        
        [TestMethod]
        public void TestEditManagerPermissions()
        {
            if (registerAndLogin())
            {
                if (openStore())
                {
                    if (addNewUser())
                    {
                        if (addNewManager())
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
                            user.editManagerPermissions(parameters);
                            Assert.IsTrue((bool)parameters.result);
                            Assert.AreEqual(permissions, StoreRepository.Instance.stores[1].storeSellersPermissions["new user"].permissionsInStore);
                        }
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestRemoveStoreManager()
        {
            if (registerAndLogin())
            {
                if (openStore())
                {
                    if (addNewUser())
                    {
                        if (addNewManager())
                        {
                            int storeID = 1;
                            ThreadParameters parameters = new ThreadParameters();
                            object[] list = new object[2];
                            list[0] = "new user";
                            list[1] = storeID;
                            parameters.parameters = list;
                            user.removeStoreManager(parameters);
                            Assert.IsTrue((bool) parameters.result);
                            Assert.AreEqual(1, StoreRepository.Instance.stores[1].storeSellersPermissions.Count);
                        }
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestGetOfficialsInformation()
        {
            if (registerAndLogin())
            {
                if (openStore())
                {
                    if (addNewUser())
                    {
                        if (addNewManager())
                        {
                            int storeID = 1;
                            ThreadParameters parameters = new ThreadParameters();
                            object[] list = new object[1];
                            list[0] = storeID;
                            parameters.parameters = list;
                            user.getOfficialsInformation(parameters);
                            Assert.AreEqual(2, ((ConcurrentDictionary<User, ConcurrentLinkedList<Permissions>>)parameters.result).Count);
                        }
                    }
                }
            }
        }
    }
}
