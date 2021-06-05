using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

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

            UserRepository.Instance.initRepo();
            User user = new User("Sagiv", 21);
            UserRepository.Instance.insertNewUser(user, "123456");
            
            ResultWithValue<int> addStoreRes = StoreRepository.Instance.addStore("Mega", "Holon", new SalePolicyMock(), new PurchasePolicyMock(), user);
            Store store = StoreRepository.Instance.getStore(addStoreRes.getValue()).getValue();
            ResultWithValue<int> addItemRes = store.addItemToStorage(100, "bamba", "snack for childrens", 4.5, ItemCategory.Snacks);
            itemA = store.getItemById(addItemRes.getValue()).getValue();
            addItemRes = store.addItemToStorage(500, "milk", "pasteurized milk", 10, ItemCategory.Dairy);
            itemB = store.getItemById(addItemRes.getValue()).getValue();
            addItemRes = store.addItemToStorage(100, "bisli", "snack for childrens", 4, ItemCategory.Snacks);
            itemC = store.getItemById(addItemRes.getValue()).getValue();
            ConcurrentDictionary<Item, int> shoppingBagItems = new ConcurrentDictionary<Item, int>();
            shoppingBagItems.TryAdd(itemA, 3);
            shoppingBagItems.TryAdd(itemB, 2);
            shoppingBagItems.TryAdd(itemC, 5);
            ConcurrentDictionary<int, double> itemsPrices = new ConcurrentDictionary<int, double>();
            itemsPrices.TryAdd(itemA.itemID, 4.5);
            itemsPrices.TryAdd(itemB.itemID, 8);
            itemsPrices.TryAdd(itemC.itemID, 4);
            purchaseDetails = new PurchaseDetails(user, shoppingBagItems, itemsPrices);
        }

        [ClassCleanup]
        public static void cleanUp()
        {
            User user = UserRepository.Instance.findUserByUserName("Sagiv").getValue();
            UserRepository.Instance.removeUser(user);
        }

        [TestMethod]
        public void simplePredicateMetTest()
        {
            LocalPredicate<PurchaseDetails> predicate = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 40);
            SimplePredicate policyPredicate = new SimplePredicate(predicate, "price is more then 40");
            Assert.IsTrue(policyPredicate.applyPrediacte(purchaseDetails));

            predicate = new LocalPredicate<PurchaseDetails>(pd => pd.numOfSpecificItem(itemC.itemID), 5);
            policyPredicate = new SimplePredicate(predicate, "more then 5 bisli in bag");
            Assert.IsTrue(policyPredicate.applyPrediacte(purchaseDetails));
        }

        [TestMethod]
        public void simplePredicateNotMetTest()
        {
            LocalPredicate<PurchaseDetails> predicate = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 60);
            SimplePredicate policyPredicate = new SimplePredicate(predicate, "price is more then 40");
            Assert.IsFalse(policyPredicate.applyPrediacte(purchaseDetails));

            predicate = new LocalPredicate<PurchaseDetails>(pd => pd.numOfSpecificItem(itemA.itemID), 5);
            policyPredicate = new SimplePredicate(predicate, "more then 5 bamba in bag");
            Assert.IsFalse(policyPredicate.applyPrediacte(purchaseDetails));
        }

        [TestMethod]
        public void conditioningPredicateMetTest()
        {
            // only if total price > 40 then you can buy 5 or more milk
            LocalPredicate<PurchaseDetails> predicate = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 40);
            PurchasePredicate onlyif = new SimplePredicate(predicate, "price is more then 40");
            LocalPredicate<PurchaseDetails> then = new LocalPredicate<PurchaseDetails>(pd => pd.numOfSpecificItem(itemC.itemID), 5);
            PurchasePredicate onlythen = new SimplePredicate(then, "more then 5 bisli in bag");
            ConditioningPredicate policyPredicate = new ConditioningPredicate(onlyif, onlythen);
            Assert.IsTrue(policyPredicate.applyPrediacte(purchaseDetails));
        }

        [TestMethod]
        public void conditioningPredicateNotMetTest()
        {
            // only if total price > 60 then you can buy 5 or more milk
            LocalPredicate<PurchaseDetails> predicate = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 60);
            PurchasePredicate onlyif = new SimplePredicate(predicate, "price is more then 60");
            LocalPredicate<PurchaseDetails> then = new LocalPredicate<PurchaseDetails>(pd => pd.numOfSpecificItem(itemC.itemID), 5);
            PurchasePredicate onlythen = new SimplePredicate(then, "more then 5 bisli in bag");
            ConditioningPredicate policyPredicate = new ConditioningPredicate(onlyif, onlythen);
            Assert.IsFalse(policyPredicate.applyPrediacte(purchaseDetails));
        }

        [TestMethod]
        public void composedPredicateMetTest()
        {
            LocalPredicate<PurchaseDetails> p1 = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 40);
            PurchasePredicate sp1 = new SimplePredicate(p1, "price is more then 40");
            LocalPredicate<PurchaseDetails> p2 = new LocalPredicate<PurchaseDetails>(pd => pd.numOfSpecificItem(itemB.itemID), 5);
            PurchasePredicate sp2 = new SimplePredicate(p2, "more then 5 milk in bag");
            LocalPredicate<PurchaseDetails> p3 = new LocalPredicate<PurchaseDetails>(pd => pd.user.userAge, 18);
            PurchasePredicate sp3 = new SimplePredicate(p3, "user age is bigger then 18");
            PurchasePredicate orPP = new OrPredicates(sp1, sp2);
            PurchasePredicate andPP = new AndPredicates(sp3, orPP);
            Assert.IsTrue(andPP.applyPrediacte(purchaseDetails));
        }

        [TestMethod]
        public void composedPredicateNotMetTest()
        {
            LocalPredicate<PurchaseDetails> p1 = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 40);
            PurchasePredicate sp1 = new SimplePredicate(p1, "price is more then 40");
            LocalPredicate<PurchaseDetails> p2 = new LocalPredicate<PurchaseDetails>(pd => pd.numOfSpecificItem(itemB.itemID), 5);
            PurchasePredicate sp2 = new SimplePredicate(p2, "more then 5 milk in bag");
            LocalPredicate<PurchaseDetails> p3 = new LocalPredicate<PurchaseDetails>(pd => pd.user.userAge, 18);
            PurchasePredicate sp3 = new SimplePredicate(p3, "user age is bigger then 18");
            PurchasePredicate andPP = new AndPredicates(sp1, sp2);
            PurchasePredicate andPP2 = new AndPredicates(sp3, andPP);
            Assert.IsFalse(andPP2.applyPrediacte(purchaseDetails));
        }
    }
}
