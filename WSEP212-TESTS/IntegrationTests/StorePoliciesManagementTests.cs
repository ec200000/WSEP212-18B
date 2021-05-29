using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.DomainLayer;
using System.Collections.Concurrent;
using WSEP212.ServiceLayer.Result;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.DomainLayer.PurchaseTypes;

namespace WSEP212_TESTS.IntegrationTests
{
    [TestClass]
    public class StorePoliciesManagementTests
    {
        public static User user;
        public static Store store;
        public static ConcurrentDictionary<int, int> shoppingBagItems;
        public static ConcurrentDictionary<int, double> itemsPrices;
        public static int milkID;

        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
            user = new User("Sagiv", 21);
            store = new Store("Adidas", "Rabinovich 35, Holon", new SalePolicy("DEFAULT"), new PurchasePolicy("DEFAULT"), user);
            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
            store.deliverySystem = DeliverySystemMock.Instance;

            shoppingBagItems = new ConcurrentDictionary<int, int>();
            itemsPrices = new ConcurrentDictionary<int, double>();

            int itemID = store.addItemToStorage(500, "bamba", "snack for childrens", 4.5, ItemCategory.Snacks).getValue();
            shoppingBagItems.TryAdd(itemID, 2);
            itemsPrices.TryAdd(itemID, 4.5);
            milkID = store.addItemToStorage(500, "milk", "pasteurized milk", 8, ItemCategory.Dairy).getValue();
            shoppingBagItems.TryAdd(milkID, 2);
            itemsPrices.TryAdd(milkID, 8);
            itemID = store.addItemToStorage(500, "bisli", "snack for childrens", 4, ItemCategory.Snacks).getValue();
            shoppingBagItems.TryAdd(itemID, 5);
            itemsPrices.TryAdd(itemID, 4);
        }

        private void noSalePurchase(ResultWithValue<ConcurrentDictionary<int, double>> purchaseRes)
        {
            ConcurrentDictionary<int, double> noSaleItemsPrices = purchaseRes.getValue();
            // 3 diffrenet items
            Assert.AreEqual(3, noSaleItemsPrices.Count);
            foreach (KeyValuePair<int, double> itemAndPrice in itemsPrices)
            {
                Assert.IsTrue(noSaleItemsPrices.ContainsKey(itemAndPrice.Key));
                Assert.AreEqual(itemAndPrice.Value, noSaleItemsPrices[itemAndPrice.Key]);
            }
        }

