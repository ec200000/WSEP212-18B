using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.DomainLayer;
using System.Collections.Concurrent;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212_TESTS.AcceptanceTests
{
    public class StorePoliciesAcceptanceTests
    { 
        [TestClass]
    public class StorePoliciesManagementTests
    {
        public static SystemController controller = SystemController.Instance;
        public static int milkID;
        public static int storeID;
        
        [TestInitialize]
        public void SetupAuth()
        {
            RegularResult result = controller.register("theuser", 18, "123456");
            controller.login("theuser", "123456");
            storeID = controller.openStore("theuser", "store", "somewhere", "DEFAULT", "DEFAULT").getValue();
            int itemID = controller.addItemToStorage("theuser", storeID, new ItemDTO(storeID, 500, "bamba", "snack for childrens", new ConcurrentDictionary<string, ItemUserReviews>(), 4.5, "snack")).getValue();
            controller.addItemToShoppingCart("theuser", storeID, itemID, 2, 0, 4.5);
            milkID = controller.addItemToStorage("theuser", storeID, new ItemDTO(storeID, 500, "milk", "pasteurized milk", new ConcurrentDictionary<string, ItemUserReviews>(), 8, "milk products")).getValue();
            controller.addItemToShoppingCart("theuser", storeID, milkID, 2, 0, 8);
            itemID = controller.addItemToStorage("theuser", storeID, new ItemDTO(storeID, 500, "bisli", "snack for childrens", new ConcurrentDictionary<string, ItemUserReviews>(), 4, "snack")).getValue();
            controller.addItemToShoppingCart("theuser", storeID, itemID, 5, 0, 4);
        }

        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            StoreRepository.Instance.stores.Clear();
        }    
        
        [TestMethod]
        public void noPolicyTest()
        {
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", "Holon");
            Assert.IsTrue(purchaseRes.getTag());
        }

        [TestMethod]
        public void approvedByPurchasePolicySimpleTest()
        {
            controller.addPurchasePredicate("theuser", storeID, pd => pd.numOfItemsInPurchase() >= 2,
                "more than 2 items in bag");
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", "Holon");
            Assert.IsTrue(purchaseRes.getTag());
        }

        [TestMethod]
        public void approvedByPurchasePolicyComplexTest()
        {
            int predID1 = controller.addPurchasePredicate("theuser", storeID, pd => pd.numOfItemsInPurchase() >= 2,
                "more than 2 items in bag").getValue();
            int predID2 = controller.addPurchasePredicate("theuser", storeID, pd => pd.totalPurchasePrice() >= 100,
                "price is more then 100").getValue();
            int predID3 = controller.addPurchasePredicate("theuser", storeID, pd => pd.user.userAge >= 18,
                "user age is more then 18").getValue();
            int composedID = controller.composePurchasePredicates("theuser", storeID, predID2, predID3,
                (int)PurchasePredicateCompositionType.OrComposition).getValue();
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", "Holon");
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, predID1);
            controller.removePurchasePredicate("theuser", storeID, composedID);
        }

        [TestMethod]
        public void rejectedByPurchasePolicySimpleTest()
        {
            int predicateID = controller.addPurchasePredicate("theuser", storeID, pd => pd.numOfItemsInPurchase() >= 20,
                "more than 20 items in bag").getValue();
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", "Holon");
            Assert.IsFalse(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, predicateID);
        }

        [TestMethod]
        public void rejectedByPurchasePolicyComplexTest()
        {
            int predID1 = controller.addPurchasePredicate("theuser", storeID, pd => pd.numOfItemsInPurchase() >= 2,
                "more than 2 items in bag").getValue();
            int predID2 = controller.addPurchasePredicate("theuser", storeID, pd => pd.totalPurchasePrice() >= 100,
                "price is more then 100").getValue();
            int predID3 = controller.addPurchasePredicate("theuser", storeID, pd => pd.user.userAge >= 18,
                "user age is more then 18").getValue();
            int composedID = controller.composePurchasePredicates("theuser", storeID, predID2, predID3,
                (int)PurchasePredicateCompositionType.ConditionalComposition).getValue();
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", "Holon");
            Assert.IsFalse(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, predID1);
            controller.removePurchasePredicate("theuser", storeID, composedID);
        }

        [TestMethod]
        public void appliedSaleSimpleTest()
        {
            int saleID = controller.addSale("theuser", storeID, 50, new SaleOnCategory("snack"), "50% sale on snacks").getValue();
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", "Holon");
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, saleID);
        }

        [TestMethod]
        public void appliedSaleComplexTest()
        {
            int saleID1 = controller.addSale("theuser", storeID, 50, new SaleOnCategory("snack"), "50% sale on snacks").getValue();
            int saleID2 = controller.addSale("theuser", storeID, 50, new SaleOnItem(milkID), "50% sale on milk").getValue();
            int saleID3 = controller.addSale("theuser", storeID, 50, new SaleOnAllStore(), "25% sale all store").getValue();
            int composedID = controller.composeSales("theuser", storeID, saleID1, saleID3,
                (int) SaleCompositionType.MaxComposition, null).getValue();
            controller.composeSales("theuser", storeID, saleID2, composedID, (int) SaleCompositionType.MaxComposition,
                null);
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", "Holon");
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, composedID);
            
        }

        [TestMethod]
        public void appliedConditionalSaleTest()
        {
            int saleID = controller.addSale("theuser", storeID, 50, new SaleOnCategory("snack"), "50% sale on snacks").getValue();
            controller.addSaleCondition("theuser", storeID, saleID, new SimplePredicate(pd => pd.totalPurchasePrice() > 40, 
                "total price is more than 40"), (int)SalePredicateCompositionType.AndComposition);
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", "Holon");
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, saleID);
        }

        [TestMethod]
        public void notAppliedConditionalSaleTest()
        {
            int saleID = controller.addSale("theuser", storeID, 50, new SaleOnCategory("snack"), "50% sale on snacks").getValue();
            controller.addSaleCondition("theuser", storeID, saleID, new SimplePredicate(pd => pd.totalPurchasePrice() > 50, 
                "total price is more than 50"), (int)SalePredicateCompositionType.AndComposition);
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", "Holon");
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, saleID);
        }
    }
    }
}