using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.ExternalPaymentSystem;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class HandlePurchasesTests
    {
        private static User user;
        private static int storeID1;
        private static int storeID2;
        private static int itemID1;
        private static int itemID2;

        private static DeliveryParameters deliveryParameters;
        private static PaymentParameters paymentParameters;

        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
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
            
            UserRepository.Instance.initRepo();
            user = new User("David");
            registerAndLogin();
            storeID1 = openStore("Store1", "Holon");
            itemID1 = addItemToStorage(storeID1, "shoko tara", 10, "taim!", 10.0, ItemCategory.Dairy);
            addItemToShoppingCart(storeID1, itemID1, 2);

            deliveryParameters = new DeliveryParameters(user.userName, "habanim", "Haifa", "Israel", "786598");
            paymentParameters = new PaymentParameters("68957221011", "1", "2021", user.userName, "086", "207885623");
        }
        
        [ClassCleanup]
        public static void testClean()
        {
            UserRepository.Instance.users.Clear();
            StoreRepository.Instance.stores.Clear();
        }

        public static void registerAndLogin()
        {
            String password = "1234";
            RegularResult insertUserRes = UserRepository.Instance.insertNewUser(user, password);
            Console.WriteLine(insertUserRes.getMessage());
            Assert.IsTrue(insertUserRes.getTag());
            user.changeState(new LoggedBuyerState(user));
        }

        public static void logout()
        {
            user.changeState(new GuestBuyerState(user));
        }
        
        public static int openStore(String name, String address)
        {
            ConcurrentLinkedList<PurchaseType> purchaseTypes = new ConcurrentLinkedList<PurchaseType>();
            purchaseTypes.TryAdd(PurchaseType.ImmediatePurchase);
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
            return ((ResultWithValue<int>)parameters.result).getValue();
        }
        
        public static int addItemToStorage(int storeID, String itemName, int quantity, String description, double price, ItemCategory category)
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
        
        public static void addItemToShoppingCart(int storeID, int itemID, int quantity)
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[4];
            list[0] = storeID;
            list[1] = itemID;
            list[2] = quantity;
            list[3] = new ItemImmediatePurchase(10.0);
            parameters.parameters = list;
            user.addItemToShoppingCart(parameters);
            Assert.IsTrue(((ResultWithValue<ConcurrentLinkedList<string>>) parameters.result).getTag());
        }

        public static void initWithAnotherStore()
        {
            storeID2 = openStore("Store2", "Holon");
            itemID2 = addItemToStorage(storeID2, "shoko tnova", 10, "taim!", 9.5, ItemCategory.Dairy);
            addItemToShoppingCart(storeID2, itemID2, 2);
        }
        
        [TestMethod]
        public void TestPurchaseWithFailingPaymentMock()
        {
            HandlePurchases.Instance.paymentSystem = BadPaymentSystemMock.Instance;
            StoreRepository.Instance.stores[storeID1].deliverySystem = DeliverySystemMock.Instance;
            ResultWithValue<ConcurrentLinkedList<string>> res = HandlePurchases.Instance.purchaseItems(user, deliveryParameters, paymentParameters);
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
            ResultWithValue<ConcurrentLinkedList<string>> res = HandlePurchases.Instance.purchaseItems(user, deliveryParameters, paymentParameters);
            Assert.IsFalse(res.getTag());
            Assert.AreEqual(user.purchases.Count, 0);
            Assert.AreEqual(user.shoppingCart.shoppingBags.Count, 1);
            Assert.AreEqual(StoreRepository.Instance.stores[storeID1].purchasesHistory.Count, 0);
            Assert.AreEqual(StoreRepository.Instance.stores[storeID1].storage[itemID1].quantity, 10);
        }

        [TestMethod]
        public void TestPurchaseWithFailingPurchasePolicyMock()
        {
            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
            StoreRepository.Instance.stores[storeID1].deliverySystem = DeliverySystemMock.Instance;
            StoreRepository.Instance.stores[storeID1].purchasePolicy = new BadPurchasePolicyMock();
            ResultWithValue<ConcurrentLinkedList<string>> res = HandlePurchases.Instance.purchaseItems(user, deliveryParameters, paymentParameters);
            Assert.IsFalse(res.getTag());
            Console.WriteLine(res.getMessage());
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
            StoreRepository.Instance.stores[storeID1].purchasePolicy = new PurchasePolicyMock();
            ResultWithValue<ConcurrentLinkedList<string>> res = HandlePurchases.Instance.purchaseItems(user, deliveryParameters, paymentParameters);
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