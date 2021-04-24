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
        
        public int openStore(String name, String address)
        {
            ConcurrentLinkedList<PurchaseType> purchaseTypes = new ConcurrentLinkedList<PurchaseType>();
            purchaseTypes.TryAdd(PurchaseType.ImmediatePurchase);
            SalePolicy salesPolicy = new SalePolicy("default");
            PurchasePolicy purchasePolicy = new PurchasePolicy("default", new ConcurrentLinkedList<PolicyPredicate>());
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
        
        public int addItemToStorage(int storeID, String itemName, int quantity, String description, double price, String category)
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[6];
            list[0] = storeID;
            list[1] = quantity;
            list[2] = itemName;
            list[3] = description;
            list[4] = price;
            list[5] = category;
            parameters.parameters = list;
            user.addItemToStorage(parameters);
            return ((ResultWithValue<int>)parameters.result).getValue();
        }
        
        public bool addItemToShoppingCart(int storeID, int itemID, int quantity)
        {
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
        public void TestPurchaseWithFailingPaymentMock()
        {
            if (registerAndLogin())
            {
                int storeID = openStore("Store1", "Holon");
                if (storeID > 0)
                {
                    int itemID = addItemToStorage(storeID, "shoko", 10, "taim!", 10.0, "milk items");
                    if (itemID > 0)
                    {
                        if (addItemToShoppingCart(storeID, itemID, 2))
                        {
                            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
                            RegularResult res = HandlePurchases.Instance.purchaseItems(this.user, "habanim");
                            Assert.IsFalse(res.getTag());
                            Assert.AreEqual(user.purchases.Count, 0);
                            Assert.AreEqual(user.shoppingCart.shoppingBags.Count, 1);
                            Assert.AreEqual(StoreRepository.Instance.stores[storeID].purchasesHistory.Count, 0);
                            Assert.AreEqual(StoreRepository.Instance.stores[storeID].storage[itemID].quantity, 10);
                        }
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestPurchaseWithFailingDeliveryMock()
        {
            if (registerAndLogin())
            {
                int storeID = openStore("Store1", "Holon");
                if (storeID > 0)
                {
                    int itemID = addItemToStorage(storeID, "shoko", 10, "taim!", 10.0, "milk items");
                    if (itemID > 0)
                    {
                        if (addItemToShoppingCart(storeID, itemID, 2))
                        {
                            HandlePurchases.Instance.paymentSystem = PaymentSystem.Instance;
                            StoreRepository.Instance.stores[storeID].deliverySystem = DeliverySystemMock.Instance;
                            RegularResult res = HandlePurchases.Instance.purchaseItems(this.user, "habanim");
                            Assert.IsFalse(res.getTag());
                            Assert.AreEqual(user.purchases.Count, 0);
                            Assert.AreEqual(user.shoppingCart.shoppingBags.Count, 1);
                            Assert.AreEqual(StoreRepository.Instance.stores[storeID].purchasesHistory.Count, 0);
                            Assert.AreEqual(StoreRepository.Instance.stores[storeID].storage[itemID].quantity, 10);
                        }
                    }
                }
            }
        }
        
        [TestMethod]
        public void TestSuccessfulPurchase()
        {
            if (registerAndLogin())
            {
                int storeID1 = openStore("Store3", "Holon");
                int storeID2 = openStore("Store4", "Ashdod");
                if (storeID1 > 0 && storeID2 > 0)
                {
                    int itemID1 = addItemToStorage(storeID1, "shoko tnova", 10, "taim!", 10.0, "milk items");
                    int itemID2 = addItemToStorage(storeID2, "shoko tara", 10, "taim!", 9.5, "milk items");
                    if (itemID1 > 0 && itemID2 > 0)
                    {
                        bool addItem1 = addItemToShoppingCart(storeID1, itemID1, 2);
                        bool addItem2 = addItemToShoppingCart(storeID2, itemID2, 2);
                        if (addItem1 && addItem2)
                        {
                            HandlePurchases.Instance.paymentSystem = PaymentSystem.Instance;
                            RegularResult res = HandlePurchases.Instance.purchaseItems(this.user, "habanim");
                            Assert.IsTrue(res.getTag());
                            Assert.AreEqual(user.purchases.Count, 2);
                            Assert.AreEqual(user.shoppingCart.shoppingBags.Count, 0);
                            Assert.AreEqual(StoreRepository.Instance.stores[storeID1].purchasesHistory.Count, 1);
                            Assert.AreEqual(StoreRepository.Instance.stores[storeID2].purchasesHistory.Count, 1);
                            Assert.AreEqual(StoreRepository.Instance.stores[storeID1].storage[itemID1].quantity, 8);
                            Assert.AreEqual(StoreRepository.Instance.stores[storeID2].storage[itemID2].quantity, 8);
                        }
                    }
                }
            }
        }

    }
}