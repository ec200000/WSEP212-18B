using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.DomainLayer;
using System.Collections.Concurrent;
using WSEP212;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.ServiceObjectsDTO;
using System.Linq.Expressions;
using WebApplication;
using WSEP212.DataAccessLayer;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class StorePoliciesAcceptanceTests
    {
        public static SystemController controller = SystemController.Instance;
        public static int milkID;
        public static int bambaID;
        public static int storeID;
        public static PaymentParametersDTO paymentParameters;
        public static DeliveryParametersDTO deliveryParameters;

        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
            Startup.readConfigurationFile();
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

            controller.register("theuser", 18, "123456");
            controller.login("theuser", "123456");
            storeID = controller.openStore("theuser", "store", "somewhere", "DEFAULT", "DEFAULT").getValue();
            bambaID = controller.addItemToStorage("theuser", storeID,
                new ItemDTO(storeID, 500, "bamba", "snack for childrens",
                    new ConcurrentDictionary<string, ItemReview>(), 4.5, (int) ItemCategory.Snacks)).getValue();
            milkID = controller.addItemToStorage("theuser", storeID,
                new ItemDTO(storeID, 500, "milk", "pasteurized milk", new ConcurrentDictionary<string, ItemReview>(), 8,
                    (int) ItemCategory.Dairy)).getValue();
            deliveryParameters = new DeliveryParametersDTO("theuser", "Rebinovich", "Holon", "Israel", "521036");
            paymentParameters = new PaymentParametersDTO("5042005811", "4", "2024", "theuser", "023", "025845318");
        }

        private void addItemsForPurchase()
        {
            controller.addItemToShoppingCart("theuser", storeID, bambaID, 2, 0, 4.5);
            controller.addItemToShoppingCart("theuser", storeID, milkID, 2, 0, 8);
        }
        
        private void cleanShoppingCart()
        {
            controller.removeItemFromShoppingCart("theuser", storeID, bambaID);
            controller.removeItemFromShoppingCart("theuser", storeID, milkID);
        }

        [TestMethod]
        public void noPolicyTest()
        {
            addItemsForPurchase();
            ResultWithValue<NotificationDTO> purchaseRes =
                controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
        }

        [TestMethod]
        public void approvedByPurchasePolicySimpleTest()
        {
            addItemsForPurchase();
            LocalPredicate<PurchaseDetails> localPredicate =
                new LocalPredicate<PurchaseDetails>(pd => pd.numOfItemsInPurchase(), 2);
            controller.addPurchasePredicate("theuser", storeID, localPredicate,
                "more than 2 items in bag");
            ResultWithValue<NotificationDTO> purchaseRes =
                controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
        }

        [TestMethod]
        public void approvedByPurchasePolicyComplexTest()
        {
            addItemsForPurchase();
            LocalPredicate<PurchaseDetails> itemsInBag =
                new LocalPredicate<PurchaseDetails>(pd => pd.numOfItemsInPurchase(), 2);
            int predID1 = controller.addPurchasePredicate("theuser", storeID, itemsInBag, "more than 2 items in bag")
                .getValue();
            LocalPredicate<PurchaseDetails> totalBagPrice =
                new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 100);
            int predID2 = controller.addPurchasePredicate("theuser", storeID, totalBagPrice, "price is more then 100")
                .getValue();
            LocalPredicate<PurchaseDetails> userAge = new LocalPredicate<PurchaseDetails>(pd => pd.user.userAge, 18);
            int predID3 = controller.addPurchasePredicate("theuser", storeID, userAge, "user age is more then 18")
                .getValue();
            int composedID = controller.composePurchasePredicates("theuser", storeID, predID2, predID3,
                (int) PurchasePredicateCompositionType.OrComposition).getValue();
            ResultWithValue<NotificationDTO> purchaseRes =
                controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, predID1);
            controller.removePurchasePredicate("theuser", storeID, composedID);
        }

        [TestMethod]
        public void rejectedByPurchasePolicySimpleTest()
        {
            addItemsForPurchase();
            LocalPredicate<PurchaseDetails> itemsInBag =
                new LocalPredicate<PurchaseDetails>(pd => pd.numOfItemsInPurchase(), 20);
            int predicateID = controller
                .addPurchasePredicate("theuser", storeID, itemsInBag, "more than 20 items in bag").getValue();
            ResultWithValue<NotificationDTO> purchaseRes =
                controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsFalse(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, predicateID);
            cleanShoppingCart();
        }

        [TestMethod]
        public void rejectedByPurchasePolicyComplexTest()
        {
            addItemsForPurchase();
            LocalPredicate<PurchaseDetails> itemsInBag =
                new LocalPredicate<PurchaseDetails>(pd => pd.numOfItemsInPurchase(), 2);
            int predID1 = controller.addPurchasePredicate("theuser", storeID, itemsInBag, "more than 2 items in bag")
                .getValue();
            LocalPredicate<PurchaseDetails> totalBagPrice =
                new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 100);
            int predID2 = controller.addPurchasePredicate("theuser", storeID, totalBagPrice, "price is more then 100")
                .getValue();
            LocalPredicate<PurchaseDetails> userAge = new LocalPredicate<PurchaseDetails>(pd => pd.user.userAge, 18);
            int predID3 = controller.addPurchasePredicate("theuser", storeID, userAge, "user age is more then 18")
                .getValue();
            int composedID = controller.composePurchasePredicates("theuser", storeID, predID2, predID3,
                (int) PurchasePredicateCompositionType.ConditionalComposition).getValue();
            ResultWithValue<NotificationDTO> purchaseRes =
                controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsFalse(purchaseRes.getTag());
            Console.WriteLine(purchaseRes.getMessage());
            controller.removePurchasePredicate("theuser", storeID, predID1);
            controller.removePurchasePredicate("theuser", storeID, composedID);
            cleanShoppingCart();
        }

        [TestMethod]
        public void appliedSaleSimpleTest()
        {
            addItemsForPurchase();
            int saleID = controller.addSale("theuser", storeID, 50, new SaleOnCategory(ItemCategory.Snacks),
                "50% sale on snacks").getValue();
            ResultWithValue<NotificationDTO> purchaseRes =
                controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, saleID);
        }

        [TestMethod]
        public void appliedSaleComplexTest()
        {
            addItemsForPurchase();
            int saleID1 = controller.addSale("theuser", storeID, 50, new SaleOnCategory(ItemCategory.Snacks),
                "50% sale on snacks").getValue();
            int saleID2 = controller.addSale("theuser", storeID, 50, new SaleOnItem(milkID), "50% sale on milk")
                .getValue();
            int saleID3 = controller.addSale("theuser", storeID, 50, new SaleOnAllStore(), "25% sale all store")
                .getValue();
            int composedID = controller.composeSales("theuser", storeID, saleID1, saleID3,
                (int) SaleCompositionType.MaxComposition, null).getValue();
            controller.composeSales("theuser", storeID, saleID2, composedID, (int) SaleCompositionType.MaxComposition,
                null);
            ResultWithValue<NotificationDTO> purchaseRes =
                controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, composedID);
        }

        [TestMethod]
        public void appliedConditionalSaleTest()
        {
            addItemsForPurchase();
            int saleID = controller.addSale("theuser", storeID, 50, new SaleOnCategory(ItemCategory.Snacks),
                "50% sale on snacks").getValue();
            LocalPredicate<PurchaseDetails> totalBagPrice =
                new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 40);
            controller.addSaleCondition("theuser", storeID, saleID,
                new SimplePredicate(totalBagPrice, "total price is more than 40"),
                (int) SalePredicateCompositionType.AndComposition);
            ResultWithValue<NotificationDTO> purchaseRes =
                controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, saleID);
        }

        [TestMethod]
        public void notAppliedConditionalSaleTest()
        {
            addItemsForPurchase();
            int saleID = controller.addSale("theuser", storeID, 50, new SaleOnCategory(ItemCategory.Snacks),
                "50% sale on snacks").getValue();
            LocalPredicate<PurchaseDetails> totalBagPrice =
                new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 50);
            controller.addSaleCondition("theuser", storeID, saleID,
                new SimplePredicate(totalBagPrice, "total price is more than 50"),
                (int) SalePredicateCompositionType.AndComposition);
            ResultWithValue<NotificationDTO> purchaseRes =
                controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, saleID);
        }
    }
}