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
    public class PolicyPredicateTests
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
            itemB = new Item(500, "milk", "pasteurized milk", 8, "milk products");
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
        public void simplePredicateMetTest()
        {
            Predicate<PurchaseDetails> predicate = pd => pd.totalPurchasePrice() > 40;
            SimplePredicate policyPredicate = new SimplePredicate(predicate);
            Assert.IsTrue(policyPredicate.applyPrediacte(purchaseDetails));

            predicate = pd => pd.atLeastNQuantity(itemC.itemID, 5);
            policyPredicate = new SimplePredicate(predicate);
            Assert.IsTrue(policyPredicate.applyPrediacte(purchaseDetails));
        }

        [TestMethod]
        public void simplePredicateNotMetTest()
        {
            Predicate<PurchaseDetails> predicate = pd => pd.totalPurchasePrice() > 60;
            SimplePredicate policyPredicate = new SimplePredicate(predicate);
            Assert.IsFalse(policyPredicate.applyPrediacte(purchaseDetails));

            predicate = pd => pd.atLeastNQuantity(itemA.itemID, 5);
            policyPredicate = new SimplePredicate(predicate);
            Assert.IsFalse(policyPredicate.applyPrediacte(purchaseDetails));
        }

        [TestMethod]
        public void conditioningPredicateMetTest()
        {
            // only if total price > 40 then you can buy 5 or more milk
            Predicate<PurchaseDetails> predicate = pd => pd.totalPurchasePrice() > 40;
            PolicyPredicate onlyif = new SimplePredicate(predicate);
            Predicate<PurchaseDetails> then = pd => pd.atLeastNQuantity(itemC.itemID, 5);
            ConditioningPredicate policyPredicate = new ConditioningPredicate(onlyif, then);
            Assert.IsTrue(policyPredicate.applyPrediacte(purchaseDetails));
        }

        [TestMethod]
        public void conditioningPredicateNotMetTest()
        {
            // only if total price > 60 then you can buy 5 or more milk
            Predicate<PurchaseDetails> predicate = pd => pd.totalPurchasePrice() > 60;
            PolicyPredicate onlyif = new SimplePredicate(predicate);
            Predicate<PurchaseDetails> then = pd => pd.atLeastNQuantity(itemC.itemID, 5);
            ConditioningPredicate policyPredicate = new ConditioningPredicate(onlyif, then);
            Assert.IsFalse(policyPredicate.applyPrediacte(purchaseDetails));
        }

        [TestMethod]
        public void composedPredicateMetTest()
        {
            Predicate<PurchaseDetails> p1 = pd => pd.totalPurchasePrice() > 60;
            PolicyPredicate sp1 = new SimplePredicate(p1);
            Predicate<PurchaseDetails> p2 = pd => pd.atMostNQuantity(itemB.itemID, 5);
            PolicyPredicate sp2 = new SimplePredicate(p2);
            Predicate<PurchaseDetails> p3 = pd => pd.user.userAge >= 18;
            PolicyPredicate sp3 = new SimplePredicate(p3);
            ConcurrentLinkedList<PolicyPredicate> orPred = new ConcurrentLinkedList<PolicyPredicate>();
            orPred.TryAdd(sp1);
            orPred.TryAdd(sp2);
            PolicyPredicate orPP = new OrPredicates(orPred);
            ConcurrentLinkedList<PolicyPredicate> andPred = new ConcurrentLinkedList<PolicyPredicate>();
            andPred.TryAdd(sp3);
            andPred.TryAdd(orPP);
            PolicyPredicate andPP = new AndPredicates(andPred);
            Assert.IsTrue(andPP.applyPrediacte(purchaseDetails));
        }

        [TestMethod]
        public void composedPredicateNotMetTest()
        {
            Predicate<PurchaseDetails> p1 = pd => pd.totalPurchasePrice() > 60;
            PolicyPredicate sp1 = new SimplePredicate(p1);
            Predicate<PurchaseDetails> p2 = pd => pd.atMostNQuantity(itemB.itemID, 5);
            PolicyPredicate sp2 = new SimplePredicate(p2);
            Predicate<PurchaseDetails> p3 = pd => pd.user.userAge >= 18;
            PolicyPredicate sp3 = new SimplePredicate(p3);
            ConcurrentLinkedList<PolicyPredicate> andPred = new ConcurrentLinkedList<PolicyPredicate>();
            andPred.TryAdd(sp1);
            andPred.TryAdd(sp2);
            PolicyPredicate andPP = new AndPredicates(andPred);
            ConcurrentLinkedList<PolicyPredicate> andPred2 = new ConcurrentLinkedList<PolicyPredicate>();
            andPred2.TryAdd(sp3);
            andPred2.TryAdd(andPP);
            PolicyPredicate andPP2 = new AndPredicates(andPred2);
            Assert.IsFalse(andPP2.applyPrediacte(purchaseDetails));
        }
    }
}
