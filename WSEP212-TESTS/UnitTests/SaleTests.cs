using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class SaleTests
    {
        public static PurchaseDetails purchaseDetails;
        public static Item itemA;
        public static Item itemB;
        public static Item itemC;

        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
            User user = new User("Sagiv", 21);
            itemA = new Item(100, "bamba", "snack for childrens", 4.5, "snack");
            itemB = new Item(500, "milk", "pasteurized milk", 10, "milk products");
            itemC = new Item(100, "bisli", "snack for childrens", 4, "snack");
            ConcurrentDictionary<Item, int> shoppingBagItems = new ConcurrentDictionary<Item, int>();
            shoppingBagItems.TryAdd(itemA, 3);
            shoppingBagItems.TryAdd(itemB, 2);
            shoppingBagItems.TryAdd(itemC, 5);
            ConcurrentDictionary<int, PurchaseType> itemsPurchaseType = new ConcurrentDictionary<int, PurchaseType>();
            itemsPurchaseType.TryAdd(itemA.itemID, PurchaseType.ImmediatePurchase);
            itemsPurchaseType.TryAdd(itemB.itemID, PurchaseType.ImmediatePurchase);
            itemsPurchaseType.TryAdd(itemC.itemID, PurchaseType.ImmediatePurchase);
            purchaseDetails = new PurchaseDetails(user, shoppingBagItems, itemsPurchaseType);
        }

        [TestMethod]
        public void simpleSaleOnItemTest()
        {
            ApplySaleOn saleOnItem = new SaleOnItem(itemB.itemID);
            Sale simpleSale = new SimpleSale(25, saleOnItem);
            Assert.AreEqual(7.5, simpleSale.applySaleOnItem(itemB, purchaseDetails));
            Assert.AreEqual(4, simpleSale.applySaleOnItem(itemC, purchaseDetails));
        }

        [TestMethod]
        public void simpleSaleOnCategoryTest()
        {
            ApplySaleOn saleOnCategory = new SaleOnCategory("snack");
            Sale simpleSale = new SimpleSale(50, saleOnCategory);
            Assert.AreEqual(2.25, simpleSale.applySaleOnItem(itemA, purchaseDetails));
            Assert.AreEqual(2, simpleSale.applySaleOnItem(itemC, purchaseDetails));
            Assert.AreEqual(10, simpleSale.applySaleOnItem(itemB, purchaseDetails));
        }

        [TestMethod]
        public void simpleSaleOnStoreTest()
        {
            ApplySaleOn saleOnStore = new SaleOnAllStore();
            Sale simpleSale = new SimpleSale(50, saleOnStore);
            Assert.AreEqual(2.25, simpleSale.applySaleOnItem(itemA, purchaseDetails));
            Assert.AreEqual(2, simpleSale.applySaleOnItem(itemC, purchaseDetails));
            Assert.AreEqual(5, simpleSale.applySaleOnItem(itemB, purchaseDetails));
        }

        [TestMethod]
        public void conditionalSaleMetTest()
        {
            ApplySaleOn saleOnCategory = new SaleOnCategory("snack");
            Predicate<PurchaseDetails> predicate = pd => pd.numOfItemsInPurchase() >= 10;
            PolicyPredicate policyPredicate = new SimplePredicate(predicate);
            Sale conditionalSale = new ConditionalSale(50, saleOnCategory, policyPredicate);
            Assert.AreEqual(2.25, conditionalSale.applySaleOnItem(itemA, purchaseDetails));
            Assert.AreEqual(2, conditionalSale.applySaleOnItem(itemC, purchaseDetails));
            Assert.AreEqual(10, conditionalSale.applySaleOnItem(itemB, purchaseDetails));
        }

        [TestMethod]
        public void conditionalSaleNotMetTest()
        {
            ApplySaleOn saleOnCategory = new SaleOnCategory("snack");
            Predicate<PurchaseDetails> p1 = pd => pd.numOfItemsInPurchase() >= 10;
            PolicyPredicate sp1 = new SimplePredicate(p1);
            Predicate<PurchaseDetails> p2 = pd => pd.totalPurchasePrice() >= 100;
            PolicyPredicate sp2 = new SimplePredicate(p2);
            ConcurrentLinkedList<PolicyPredicate> andPred = new ConcurrentLinkedList<PolicyPredicate>();
            andPred.TryAdd(sp1);
            andPred.TryAdd(sp2);
            PolicyPredicate policyPredicate = new AndPredicates(andPred);
            Sale conditionalSale = new ConditionalSale(50, saleOnCategory, policyPredicate);
            Assert.AreEqual(4.5, conditionalSale.applySaleOnItem(itemA, purchaseDetails));
            Assert.AreEqual(4, conditionalSale.applySaleOnItem(itemC, purchaseDetails));
            Assert.AreEqual(10, conditionalSale.applySaleOnItem(itemB, purchaseDetails));
        }

        [TestMethod]
        public void maxSaleTest()
        {
            // sale 1
            ApplySaleOn saleOnCategory = new SaleOnCategory("snack");
            Predicate<PurchaseDetails> predicate = pd => pd.numOfItemsInPurchase() >= 10;
            PolicyPredicate policyPredicate = new SimplePredicate(predicate);
            Sale conditionalSale = new ConditionalSale(50, saleOnCategory, policyPredicate);
            // sale 2
            ApplySaleOn saleOnStore = new SaleOnAllStore();
            Sale simpleSale = new SimpleSale(50, saleOnStore);

            ConcurrentLinkedList<Sale> sales = new ConcurrentLinkedList<Sale>();
            sales.TryAdd(conditionalSale);
            sales.TryAdd(simpleSale);
            Sale maxSale = new MaxSale(sales);
            Assert.AreEqual(2.25, maxSale.applySaleOnItem(itemA, purchaseDetails));
            Assert.AreEqual(2, maxSale.applySaleOnItem(itemC, purchaseDetails));
            Assert.AreEqual(5, maxSale.applySaleOnItem(itemB, purchaseDetails));
        }

        [TestMethod]
        public void doubleSaleTest()
        {
            // sale 1
            ApplySaleOn saleOnCategory = new SaleOnCategory("snack");
            Predicate<PurchaseDetails> predicate = pd => pd.numOfItemsInPurchase() >= 10;
            PolicyPredicate policyPredicate = new SimplePredicate(predicate);
            Sale conditionalSale = new ConditionalSale(25, saleOnCategory, policyPredicate);
            // sale 2
            ApplySaleOn saleOnStore = new SaleOnAllStore();
            Sale simpleSale = new SimpleSale(25, saleOnStore);

            ConcurrentLinkedList<Sale> sales = new ConcurrentLinkedList<Sale>();
            sales.TryAdd(conditionalSale);
            sales.TryAdd(simpleSale);
            Sale doubleSale = new DoubleSales(sales);
            Assert.AreEqual(2.25, doubleSale.applySaleOnItem(itemA, purchaseDetails));
            Assert.AreEqual(2, doubleSale.applySaleOnItem(itemC, purchaseDetails));
            Assert.AreEqual(7.5, doubleSale.applySaleOnItem(itemB, purchaseDetails));
        }

        [TestMethod]
        public void xorSaleTest()
        {
            // sale 1
            ApplySaleOn saleOnCategory = new SaleOnCategory("snack");
            Predicate<PurchaseDetails> predicate = pd => pd.numOfItemsInPurchase() >= 10;
            PolicyPredicate policyPredicate = new SimplePredicate(predicate);
            Sale conditionalSale = new ConditionalSale(25, saleOnCategory, policyPredicate);
            // sale 2
            ApplySaleOn saleOnStore = new SaleOnAllStore();
            Sale simpleSale = new SimpleSale(25, saleOnStore);

            Sale xorSale = new XorSale(conditionalSale, simpleSale, );
            
        }
    }
}
