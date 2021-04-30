using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.DomainLayer;
using System.Collections.Concurrent;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TESTS.IntegrationTests
{
    [TestClass]
    public class StorePoliciesManagementTests
    {
        public static User user;
        public static Store store;
        public static ConcurrentDictionary<int, int> shoppingBagItems;
        public static ConcurrentDictionary<int, PurchaseType> itemsPurchaseType;
        public static int milkID;

        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
            user = new User("Sagiv", 21);
            store = new Store("Adidas", "Rabinovich 35, Holon", new SalePolicy("DEFAULT"), new PurchasePolicy("DEFAULT"), user);

            shoppingBagItems = new ConcurrentDictionary<int, int>();
            itemsPurchaseType = new ConcurrentDictionary<int, PurchaseType>();

            int itemID = store.addItemToStorage(500, "bamba", "snack for childrens", 4.5, "snack").getValue();
            shoppingBagItems.TryAdd(itemID, 2);
            itemsPurchaseType.TryAdd(itemID, PurchaseType.ImmediatePurchase);
            milkID = store.addItemToStorage(500, "milk", "pasteurized milk", 8, "milk products").getValue();
            shoppingBagItems.TryAdd(milkID, 2);
            itemsPurchaseType.TryAdd(milkID, PurchaseType.ImmediatePurchase);
            itemID = store.addItemToStorage(500, "bisli", "snack for childrens", 4, "snack").getValue();
            shoppingBagItems.TryAdd(itemID, 5);
            itemsPurchaseType.TryAdd(itemID, PurchaseType.ImmediatePurchase);
        }

        [TestMethod]
        public void noPolicyTest()
        {
            ResultWithValue<double> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPurchaseType);
            Assert.IsTrue(purchaseRes.getTag());
            Assert.AreEqual(45.0, purchaseRes.getValue());
        }

        [TestMethod]
        public void approvedByPurchasePolicySimpleTest()
        {
            Predicate<PurchaseDetails> pred = pd => pd.numOfItemsInPurchase() >= 2;
            int predicateID = store.addPurchasePredicate(pred);

            ResultWithValue<double> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPurchaseType);
            Assert.IsTrue(purchaseRes.getTag());
            Assert.AreEqual(45.0, purchaseRes.getValue());

            store.removePurchasePredicate(predicateID);
        }

        [TestMethod]
        public void approvedByPurchasePolicyComplexTest()
        {
            Predicate<PurchaseDetails> pred1 = pd => pd.numOfItemsInPurchase() >= 2;
            int predID1 = store.addPurchasePredicate(pred1);
            Predicate<PurchaseDetails> pred2 = pd => pd.totalPurchasePrice() >= 100;
            int predID2 = store.addPurchasePredicate(pred2);
            Predicate<PurchaseDetails> pred3 = pd => pd.user.userAge >= 18;
            int predID3 = store.addPurchasePredicate(pred3);
            int composedID = store.composePurchasePredicates(predID2, predID3, PurchasePredicateCompositionType.OrComposition).getValue();

            ResultWithValue<double> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPurchaseType);
            Assert.IsTrue(purchaseRes.getTag());
            Assert.AreEqual(45.0, purchaseRes.getValue());

            store.removePurchasePredicate(predID1);
            store.removePurchasePredicate(composedID);
        }

        [TestMethod]
        public void rejectedByPurchasePolicySimpleTest()
        {
            Predicate<PurchaseDetails> pred = pd => pd.numOfItemsInPurchase() >= 20;
            int predicateID = store.addPurchasePredicate(pred);

            ResultWithValue<double> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPurchaseType);
            Assert.IsFalse(purchaseRes.getTag());

            store.removePurchasePredicate(predicateID);
        }

        [TestMethod]
        public void rejectedByPurchasePolicyComplexTest()
        {
            Predicate<PurchaseDetails> pred1 = pd => pd.numOfItemsInPurchase() >= 2;
            int predID1 = store.addPurchasePredicate(pred1);
            Predicate<PurchaseDetails> pred2 = pd => pd.totalPurchasePrice() >= 100;
            int predID2 = store.addPurchasePredicate(pred2);
            Predicate<PurchaseDetails> pred3 = pd => pd.user.userAge >= 18;
            int predID3 = store.addPurchasePredicate(pred3);
            int composedID = store.composePurchasePredicates(predID2, predID3, PurchasePredicateCompositionType.ConditionalComposition).getValue();

            ResultWithValue<double> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPurchaseType);
            Assert.IsFalse(purchaseRes.getTag());

            store.removePurchasePredicate(predID1);
            store.removePurchasePredicate(composedID);
        }

        [TestMethod]
        public void appliedSaleSimpleTest()
        {
            ApplySaleOn saleOn = new SaleOnCategory("snack");
            int saleID = store.addSale(50, saleOn);

            ResultWithValue<double> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPurchaseType);
            Assert.IsTrue(purchaseRes.getTag());
            Assert.AreEqual(30.5, purchaseRes.getValue());

            store.removeSale(saleID);
        }

        [TestMethod]
        public void appliedSaleComplexTest()
        {
            ApplySaleOn saleOnSnacks = new SaleOnCategory("snack");
            int saleID1 = store.addSale(50, saleOnSnacks);
            ApplySaleOn saleOnMilk = new SaleOnItem(milkID);
            int saleID2 = store.addSale(50, saleOnMilk);
            ApplySaleOn saleOnAllStore = new SaleOnAllStore();
            int saleID3 = store.addSale(25, saleOnAllStore);
            int composedID = store.composeSales(saleID1, saleID3, SaleCompositionType.MaxComposition, null).getValue();
            composedID = store.composeSales(saleID2, composedID, SaleCompositionType.MaxComposition, null).getValue();

            ResultWithValue<double> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPurchaseType);
            Assert.IsTrue(purchaseRes.getTag());
            Assert.AreEqual(30.5, purchaseRes.getValue());

            store.removeSale(composedID);
        }

        [TestMethod]
        public void appliedConditionalSaleTest()
        {
            ApplySaleOn saleOn = new SaleOnCategory("snack");
            int saleID = store.addSale(50, saleOn);
            Predicate<PurchaseDetails> pred = pd => pd.totalPurchasePrice() > 40;
            saleID = store.addSaleCondition(saleID, pred, SalePredicateCompositionType.AndComposition).getValue();

            ResultWithValue<double> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPurchaseType);
            Assert.IsTrue(purchaseRes.getTag());
            Assert.AreEqual(30.5, purchaseRes.getValue());

            store.removeSale(saleID);
        }

        [TestMethod]
        public void notAppliedConditionalSaleTest()
        {
            ApplySaleOn saleOn = new SaleOnCategory("snack");
            int saleID = store.addSale(50, saleOn);
            Predicate<PurchaseDetails> pred = pd => pd.totalPurchasePrice() > 50;
            saleID = store.addSaleCondition(saleID, pred, SalePredicateCompositionType.AndComposition).getValue();

            ResultWithValue<double> purchaseRes = store.purchaseItems(user, shoppingBagItems, itemsPurchaseType);
            Assert.IsTrue(purchaseRes.getTag());
            Assert.AreEqual(45.0, purchaseRes.getValue());

            store.removeSale(saleID);
        }
    }
}