        [TestMethod]
        public void noPolicyTest()
        {
            ResultWithValue<ConcurrentDictionary<int, double>> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPrices);
            Assert.IsTrue(purchaseRes.getTag());
            noSalePurchase(purchaseRes);
        }

        [TestMethod]
        public void approvedByPurchasePolicySimpleTest()
        {
            LocalPredicate<PurchaseDetails> pred = new LocalPredicate<PurchaseDetails>(pd => pd.numOfItemsInPurchase(), 2);
            int predicateID = store.addPurchasePredicate(pred, "more then 2 items in bag");

            ResultWithValue<ConcurrentDictionary<int, double>> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPrices);
            Assert.IsTrue(purchaseRes.getTag());
            noSalePurchase(purchaseRes);

            store.removePurchasePredicate(predicateID);
        }

        [TestMethod]
        public void approvedByPurchasePolicyComplexTest()
        {
            LocalPredicate<PurchaseDetails> pred1 = new LocalPredicate<PurchaseDetails>(pd => pd.numOfItemsInPurchase(), 2);
            int predID1 = store.addPurchasePredicate(pred1, "more then 2 items in bag");
            LocalPredicate<PurchaseDetails> pred2 = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 100);
            int predID2 = store.addPurchasePredicate(pred2, "price is more then 100");
            LocalPredicate<PurchaseDetails> pred3 = new LocalPredicate<PurchaseDetails>(pd => pd.user.userAge, 18);
            int predID3 = store.addPurchasePredicate(pred3, "user age is more then 18");
            int composedID = store.composePurchasePredicates(predID2, predID3, PurchasePredicateCompositionType.OrComposition).getValue();

            ResultWithValue<ConcurrentDictionary<int, double>> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPrices);
            Assert.IsTrue(purchaseRes.getTag());
            noSalePurchase(purchaseRes);

            store.removePurchasePredicate(predID1);
            store.removePurchasePredicate(composedID);
        }

        [TestMethod]
        public void rejectedByPurchasePolicySimpleTest()
        {
            LocalPredicate<PurchaseDetails> pred = new LocalPredicate<PurchaseDetails>(pd => pd.numOfItemsInPurchase(), 20);
            int predicateID = store.addPurchasePredicate(pred, "more then 20 items in bag");

            ResultWithValue<ConcurrentDictionary<int, double>> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPrices);
            Assert.IsFalse(purchaseRes.getTag());

            store.removePurchasePredicate(predicateID);
        }

        [TestMethod]
        public void rejectedByPurchasePolicyComplexTest()
        {
            LocalPredicate<PurchaseDetails> pred1 = new LocalPredicate<PurchaseDetails>(pd => pd.numOfItemsInPurchase(), 2);
            int predID1 = store.addPurchasePredicate(pred1, "more then 2 items in bag");
            LocalPredicate<PurchaseDetails> pred2 = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 100);
            int predID2 = store.addPurchasePredicate(pred2, "price is more then 100");
            LocalPredicate<PurchaseDetails> pred3 = new LocalPredicate<PurchaseDetails>(pd => pd.user.userAge, 18);
            int predID3 = store.addPurchasePredicate(pred3, "user age is more then 18");
            int composedID = store.composePurchasePredicates(predID2, predID3, PurchasePredicateCompositionType.ConditionalComposition).getValue();

            ResultWithValue<ConcurrentDictionary<int, double>> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPrices);
            Assert.IsFalse(purchaseRes.getTag());

            store.removePurchasePredicate(predID1);
            store.removePurchasePredicate(composedID);
        }

        private void saleOnSancks(ResultWithValue<ConcurrentDictionary<int, double>> purchaseRes)
        {
            ConcurrentDictionary<int, double> saleItemsPrices = purchaseRes.getValue();
            // 3 diffrenet items
            Assert.AreEqual(3, saleItemsPrices.Count);
            foreach (KeyValuePair<int, double> itemAndPrice in itemsPrices)
            {
                Assert.IsTrue(saleItemsPrices.ContainsKey(itemAndPrice.Key));
                if (itemAndPrice.Key == milkID)
                {
                    Assert.AreEqual(itemAndPrice.Value, saleItemsPrices[itemAndPrice.Key]);
                }
                else
                {
                    Assert.AreEqual(itemAndPrice.Value, saleItemsPrices[itemAndPrice.Key] * 2);
                }
            }
        }

        [TestMethod]
        public void appliedSaleSimpleTest()
        {
            ApplySaleOn saleOn = new SaleOnCategory(ItemCategory.Snacks);
            int saleID = store.addSale(50, saleOn, "50% sale on sancks");

            ResultWithValue<ConcurrentDictionary<int, double>> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPrices);
            Assert.IsTrue(purchaseRes.getTag());
            saleOnSancks(purchaseRes);

            store.removeSale(saleID);
        }

        [TestMethod]
        public void appliedSaleComplexTest()
        {
            ApplySaleOn saleOnSnacks = new SaleOnCategory(ItemCategory.Snacks);
            int saleID1 = store.addSale(50, saleOnSnacks, "50% sale on sancks");
            ApplySaleOn saleOnMilk = new SaleOnItem(milkID);
            int saleID2 = store.addSale(50, saleOnMilk, "50% sale on milk");
            ApplySaleOn saleOnAllStore = new SaleOnAllStore();
            int saleID3 = store.addSale(25, saleOnAllStore, "25% sale on all store");
            int composedID = store.composeSales(saleID1, saleID3, SaleCompositionType.MaxComposition, null).getValue();
            composedID = store.composeSales(saleID2, composedID, SaleCompositionType.MaxComposition, null).getValue();

            ResultWithValue<ConcurrentDictionary<int, double>> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPrices);
            Assert.IsTrue(purchaseRes.getTag());
            saleOnSancks(purchaseRes);

            store.removeSale(composedID);
        }

        [TestMethod]
        public void appliedConditionalSaleTest()
        {
            ApplySaleOn saleOn = new SaleOnCategory(ItemCategory.Snacks);
            int saleID = store.addSale(50, saleOn, "50% sale on sancks");
            LocalPredicate<PurchaseDetails> pred = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 40);
            SimplePredicate predicate = new SimplePredicate(pred, "total proce is more then 40");
            saleID = store.addSaleCondition(saleID, predicate, SalePredicateCompositionType.AndComposition).getValue();

            ResultWithValue<ConcurrentDictionary<int, double>> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPrices);
            Assert.IsTrue(purchaseRes.getTag());
            saleOnSancks(purchaseRes);

            store.removeSale(saleID);
        }

        [TestMethod]
        public void notAppliedConditionalSaleTest()
        {
            ApplySaleOn saleOn = new SaleOnCategory(ItemCategory.Snacks);
            int saleID = store.addSale(50, saleOn, "50% sale on sancks");
            LocalPredicate<PurchaseDetails> pred = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 50);
            SimplePredicate predicate = new SimplePredicate(pred, "total proce is more then 50");
            saleID = store.addSaleCondition(saleID, predicate, SalePredicateCompositionType.AndComposition).getValue();

            ResultWithValue<ConcurrentDictionary<int, double>> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPrices);
            Assert.IsTrue(purchaseRes.getTag());
            noSalePurchase(purchaseRes);

            store.removeSale(saleID);
        }
    }
}
