﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;

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
            itemA = new Item(100, "bamba", "snack for childrens", 4.5, ItemCategory.Snacks);
            itemB = new Item(500, "milk", "pasteurized milk", 10, ItemCategory.Dairy);
            itemC = new Item(100, "bisli", "snack for childrens", 4, ItemCategory.Snacks);
            ConcurrentDictionary<Item, int> shoppingBagItems = new ConcurrentDictionary<Item, int>();
            shoppingBagItems.TryAdd(itemA, 3);
            shoppingBagItems.TryAdd(itemB, 2);
            shoppingBagItems.TryAdd(itemC, 5);
            ConcurrentDictionary<int, double> itemsPrices = new ConcurrentDictionary<int, double>();
            itemsPrices.TryAdd(itemA.itemID, 4.5);
            itemsPrices.TryAdd(itemB.itemID, 10);
            itemsPrices.TryAdd(itemC.itemID, 4);
            purchaseDetails = new PurchaseDetails(user, shoppingBagItems, itemsPrices);
        }

        [TestMethod]
        public void simpleSaleOnItemTest()
        {
            ApplySaleOn saleOnItem = new SaleOnItem(itemB.itemID);
            Sale simpleSale = new SimpleSale(25, saleOnItem, "25% sale on milk");
            Assert.AreEqual(7.5, simpleSale.applySaleOnItem(itemB, 10, purchaseDetails));
            Assert.AreEqual(4, simpleSale.applySaleOnItem(itemC, 4, purchaseDetails));
        }

        [TestMethod]
        public void simpleSaleOnCategoryTest()
        {
            ApplySaleOn saleOnCategory = new SaleOnCategory(ItemCategory.Snacks);
            Sale simpleSale = new SimpleSale(50, saleOnCategory, "50% sale on snacks");
            Assert.AreEqual(2.25, simpleSale.applySaleOnItem(itemA, 4.5, purchaseDetails));
            Assert.AreEqual(2, simpleSale.applySaleOnItem(itemC, 4, purchaseDetails));
            Assert.AreEqual(10, simpleSale.applySaleOnItem(itemB, 10, purchaseDetails));
        }

        [TestMethod]
        public void simpleSaleOnStoreTest()
        {
            ApplySaleOn saleOnStore = new SaleOnAllStore();
            Sale simpleSale = new SimpleSale(50, saleOnStore, "50% sale on all store");
            Assert.AreEqual(2.25, simpleSale.applySaleOnItem(itemA, 4.5, purchaseDetails));
            Assert.AreEqual(2, simpleSale.applySaleOnItem(itemC, 4, purchaseDetails));
            Assert.AreEqual(5, simpleSale.applySaleOnItem(itemB, 10, purchaseDetails));
        }

        [TestMethod]
        public void conditionalSaleMetTest()
        {
            ApplySaleOn saleOnCategory = new SaleOnCategory(ItemCategory.Snacks);
            Predicate<PurchaseDetails> predicate = pd => pd.numOfItemsInPurchase() >= 10;
            SalePredicate policyPredicate = new SimplePredicate(predicate, "more then 10 items in bag");
            SimpleSale sale = new SimpleSale(50, saleOnCategory, "50% sale on snacks");
            Sale conditionalSale = new ConditionalSale(sale, policyPredicate);
            Assert.AreEqual(2.25, conditionalSale.applySaleOnItem(itemA, 4.5, purchaseDetails));
            Assert.AreEqual(2, conditionalSale.applySaleOnItem(itemC, 4, purchaseDetails));
            Assert.AreEqual(10, conditionalSale.applySaleOnItem(itemB, 10, purchaseDetails));
        }

        [TestMethod]
        public void conditionalSaleNotMetTest()
        {
            ApplySaleOn saleOnCategory = new SaleOnCategory(ItemCategory.Snacks);
            Predicate<PurchaseDetails> p1 = pd => pd.numOfItemsInPurchase() >= 10;
            SalePredicate sp1 = new SimplePredicate(p1, "more then 10 items in bag");
            Predicate<PurchaseDetails> p2 = pd => pd.totalPurchasePrice() >= 100;
            SalePredicate sp2 = new SimplePredicate(p2, "more then 100 price");
            SalePredicate policyPredicate = new AndPredicates(sp1, sp2);
            SimpleSale sale = new SimpleSale(50, saleOnCategory, "50% sale on snacks");
            Sale conditionalSale = new ConditionalSale(sale, policyPredicate);
            Assert.AreEqual(4.5, conditionalSale.applySaleOnItem(itemA, 4.5, purchaseDetails));
            Assert.AreEqual(4, conditionalSale.applySaleOnItem(itemC, 4, purchaseDetails));
            Assert.AreEqual(10, conditionalSale.applySaleOnItem(itemB, 10, purchaseDetails));
        }

        [TestMethod]
        public void maxSaleTest()
        {
            // sale 1
            ApplySaleOn saleOnCategory = new SaleOnCategory(ItemCategory.Snacks);
            Predicate<PurchaseDetails> predicate = pd => pd.numOfItemsInPurchase() >= 10;
            SalePredicate policyPredicate = new SimplePredicate(predicate, "more then 10 items in bag");
            SimpleSale sale = new SimpleSale(50, saleOnCategory, "50% sale on snacks");
            Sale conditionalSale = new ConditionalSale(sale, policyPredicate);
            // sale 2
            ApplySaleOn saleOnStore = new SaleOnAllStore();
            Sale simpleSale = new SimpleSale(50, saleOnStore, "50% sale on all store");

            Sale maxSale = new MaxSale(conditionalSale, simpleSale);
            Assert.AreEqual(2.25, maxSale.applySaleOnItem(itemA, 4.5, purchaseDetails));
            Assert.AreEqual(2, maxSale.applySaleOnItem(itemC, 4, purchaseDetails));
            Assert.AreEqual(5, maxSale.applySaleOnItem(itemB, 10, purchaseDetails));
        }

        [TestMethod]
        public void doubleSaleTest()
        {
            // sale 1
            ApplySaleOn saleOnCategory = new SaleOnCategory(ItemCategory.Snacks);
            Predicate<PurchaseDetails> predicate = pd => pd.numOfItemsInPurchase() >= 10;
            SalePredicate policyPredicate = new SimplePredicate(predicate, "more then 10 items in bag");
            SimpleSale sale = new SimpleSale(25, saleOnCategory, "25% sale on snacks");
            Sale conditionalSale = new ConditionalSale(sale, policyPredicate);
            // sale 2
            ApplySaleOn saleOnStore = new SaleOnAllStore();
            Sale simpleSale = new SimpleSale(25, saleOnStore, "25% sale on all store");

            Sale doubleSale = new DoubleSale(conditionalSale, simpleSale);
            Assert.AreEqual(2.25, doubleSale.applySaleOnItem(itemA, 4.5, purchaseDetails));
            Assert.AreEqual(2, doubleSale.applySaleOnItem(itemC, 4, purchaseDetails));
            Assert.AreEqual(7.5, doubleSale.applySaleOnItem(itemB, 10, purchaseDetails));
        }

        [TestMethod]
        public void xorSaleBothNotMetTest()
        {
            // sale 1
            ApplySaleOn saleOnStore = new SaleOnAllStore();
            Predicate<PurchaseDetails> predicate = pd => pd.numOfItemsInPurchase() > 10;
            SalePredicate policyPredicate = new SimplePredicate(predicate, "more then 10 items in bag");
            SimpleSale sale = new SimpleSale(25, saleOnStore, "25% sale on all store");
            Sale conditionalSale = new ConditionalSale(sale, policyPredicate);
            // sale 2
            ApplySaleOn saleOnCategory = new SaleOnCategory(ItemCategory.Snacks);
            Sale simpleSale = new SimpleSale(25, saleOnCategory, "25% sale on snacks");
            // selection rule
            SimplePredicate selectionRule = new SimplePredicate(pd => pd.totalPurchasePriceAfterSale(conditionalSale) <= pd.totalPurchasePriceAfterSale(simpleSale), "first sale is cheapest");

            Sale xorSale = new XorSale(conditionalSale, simpleSale, selectionRule);
            Assert.AreEqual(10, xorSale.applySaleOnItem(itemB, 10, purchaseDetails));
        }

        [TestMethod]
        public void xorSaleOnlyOneMetTest()
        {
            // sale 1
            ApplySaleOn saleOnStore = new SaleOnAllStore();
            Predicate<PurchaseDetails> predicate = pd => pd.numOfItemsInPurchase() > 10;
            SalePredicate policyPredicate = new SimplePredicate(predicate, "more then 10 items in bag");
            SimpleSale sale = new SimpleSale(25, saleOnStore, "25% sale on all store");
            Sale conditionalSale = new ConditionalSale(sale, policyPredicate);
            // sale 2
            ApplySaleOn saleOnCategory = new SaleOnCategory(ItemCategory.Dairy);
            Sale simpleSale = new SimpleSale(25, saleOnCategory, "25% sale on milk products");
            // selection rule
            SimplePredicate selectionRule = new SimplePredicate(pd => pd.totalPurchasePriceAfterSale(conditionalSale) <= pd.totalPurchasePriceAfterSale(simpleSale), "first sale is cheapest");

            Sale xorSale = new XorSale(conditionalSale, simpleSale, selectionRule);
            Assert.AreEqual(7.5, xorSale.applySaleOnItem(itemB, 10, purchaseDetails));
        }

        [TestMethod]
        public void xorSaleBothMetTest()
        {
            // sale 1
            ApplySaleOn saleOnStore = new SaleOnAllStore();
            Predicate<PurchaseDetails> predicate = pd => pd.numOfItemsInPurchase() > 5;
            SalePredicate policyPredicate = new SimplePredicate(predicate, "more then 5 items in bag");
            SimpleSale sale = new SimpleSale(50, saleOnStore, "50% sale on all store");
            Sale conditionalSale = new ConditionalSale(sale, policyPredicate);
            // sale 2
            ApplySaleOn saleOnCategory = new SaleOnCategory(ItemCategory.Dairy);
            Sale simpleSale = new SimpleSale(25, saleOnCategory, "50% sale on milk products");
            // selection rule
            SimplePredicate selectionRule = new SimplePredicate(pd => pd.totalPurchasePriceAfterSale(conditionalSale) <= pd.totalPurchasePriceAfterSale(simpleSale), "first sale is cheapest");

            Sale xorSale = new XorSale(conditionalSale, simpleSale, selectionRule);
            // choose the cheapest sale, selection rule
            Assert.AreEqual(5, xorSale.applySaleOnItem(itemB, 10, purchaseDetails));
            Assert.AreEqual(2, xorSale.applySaleOnItem(itemC, 4, purchaseDetails));
        }
    }
}
