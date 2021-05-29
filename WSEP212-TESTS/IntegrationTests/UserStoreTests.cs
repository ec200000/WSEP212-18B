using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using WSEP212;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TESTS.IntegrationTests
{
    [TestClass]
    public class UserStoreTests
    {
        User user;
        private User user3;

        [ClassInitialize]
        public void init()
        {
            SystemDBAccess.mock = true;
        }
        
        [TestInitialize]
        public void testInit()
        {
            this.user = new User("check name");
            user3 = new User("b"); //logged
            user3.changeState(new LoggedBuyerState(user3));
            UserRepository.Instance.users.TryAdd(user3, true);
        }

        public bool registerAndLogin()
        {
            String password = "1234";
            RegularResult insertUserRes = UserRepository.Instance.insertNewUser(this.user, password);
            if (insertUserRes.getTag())
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
        
        public int openStore()
        {
            //Store.resetStoreCounter();
            String name = "store";
            String address = "holon";
            SalePolicyMock salesPolicy = new SalePolicyMock();
            PurchasePolicyMock purchasePolicy = new PurchasePolicyMock();
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[4];
            list[0] = name;
            list[1] = address;
            list[2] = purchasePolicy;
            list[3] = salesPolicy;
            parameters.parameters = list;
            user.openStore(parameters);
            
            return ((ResultWithValue<int>)parameters.result).getValue();
        }
        
        public int openStore2()
        {
            //Store.resetStoreCounter();
            String name = "store";
            String address = "holon";
            SalePolicyMock salesPolicy = new SalePolicyMock();
            PurchasePolicyMock purchasePolicy = new PurchasePolicyMock();
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[4];
            list[0] = name;
            list[1] = address;
            list[2] = purchasePolicy;
            list[3] = salesPolicy;
            parameters.parameters = list;
            user3.openStore(parameters);
            
            return ((ResultWithValue<int>)parameters.result).getValue();
        }

        public int addItemToStorage()
        {
            int storeID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[6];
            list[0] = storeID;
            list[1] = 3;
            list[2] = "shoko";
            list[3] = "taim retzah!";
            list[4] = 12.0;
            list[5] = ItemCategory.Dairy;
            parameters.parameters = list;
            user.addItemToStorage(parameters);
            return ((ResultWithValue<int>)parameters.result).getValue();
        }
        
        public int addItemToStorage2()
        {
            int storeID = 1;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[6];
            list[0] = storeID;
            list[1] = 3;
            list[2] = "shoko";
            list[3] = "taim retzah!";
            list[4] = 12.0;
            list[5] = ItemCategory.Dairy;
            parameters.parameters = list;
            user3.addItemToStorage(parameters);
            return ((ResultWithValue<int>)parameters.result).getValue();
        }
        
        public bool addNewUser()
        {
            User user2 = new User("new user");
            String password = "1234";
            return UserRepository.Instance.insertNewUser(this.user, password).getTag();
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

        public bool addItemToShoppingCart()
        {
            int storeID = 1;
            int itemID = 1;
            int quantity = 2;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[4];
            list[0] = storeID;
            list[1] = itemID;
            list[2] = quantity;
            list[3] = new ItemImmediatePurchase(12.0);
            parameters.parameters = list;
            user.addItemToShoppingCart(parameters);
            return (bool) parameters.result;
        }

        public bool purchaseItems()
        {
            string address = "moshe levi 3 beer sheva";
            ThreadParameters parameters2 = new ThreadParameters();
            object[] list2 = new object[1];
            list2[0] = address;
            parameters2.parameters = list2;
            user.purchaseItems(parameters2);
            return (bool)parameters2.result;
        }

        public void switchToSystemManager()
        {
            user.changeState(new SystemManagerState(this.user));
        }

        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
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
            object[] list = new object[3];
            list[0] = user.userName;
            list[1] = 18;
            list[2] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            Assert.IsTrue(((RegularResult)parameters.result).getTag());
            Assert.AreEqual(UserRepository.Instance.users.Count, 2);
        }

        [TestMethod]

        public void TestLogin()
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = user.userName;
            list[1] = 18;
            list[2] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            if (((RegularResult)parameters.result).getTag())
            {
                ThreadParameters parameters2 = new ThreadParameters();
                object[] list2 = new object[2];
                list2[0] = user.userName;
                list2[1] = "1234";
                parameters2.parameters = list2;
                user.login(parameters2);
                Assert.IsTrue(((RegularResult)parameters2.result).getTag());
                Assert.IsTrue(user.state is LoggedBuyerState);
            }
        }

        [TestMethod]
        public void TestLoginWrongPassword() // wrong password
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = user.userName;
            list[1] = 18;
            list[2] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            if (((RegularResult)parameters.result).getTag())
            {
                ThreadParameters parameters2 = new ThreadParameters();
                object[] list2 = new object[2];
                list2[0] = user.userName;
                list2[1] = "3456";
                parameters2.parameters = list2;
                user.login(parameters2);
                Assert.IsFalse(((RegularResult)parameters2.result).getTag());
                Assert.IsTrue(user.state is GuestBuyerState);

            }
        }

        [TestMethod]
        public void TestLogout()
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = user.userName;
            list[1] = 18;
            list[2] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            if (((RegularResult)parameters.result).getTag())
            {
                ThreadParameters parameters2 = new ThreadParameters();
                object[] list2 = new object[2];
                list2[0] = user.userName;
                list2[1] = "1234";
                parameters2.parameters = list2;
                user.login(parameters2);
                if (((RegularResult)parameters2.result).getTag())
                {
                    ThreadParameters parameters3 = new ThreadParameters();
                    object[] list3 = new object[1];
                    list3[0] = user.userName;
                    parameters3.parameters = list3;
                    user.logout(parameters3);
                    Assert.IsTrue(((RegularResult)parameters3.result).getTag());
                }
            }
        }

        [TestMethod]
        public void TestOpenStore()
        {
            if (registerAndLogin())
            {
                String name = "store";
                String address = "holon";
                SalePolicyMock salesPolicy = new SalePolicyMock();
                PurchasePolicyMock purchasePolicy = new PurchasePolicyMock();
                ThreadParameters parameters = new ThreadParameters();
                object[] list = new object[4];
                list[0] = name;
                list[1] = address;
                list[2] = purchasePolicy;
                list[3] = salesPolicy;
                parameters.parameters = list;
                user.openStore(parameters);
                Assert.IsTrue(((ResultWithValue<int>)parameters.result).getTag());
                Assert.AreEqual(1, StoreRepository.Instance.stores.Count);
            }
        }
        
        [TestMethod]
        public void TestAddItemToStorage()
        {
            if (registerAndLogin())
            {
                int storeID = openStore();
                if (storeID > 0)
                {
                    ThreadParameters parameters = new ThreadParameters();
                    object[] list = new object[6];
                    list[0] = storeID;
                    list[1] = 3;
                    list[2] = "bamba";
                    list[3] = "taim retzah!";
                    list[4] = 1.23;
                    list[5] = ItemCategory.Snacks;
                    parameters.parameters = list;
                    user.addItemToStorage(parameters);
                    Assert.AreEqual(1, StoreRepository.Instance.stores[storeID].storage.Count);
                    Assert.IsTrue(((ResultWithValue<int>)parameters.result).getTag());
                }
            }
        }
        
        [TestMethod]
        public void TestItemReview()
        {
            if (registerAndLogin())
            {
                int storeID = openStore();
                if (storeID > 0) 
                {
                    int itemID = addItemToStorage();
                    if (itemID > 0)
                    {
                        if (addItemToShoppingCart())
                        {
                            if (purchaseItems())
                            {
                                String review = "best shoko ever!!";
                                ThreadParameters parameters = new ThreadParameters();
                                object[] list = new object[3];
                                list[0] = review;
                                list[1] = itemID;
                                list[2] = storeID;
                                parameters.parameters = list;
                                user.itemReview(parameters);
                                Assert.IsTrue(((RegularResult)parameters.result).getTag());
                                Assert.AreEqual(1, StoreRepository.Instance.stores[1].storage[1].reviews.Count);
                            }
                        }
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestItemReviewNoPurchase()
        {
            if (registerAndLogin())
            {
                int storeID = openStore();
                if (storeID > 0) 
                {
                    int itemID = addItemToStorage();
                    if (itemID > 0)
                    {
                        String review = "best shoko ever!!";
                        ThreadParameters parameters = new ThreadParameters();
                        object[] list = new object[3];
                        list[0] = review;
                        list[1] = itemID;
                        list[2] = storeID;
                        parameters.parameters = list;
                        user.itemReview(parameters);
                        Assert.IsFalse(((RegularResult)parameters.result).getTag());
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
                int storeID = openStore();
                if (storeID > 0)
                {
                    int itemID = addItemToStorage();
                    if (itemID > 0)
                    {
                        ThreadParameters parameters = new ThreadParameters();
                        object[] list = new object[2];
                        list[0] = storeID;
                        list[1] = itemID;
                        parameters.parameters = list;
                        user.removeItemFromStorage(parameters);
                        Assert.IsTrue(((RegularResult)parameters.result).getTag());
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
                int storeID = openStore();
                if (storeID > 0)
                {
                    int itemID = addItemToStorage();
                    if (itemID > 0)
                    {
                        Item item = StoreRepository.Instance.stores[1].getItemById(itemID).getValue();
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
                        user.editItemDetails(parameters);
                        Assert.IsTrue(((RegularResult)parameters.result).getTag());
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
                int storeID = StoreRepository.Instance.addStore("store", "Bat Yam", new SalePolicyMock(), new PurchasePolicyMock(), this.user).getValue();
                Store store = StoreRepository.Instance.getStore(storeID).getValue();
                int itemID = store.addItemToStorage(3, "shoko", "taim retzah!", 12, ItemCategory.Dairy).getValue();
                int quantity = 2;
                ThreadParameters parameters = new ThreadParameters();
                object[] list = new object[4];
                list[0] = storeID;
                list[1] = itemID;
                list[2] = quantity;
                list[3] = new ItemImmediatePurchase(12.0);
                parameters.parameters = list;
                user.addItemToShoppingCart(parameters);
                RegularResult res = (RegularResult)(parameters.result);
                Assert.IsTrue(res.getTag());
                Assert.AreEqual(1, user.shoppingCart.shoppingBags[storeID].items.Count);
            }
        }

        [TestMethod]
        public void TestRemoveItemFromShoppingCart()
        {
            if (registerAndLogin())
            {
                int storeID = StoreRepository.Instance.addStore("store", "Bat Yam", new SalePolicyMock(), new PurchasePolicyMock(), this.user).getValue();
                Store store = StoreRepository.Instance.getStore(storeID).getValue();
                int itemID = store.addItemToStorage(3, "shoko", "taim retzah!", 12, ItemCategory.Dairy).getValue();
                int quantity = 2;
                ThreadParameters parameters = new ThreadParameters();
                object[] list = new object[4];
                list[0] = storeID;
                list[1] = itemID;
                list[2] = quantity;
                list[3] = new ItemImmediatePurchase(12.0);
                parameters.parameters = list;
                user.addItemToShoppingCart(parameters);
                if (((RegularResult)parameters.result).getTag())
                {
                    ThreadParameters parameters2 = new ThreadParameters();
                    object[] list2 = new object[2];
                    list2[0] = storeID;
                    list2[1] = itemID;
                    parameters2.parameters = list2;
                    user.removeItemFromShoppingCart(parameters2);
                    Assert.IsTrue(((RegularResult)parameters2.result).getTag());
                    Assert.AreEqual(0, user.shoppingCart.shoppingBags.Count);
                }
            }
        }
        
        [TestMethod]
        public void TestAppointStoreManager()
        {
            if (registerAndLogin())
            {
                int storeID = openStore();
                if (storeID > 0)
                {
                    if (addNewUser())
                    {
                        ThreadParameters parameters = new ThreadParameters();
                        object[] list = new object[2];
                        list[0] = "new user";
                        list[1] = storeID;
                        parameters.parameters = list;
                        user.appointStoreManager(parameters);
                        Assert.IsTrue(((RegularResult)parameters.result).getTag());
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
                int storeID = openStore();
                if (storeID > 0)
                {
                    if (addNewUser())
                    {
                        ThreadParameters parameters = new ThreadParameters();
                        object[] list = new object[2];
                        list[0] = "new user";
                        list[1] = storeID;
                        parameters.parameters = list;
                        user.appointStoreOwner(parameters);
                        Assert.IsTrue(((RegularResult)parameters.result).getTag());
                        Assert.AreEqual(2, StoreRepository.Instance.stores[1].storeSellersPermissions.Count);                    }
                }
            }
        }
        
        [TestMethod]
        public void TestEditManagerPermissions()
        {
            if (registerAndLogin())
            {
                int storeID = openStore();
                if (storeID > 0)
                {
                    if (addNewUser())
                    {
                        if (addNewManager())
                        {
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
                            Assert.IsTrue(((RegularResult)parameters.result).getTag());
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
                int storeID = openStore();
                if (storeID > 0)
                {
                    if (addNewUser())
                    {
                        if (addNewManager())
                        {
                            ThreadParameters parameters = new ThreadParameters();
                            object[] list = new object[2];
                            list[0] = "new user";
                            list[1] = storeID;
                            parameters.parameters = list;
                            user.removeStoreManager(parameters);
                            Assert.IsTrue(((RegularResult)parameters.result).getTag());
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
                int storeID = openStore();
                if (storeID > 0)
                {
                    if (addNewUser())
                    {
                        if (addNewManager())
                        {
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
        
        [TestMethod]
        public void TestPurchaseItemsLoggedUser()
        {
            if (registerAndLogin())
            {
                int storeID = openStore();
                if (storeID > 0)
                {
                    int itemID = addItemToStorage();
                    if (itemID > 0)
                    {
                        int quantity = 2;
                        ThreadParameters parameters = new ThreadParameters();
                        object[] list = new object[3];
                        list[0] = storeID;
                        list[1] = itemID;
                        list[2] = quantity;
                        parameters.parameters = list;
                        user.addItemToShoppingCart(parameters);
                        if (((RegularResult)parameters.result).getTag())
                        {
                            string address = "moshe levi 3 beer sheva";
                            ThreadParameters parameters2 = new ThreadParameters();
                            object[] list2 = new object[1];
                            list2[0] = address;
                            parameters2.parameters = list2;
                            user.purchaseItems(parameters2);
                            Assert.IsTrue(((RegularResult)parameters2.result).getTag());
                            Assert.AreEqual(1, this.user.purchases.Count);
                            Assert.AreEqual(1, StoreRepository.Instance.stores[storeID].purchasesHistory.Count);
                        }
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestPurchaseItemsGuestUser()
        {
            int storeID = openStore2();
            if (storeID > 0)
            {
                int itemID = addItemToStorage2();
                if (itemID > 0)
                {
                    int quantity = 2;
                    ThreadParameters parameters = new ThreadParameters();
                    object[] list = new object[3];
                    list[0] = storeID;
                    list[1] = itemID;
                    list[2] = quantity;
                    parameters.parameters = list;
                    user.addItemToShoppingCart(parameters);
                    if (((RegularResult)parameters.result).getTag())
                    {
                        string address = "moshe levi 3 beer sheva";
                        ThreadParameters parameters2 = new ThreadParameters();
                        object[] list2 = new object[1];
                        list2[0] = address;
                        parameters2.parameters = list2;
                        user.purchaseItems(parameters2);
                        Assert.IsTrue(((RegularResult)parameters2.result).getTag());
                        Assert.AreEqual(1, this.user.purchases.Count);
                        Assert.AreEqual(1, StoreRepository.Instance.stores[storeID].purchasesHistory.Count);
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestGetStorePurchaseHistory()
        {
            if (registerAndLogin())
            {
                int storeID = openStore();
                if (storeID > 0)
                {
                    int itemID = addItemToStorage();
                    if (itemID > 0)
                    {
                        int quantity = 2;
                        ThreadParameters parameters = new ThreadParameters();
                        object[] list = new object[3];
                        list[0] = storeID;
                        list[1] = itemID;
                        list[2] = quantity;
                        parameters.parameters = list;
                        user.addItemToShoppingCart(parameters);
                        if (((RegularResult)parameters.result).getTag())
                        {
                            string address = "moshe levi 3 beer sheva";
                            ThreadParameters parameters2 = new ThreadParameters();
                            object[] list2 = new object[1];
                            list2[0] = address;
                            parameters2.parameters = list2;
                            user.purchaseItems(parameters2);
                            Assert.IsTrue(((RegularResult)parameters2.result).getTag());
                            Assert.AreEqual(1, this.user.purchases.Count);
                            Assert.AreEqual(1, StoreRepository.Instance.stores[storeID].purchasesHistory.Count);
                        }
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestGetStoresPurchaseHistory()
        {
            if (registerAndLogin())
            {
                int storeID = openStore();
                if (storeID > 0)
                {
                    int itemID = addItemToStorage();
                    if (itemID > 0)
                    {
                        if (purchaseItems())
                        {
                            switchToSystemManager();
                            ThreadParameters parameters2 = new ThreadParameters();
                            object[] list2 = new object[0];
                            parameters2.parameters = list2;
                            user.getStoresPurchaseHistory(parameters2);
                            Assert.AreEqual(1, ((ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>>)parameters2.result).Count);
                        }
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestGetUsersPurchaseHistory()
        {
            if (registerAndLogin())
            {
                int storeID = openStore();
                if (storeID > 0)
                {
                    int itemID = addItemToStorage();
                    if (itemID > 0)
                    {
                        if (purchaseItems())
                        {
                            switchToSystemManager();
                            ThreadParameters parameters2 = new ThreadParameters();
                            object[] list2 = new object[0];
                            parameters2.parameters = list2;
                            user.getUsersPurchaseHistory(parameters2);
                            Assert.AreEqual(1, ((ConcurrentDictionary<String, ConcurrentBag<PurchaseInvoice>>)parameters2.result).Count);
                        }
                    }
                }
            }
        }
    }
}
