using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class PurchaseTypeTests
    {
        private static Store shoppingBagStore;
        private static int storeItemID;
        private static ShoppingBag shoppingBag;

        [ClassInitialize]
        public void init()
        {
            SystemDBAccess.mock = true;
        }
        
        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
            ResultWithValue<int> addStoreRes = StoreRepository.Instance.addStore("SUPER PHARAM", "Ashdod", new SalePolicy("DEFUALT"), new PurchasePolicy("DEFUALT"), new User("admin"));
            shoppingBagStore = StoreRepository.Instance.getStore(addStoreRes.getValue()).getValue();
            storeItemID = shoppingBagStore.addItemToStorage(500, "black masks", "protects against infection of covid-19", 10, ItemCategory.Health).getValue();
            UserRepository.Instance.insertNewUser(new User("Sagiv", 21), "qwerty");
            shoppingBag = new ShoppingBag(shoppingBagStore, "Sagiv");
        }

        [ClassCleanup]
        public static void cleanUp()
        {
            StoreRepository.Instance.removeStore(shoppingBagStore.storeID);
        }

        private void supportPurchaseType(PurchaseType purchaseType)
        {
            shoppingBagStore.supportPurchaseType(purchaseType);
        }

        private void unsupportAndCleanBag(PurchaseType purchaseType)
        {
            shoppingBagStore.unsupportPurchaseType(purchaseType);
            shoppingBag.clearShoppingBag();
        }

        [TestMethod]
        public void supportPurchaseTypeTest()
        {
            shoppingBagStore.supportPurchaseType(PurchaseType.ImmediatePurchase);
            Assert.IsTrue(shoppingBagStore.isStoreSupportPurchaseType(PurchaseType.ImmediatePurchase));

            unsupportAndCleanBag(PurchaseType.ImmediatePurchase);
        }

        [TestMethod]
        public void unsupportPurchaseTypeTest()
        {
            shoppingBagStore.supportPurchaseType(PurchaseType.ImmediatePurchase);
            Assert.IsTrue(shoppingBagStore.isStoreSupportPurchaseType(PurchaseType.ImmediatePurchase));
            shoppingBagStore.unsupportPurchaseType(PurchaseType.ImmediatePurchase);
            Assert.IsFalse(shoppingBagStore.isStoreSupportPurchaseType(PurchaseType.ImmediatePurchase));
        }

        [TestMethod]
        public void submitOfferOnImmediateTest()
        {
            supportPurchaseType(PurchaseType.ImmediatePurchase);
            ItemPurchaseType purchaseType = new ItemImmediatePurchase(10.0);
            shoppingBag.addItem(storeItemID, 2, purchaseType);

            RegularResult res = shoppingBag.submitItemPriceOffer(storeItemID, 7.5);
            Assert.IsFalse(res.getTag());
            Assert.AreEqual(10.0, shoppingBag.itemsPurchaseTypes[storeItemID].getCurrentPrice());

            unsupportAndCleanBag(PurchaseType.ImmediatePurchase);
        }

        [TestMethod]
        public void submitOfferSuccessfullyTest()
        {
            supportPurchaseType(PurchaseType.SubmitOfferPurchase);
            ItemPurchaseType purchaseType = new ItemSubmitOfferPurchase(7.5);
            RegularResult resu = shoppingBag.addItem(storeItemID, 2, purchaseType);
            Assert.AreEqual(7.5, shoppingBag.itemsPurchaseTypes[storeItemID].getCurrentPrice());
            Assert.AreEqual(PriceStatus.Pending, shoppingBag.itemsPurchaseTypes[storeItemID].getPriceStatus());

            RegularResult res = shoppingBag.submitItemPriceOffer(storeItemID, 7);
            Assert.IsTrue(res.getTag());
            Assert.AreEqual(7, shoppingBag.itemsPurchaseTypes[storeItemID].getCurrentPrice());
            Assert.AreEqual(PriceStatus.Pending, shoppingBag.itemsPurchaseTypes[storeItemID].getPriceStatus());

            unsupportAndCleanBag(PurchaseType.SubmitOfferPurchase);
        }

        [TestMethod]
        public void approveOfferSuccessfullyTest()
        {
            supportPurchaseType(PurchaseType.SubmitOfferPurchase);
            ItemPurchaseType purchaseType = new ItemSubmitOfferPurchase(7.5);
            shoppingBag.addItem(storeItemID, 2, purchaseType);

            RegularResult res = shoppingBag.itemPriceStatusDecision(storeItemID, PriceStatus.Approved);
            Assert.IsTrue(res.getTag());
            Assert.AreEqual(7.5, shoppingBag.itemsPurchaseTypes[storeItemID].getCurrentPrice());
            Assert.AreEqual(PriceStatus.Approved, shoppingBag.itemsPurchaseTypes[storeItemID].getPriceStatus());

            unsupportAndCleanBag(PurchaseType.SubmitOfferPurchase);
        }

        [TestMethod]
        public void rejectOfferSuccessfullyTest()
        {
            supportPurchaseType(PurchaseType.SubmitOfferPurchase);
            ItemPurchaseType purchaseType = new ItemSubmitOfferPurchase(7.5);
            shoppingBag.addItem(storeItemID, 2, purchaseType);

            RegularResult res = shoppingBag.itemPriceStatusDecision(storeItemID, PriceStatus.Rejected);
            Assert.IsTrue(res.getTag());
            Assert.AreEqual(7.5, shoppingBag.itemsPurchaseTypes[storeItemID].getCurrentPrice());
            Assert.AreEqual(PriceStatus.Rejected, shoppingBag.itemsPurchaseTypes[storeItemID].getPriceStatus());

            unsupportAndCleanBag(PurchaseType.SubmitOfferPurchase);
        }

        [TestMethod]
        public void rejectOfferOnImmediateTest()
        {
            supportPurchaseType(PurchaseType.ImmediatePurchase);
            ItemPurchaseType purchaseType = new ItemImmediatePurchase(10.0);
            shoppingBag.addItem(storeItemID, 2, purchaseType);

            RegularResult res = shoppingBag.itemPriceStatusDecision(storeItemID, PriceStatus.Rejected);
            // cannot change status of immediate purchase - always approved
            Assert.IsFalse(res.getTag());
            Assert.AreEqual(10.0, shoppingBag.itemsPurchaseTypes[storeItemID].getCurrentPrice());
            Assert.AreEqual(PriceStatus.Approved, shoppingBag.itemsPurchaseTypes[storeItemID].getPriceStatus());

            unsupportAndCleanBag(PurchaseType.ImmediatePurchase);
        }

        [TestMethod]
        public void countingOfferTest()
        {
            supportPurchaseType(PurchaseType.SubmitOfferPurchase);
            ItemPurchaseType purchaseType = new ItemSubmitOfferPurchase(7.5);
            shoppingBag.addItem(storeItemID, 2, purchaseType);

            RegularResult res = shoppingBag.counterOffer(storeItemID, 8.0);
            Assert.IsTrue(res.getTag());
            Assert.AreEqual(8.0, shoppingBag.itemsPurchaseTypes[storeItemID].getCurrentPrice());
            Assert.AreEqual(PriceStatus.Approved, shoppingBag.itemsPurchaseTypes[storeItemID].getPriceStatus());

            unsupportAndCleanBag(PurchaseType.SubmitOfferPurchase);
        }
    }
}
