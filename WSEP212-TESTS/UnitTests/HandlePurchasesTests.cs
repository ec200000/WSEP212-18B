using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.ServiceLayer.Result;
using WSEP212_TEST.UnitTests.UnitTestMocks;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class HandlePurchasesTests
    {
        private User user;
        private User user3;
        private int storeID1;
        private int storeID2;
        private int itemID1;
        private int itemID2;

        [ClassInitialize]
        public void testInit()
        {
            this.user = new User("check name");
            user3 = new User("b"); //logged
            user3.changeState(new LoggedBuyerState(user3));
            UserRepository.Instance.users.TryAdd(user3, true);

            registerAndLogin();
            this.storeID1 = openStore("Store1", "Holon");
            this.itemID1 = addItemToStorage(storeID1, "shoko tara", 10, "taim!", 10.0, "milk items");
            addItemToShoppingCart(storeID1, itemID1, 2);
        }
        
        [ClassCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            StoreRepository.Instance.stores.Clear();
        }

        public void registerAndLogin()
        {
            String password = "1234";
            RegularResult insertUserRes = UserRepository.Instance.insertNewUser(this.user, password);
            Assert.IsTrue(insertUserRes.getTag());
            user.changeState(new LoggedBuyerState(user));
        }

        public void logout()
        {
            this.user.changeState(new GuestBuyerState(this.user));
        }
        
        public int openStore(String name, String address)
        {
            ConcurrentLinkedList<PurchaseType> purchaseTypes = new ConcurrentLinkedList<PurchaseType>();
            purchaseTypes.TryAdd(PurchaseType.ImmediatePurchase);
            SalePolicy salesPolicy = new SalePolicy("default");
            PurchasePolicy purchasePolicy = new PurchasePolicy("default");
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[4];
            list[0] = name;
            list[1] = address;
            list[2] = purchasePolicy;
            list[3] = salesPolicy;
            parameters.parameters = list;
            user.openStore(parameters);
            Assert.IsTrue(((ResultWithValue<int>)parameters.result).getTag());
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
            Assert.IsTrue(((ResultWithValue<int>)parameters.result).getTag());
            return ((ResultWithValue<int>)parameters.result).getValue();
        }
        
        public void addItemToShoppingCart(int storeID, int itemID, int quantity)
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = storeID;
            list[1] = itemID;
            list[2] = quantity;
            parameters.parameters = list;
            user.addItemToShoppingCart(parameters);
            Assert.IsTrue(((RegularResult) parameters.result).getTag());
        }

        public void initWithAnotherStore()
        {
            this.storeID2 = openStore("Store2", "Holon");
            this.itemID2 = addItemToStorage(storeID2, "shoko tnova", 10, "taim!", 9.5, "milk items");
            addItemToShoppingCart(storeID2, itemID2, 2);
        }
        
        [TestMethod]
        public void TestPurchaseWithFailingPaymentMock()
        {
            HandlePurchases.Instance.paymentSystem = BadPaymentSystemMock.Instance;
            StoreRepository.Instance.stores[storeID1].deliverySystem = DeliverySystemMock.Instance;
            ResultWithValue<ConcurrentLinkedList<string>> res = HandlePurchases.Instance.purchaseItems(this.user, "habanim");
            Assert.IsFalse(res.getTag());
            Assert.AreEqual(user.purchases.Count, 0);
            Assert.AreEqual(user.shoppingCart.shoppingBags.Count, 1);
            Assert.AreEqual(StoreRepository.Instance.stores[storeID1].purchasesHistory.Count, 0);
            Assert.AreEqual(StoreRepository.Instance.stores[storeID1].storage[itemID1].quantity, 10);
        }
        
        [TestMethod]
        public void TestPurchaseWithFailingDeliveryMock()
        {
            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
            StoreRepository.Instance.stores[storeID1].deliverySystem = BadDeliverySystemMock.Instance;
            ResultWithValue<ConcurrentLinkedList<string>> res = HandlePurchases.Instance.purchaseItems(this.user, "habanim");
            Assert.IsFalse(res.getTag());
            Assert.AreEqual(user.purchases.Count, 0);
            Assert.AreEqual(user.shoppingCart.shoppingBags.Count, 1);
            Assert.AreEqual(StoreRepository.Instance.stores[storeID1].purchasesHistory.Count, 0);
            Assert.AreEqual(StoreRepository.Instance.stores[storeID1].storage[itemID1].quantity, 10);
        }
        
        [TestMethod]
        public void TestSuccessfulPurchase()
        {
            initWithAnotherStore();

            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
            StoreRepository.Instance.stores[storeID1].deliverySystem = DeliverySystemMock.Instance;
            StoreRepository.Instance.stores[storeID2].deliverySystem = DeliverySystemMock.Instance;
            ResultWithValue<ConcurrentLinkedList<string>> res = HandlePurchases.Instance.purchaseItems(this.user, "habanim");
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