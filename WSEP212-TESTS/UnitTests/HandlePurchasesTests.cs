using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TESTS
{
    [TestClass]
    public class HandlePurchasesTests
    {
        User user;
        private User user3;
        [TestInitialize]
        public void testInit()
        {
            this.user = new User("check name");
            user3 = new User("b"); //logged
            user3.changeState(new LoggedBuyerState(user3));
            UserRepository.Instance.users.TryAdd(user3, true);
        }
        
        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            StoreRepository.Instance.stores.Clear();
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
            SalesPolicy salesPolicy = new SalesPolicy("default", new ConcurrentLinkedList<PolicyRule>());
            PurchasePolicy purchasePolicy = new PurchasePolicy("default", new ConcurrentLinkedList<PurchaseType>(), new ConcurrentLinkedList<PolicyRule>());
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
            list[5] = "milk products";
            parameters.parameters = list;
            user.addItemToStorage(parameters);
            return ((ResultWithValue<int>)parameters.result).getValue();
        }
        
        public bool addItemToShoppingCart()
        {
            int storeID = 1;
            int itemID = 1;
            int quantity = 2;
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = storeID;
            list[1] = itemID;
            list[2] = quantity;
            parameters.parameters = list;
            user.addItemToShoppingCart(parameters);
            return ((RegularResult) parameters.result).getTag();
        }
        
        [TestMethod]
        public void TestPurchaseItemsPaymentMock()
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
                            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
                            RegularResult res = HandlePurchases.Instance.purchaseItems(this.user, "habanim");
                            Assert.IsFalse(res.getTag());
                            Assert.AreEqual(user.purchases.Count, 0);
                            Assert.AreEqual(user.shoppingCart.shoppingBags.Count, 1);
                            Assert.AreEqual(StoreRepository.Instance.stores[storeID].purchasesHistory.Count, 0);
                        }
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestPurchaseItemDeliveryMock()
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
                            StoreRepository.Instance.stores[storeID].deliverySystem = DeliverySystemMock.Instance;
                            RegularResult res = HandlePurchases.Instance.purchaseItems(this.user, "habanim");
                            Assert.IsFalse(res.getTag());
                            Assert.AreEqual(user.purchases.Count, 0);
                            Assert.AreEqual(user.shoppingCart.shoppingBags.Count, 1);
                            Assert.AreEqual(StoreRepository.Instance.stores[storeID].purchasesHistory.Count, 0);
                        }
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestPurchaseItem()
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
                            RegularResult res = HandlePurchases.Instance.purchaseItems(this.user, "habanim");
                            Assert.IsTrue(res.getTag());
                            Assert.AreEqual(user.purchases.Count, 1);
                            Assert.AreEqual(user.shoppingCart.shoppingBags.Count, 0);
                            Assert.AreEqual(StoreRepository.Instance.stores[storeID].purchasesHistory.Count, 1);
                        }
                    }
                }
            }
        }
    }
}