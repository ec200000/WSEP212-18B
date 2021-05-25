using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.ExternalPaymentSystem;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.ServiceLayer.Result;
using WSEP212_TEST.UnitTests.UnitTestMocks;

namespace WSEP212_TESTS.IntegrationTests
{
    [TestClass]
    public class SubmitOfferPurchaseTests
    {
        private static Store store;
        private static User user;
        private static int itemIDA;
        private static int itemIDB;

        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
            user = new User("Sagiv", 21);
            ResultWithValue<int> addStoreRes = StoreRepository.Instance.addStore("Delta", "Ashdod", new SalePolicy("DEFUALT"), new PurchasePolicy("DEFUALT"), new User("admin"));
            store = StoreRepository.Instance.getStore(addStoreRes.getValue()).getValue();
            store.supportPurchaseType(PurchaseType.ImmediatePurchase);
            store.supportPurchaseType(PurchaseType.SubmitOfferPurchase);
            itemIDA = store.addItemToStorage(500, "black masks", "protects against infection of covid-19", 10, ItemCategory.Health).getValue();
            itemIDB = store.addItemToStorage(500, "white masks", "protects against infection of covid-19", 10, ItemCategory.Health).getValue();

            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
            store.deliverySystem = DeliverySystemMock.Instance;
        }

        [ClassCleanup]
        public static void cleanUp()
        {
            StoreRepository.Instance.removeStore(store.storeID);
        }

        private void cleaningCart()
        {
            user.shoppingCart.clearShoppingCart();
        }

        private void addItemToShoppingCart(int storeID, int itemID, int quantity, ItemPurchaseType purchaseType)
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[4];
            list[0] = storeID;
            list[1] = itemID;
            list[2] = quantity;
            list[3] = purchaseType;
            parameters.parameters = list;
            user.addItemToShoppingCart(parameters);
        }

        private void submitOffer(int storeID, int itemID, double offer)
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = storeID;
            list[1] = itemID;
            list[2] = offer;
            parameters.parameters = list;
            user.submitPriceOffer(parameters);
        }

        private void priceOfferDecision(String userName, int storeID, int itemID, PriceStatus status)
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[4];
            list[0] = userName;
            list[1] = storeID;
            list[2] = itemID;
            list[3] = status;
            parameters.parameters = list;
            user.confirmPriceStatus(parameters);
        }

        public ResultWithValue<ConcurrentLinkedList<String>> purchaseItems()
        {
            DeliveryParameters deliveryParameters = new DeliveryParameters(user.userName, "habanim", "Haifa", "Israel", "786598");
            PaymentParameters paymentParameters = new PaymentParameters("68957221011", "1", "2021", user.userName, "086", "207885623");
            ThreadParameters parameters2 = new ThreadParameters();
            object[] list2 = new object[2];
            list2[0] = deliveryParameters;
            list2[1] = paymentParameters;
            parameters2.parameters = list2;
            user.purchaseItems(parameters2);
            return ((ResultWithValue<ConcurrentLinkedList<String>>)parameters2.result);
        }

        [TestMethod]
        public void simpleSuccessfulSubmitPurchaseTest()
        {
            ItemSubmitOfferPurchase submitOfferPurchaseA = new ItemSubmitOfferPurchase(9.0);
            addItemToShoppingCart(store.storeID, itemIDA, 2, submitOfferPurchaseA);
            // can do the purchase
            user.shoppingCart.itemPriceStatusDecision(store.storeID, itemIDA, PriceStatus.Approved);
            ResultWithValue<ConcurrentLinkedList<String>> purchaseRes = purchaseItems();
            Assert.IsTrue(purchaseRes.getTag());
            Assert.AreEqual(1, user.purchases.Count);
            Assert.IsTrue(user.shoppingCart.isEmpty());

            user.purchases.Clear();
        }

        [TestMethod]
        public void simpleFailierSubmitPurchaseTest()
        {
            ItemSubmitOfferPurchase submitOfferPurchaseA = new ItemSubmitOfferPurchase(9.0);
            addItemToShoppingCart(store.storeID, itemIDA, 2, submitOfferPurchaseA);
            // do not approve the offer - cannot do the purchase
            ResultWithValue<ConcurrentLinkedList<String>> purchaseRes = purchaseItems();
            Assert.IsFalse(purchaseRes.getTag());
            Assert.AreEqual(0, user.purchases.Count);
            Assert.IsTrue(user.shoppingCart.shoppingBags[store.storeID].items.ContainsKey(itemIDA));

            cleaningCart();
        }

        [TestMethod]
        public void submitAndImmediatePurchaseTest()
        {
            ItemSubmitOfferPurchase submitOfferPurchaseA = new ItemSubmitOfferPurchase(9.0);
            addItemToShoppingCart(store.storeID, itemIDA, 2, submitOfferPurchaseA);
            ItemImmediatePurchase immediatePurchaseB = new ItemImmediatePurchase(10.0);
            addItemToShoppingCart(store.storeID, itemIDB, 2, immediatePurchaseB);
            // can do the purchase
            user.shoppingCart.itemPriceStatusDecision(store.storeID, itemIDA, PriceStatus.Approved);

            ResultWithValue<ConcurrentLinkedList<String>> purchaseRes = purchaseItems();
            Assert.IsTrue(purchaseRes.getTag());
            Assert.AreEqual(1, user.purchases.Count);
            Assert.IsTrue(user.shoppingCart.isEmpty());

            user.purchases.Clear();
        }

        [TestMethod]
        public void complexPurchaseTest()
        {
            ItemSubmitOfferPurchase submitOfferPurchaseA = new ItemSubmitOfferPurchase(5.0);
            addItemToShoppingCart(store.storeID, itemIDA, 2, submitOfferPurchaseA);
            ItemImmediatePurchase immediatePurchaseB = new ItemImmediatePurchase(10.0);
            addItemToShoppingCart(store.storeID, itemIDB, 2, immediatePurchaseB);
            // reject
            user.shoppingCart.itemPriceStatusDecision(store.storeID, itemIDA, PriceStatus.Rejected);

            ResultWithValue<ConcurrentLinkedList<String>> purchaseRes = purchaseItems();
            Assert.IsFalse(purchaseRes.getTag());

            submitOffer(store.storeID, itemIDA, 9.0);
            // approve
            user.shoppingCart.itemPriceStatusDecision(store.storeID, itemIDA, PriceStatus.Approved);

            purchaseRes = purchaseItems();
            Assert.IsTrue(purchaseRes.getTag());
            Assert.AreEqual(1, user.purchases.Count);
            Assert.IsTrue(user.shoppingCart.isEmpty());

            user.purchases.Clear();
        }
    }
}
