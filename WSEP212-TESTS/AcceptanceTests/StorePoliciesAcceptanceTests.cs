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
        public static PaymentParametersDTO paymentParameters;
        public static DeliveryParametersDTO deliveryParameters;
        
        [ClassInitialize]
        public void init()
        {
            SystemDBAccess.mock = true;
        }
        
        [TestInitialize]
        public void SetupAuth()
        {
            RegularResult result = controller.register("theuser", 18, "123456");
            controller.login("theuser", "123456");
            storeID = controller.openStore("theuser", "store", "somewhere", "DEFAULT", "DEFAULT").getValue();
            int itemID = controller.addItemToStorage("theuser", storeID, new ItemDTO(storeID, 500, "bamba", "snack for childrens", new ConcurrentDictionary<string, ItemReview>(), 4.5, (int)ItemCategory.Snacks)).getValue();
            controller.addItemToShoppingCart("theuser", storeID, itemID, 2, 0, 4.5);
            milkID = controller.addItemToStorage("theuser", storeID, new ItemDTO(storeID, 500, "milk", "pasteurized milk", new ConcurrentDictionary<string, ItemReview>(), 8, (int)ItemCategory.Dairy)).getValue();
            controller.addItemToShoppingCart("theuser", storeID, milkID, 2, 0, 8);
            itemID = controller.addItemToStorage("theuser", storeID, new ItemDTO(storeID, 500, "bisli", "snack for childrens", new ConcurrentDictionary<string, ItemReview>(), 4, (int)ItemCategory.Snacks)).getValue();
            controller.addItemToShoppingCart("theuser", storeID, itemID, 5, 0, 4);
            deliveryParameters = new DeliveryParametersDTO("theuser", "Rebinovich", "Holon", "Israel", "521036");
            paymentParameters = new PaymentParametersDTO("5042005811", "4", "2024", "theuser", "023", "025845318");
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
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
        }

        [TestMethod]
        public void approvedByPurchasePolicySimpleTest()
        {
            LocalPredicate<PurchaseDetails> localPredicate = new LocalPredicate<PurchaseDetails>(pd => pd.numOfItemsInPurchase(), 2);
            controller.addPurchasePredicate("theuser", storeID, localPredicate,
                "more than 2 items in bag");
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
        }

        [TestMethod]
        public void approvedByPurchasePolicyComplexTest()
        {
            LocalPredicate<PurchaseDetails> itemsInBag = new LocalPredicate<PurchaseDetails>(pd => pd.numOfItemsInPurchase(), 2);
            int predID1 = controller.addPurchasePredicate("theuser", storeID, itemsInBag, "more than 2 items in bag").getValue();
            LocalPredicate<PurchaseDetails> totalBagPrice = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 100);
            int predID2 = controller.addPurchasePredicate("theuser", storeID, totalBagPrice, "price is more then 100").getValue();
            LocalPredicate<PurchaseDetails> userAge = new LocalPredicate<PurchaseDetails>(pd => pd.user.userAge, 18);
            int predID3 = controller.addPurchasePredicate("theuser", storeID, userAge, "user age is more then 18").getValue();
            int composedID = controller.composePurchasePredicates("theuser", storeID, predID2, predID3,
                (int)PurchasePredicateCompositionType.OrComposition).getValue();
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, predID1);
            controller.removePurchasePredicate("theuser", storeID, composedID);
        }

        [TestMethod]
        public void rejectedByPurchasePolicySimpleTest()
        {
            LocalPredicate<PurchaseDetails> itemsInBag = new LocalPredicate<PurchaseDetails>(pd => pd.numOfItemsInPurchase(), 20);
            int predicateID = controller.addPurchasePredicate("theuser", storeID, itemsInBag, "more than 20 items in bag").getValue();
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsFalse(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, predicateID);
        }

        [TestMethod]
        public void rejectedByPurchasePolicyComplexTest()
        {
            LocalPredicate<PurchaseDetails> itemsInBag = new LocalPredicate<PurchaseDetails>(pd => pd.numOfItemsInPurchase(), 2);
            int predID1 = controller.addPurchasePredicate("theuser", storeID, itemsInBag, "more than 2 items in bag").getValue();
            LocalPredicate<PurchaseDetails> totalBagPrice = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 100);
            int predID2 = controller.addPurchasePredicate("theuser", storeID, totalBagPrice, "price is more then 100").getValue();
            LocalPredicate<PurchaseDetails> userAge = new LocalPredicate<PurchaseDetails>(pd => pd.user.userAge, 18);
            int predID3 = controller.addPurchasePredicate("theuser", storeID, userAge, "user age is more then 18").getValue();
            int composedID = controller.composePurchasePredicates("theuser", storeID, predID2, predID3,
                (int)PurchasePredicateCompositionType.ConditionalComposition).getValue();
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsFalse(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, predID1);
            controller.removePurchasePredicate("theuser", storeID, composedID);
        }

        [TestMethod]
        public void appliedSaleSimpleTest()
        {
            int saleID = controller.addSale("theuser", storeID, 50, new SaleOnCategory(ItemCategory.Snacks), "50% sale on snacks").getValue();
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, saleID);
        }

        [TestMethod]
        public void appliedSaleComplexTest()
        {
            int saleID1 = controller.addSale("theuser", storeID, 50, new SaleOnCategory(ItemCategory.Snacks), "50% sale on snacks").getValue();
            int saleID2 = controller.addSale("theuser", storeID, 50, new SaleOnItem(milkID), "50% sale on milk").getValue();
            int saleID3 = controller.addSale("theuser", storeID, 50, new SaleOnAllStore(), "25% sale all store").getValue();
            int composedID = controller.composeSales("theuser", storeID, saleID1, saleID3,
                (int) SaleCompositionType.MaxComposition, null).getValue();
            controller.composeSales("theuser", storeID, saleID2, composedID, (int) SaleCompositionType.MaxComposition,
                null);
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, composedID);
            
        }

        [TestMethod]
        public void appliedConditionalSaleTest()
        {
            int saleID = controller.addSale("theuser", storeID, 50, new SaleOnCategory(ItemCategory.Snacks), "50% sale on snacks").getValue();
            LocalPredicate<PurchaseDetails> totalBagPrice = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 40);
            controller.addSaleCondition("theuser", storeID, saleID, new SimplePredicate(totalBagPrice, "total price is more than 40"), (int)SalePredicateCompositionType.AndComposition);
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, saleID);
        }

        [TestMethod]
        public void notAppliedConditionalSaleTest()
        {
            int saleID = controller.addSale("theuser", storeID, 50, new SaleOnCategory(ItemCategory.Snacks), "50% sale on snacks").getValue();
            LocalPredicate<PurchaseDetails> totalBagPrice = new LocalPredicate<PurchaseDetails>(pd => pd.totalPurchasePrice(), 50);
            controller.addSaleCondition("theuser", storeID, saleID, new SimplePredicate(totalBagPrice, "total price is more than 50"), (int)SalePredicateCompositionType.AndComposition);
            ResultWithValue<NotificationDTO> purchaseRes = controller.purchaseItems("theuser", deliveryParameters, paymentParameters);
            Assert.IsTrue(purchaseRes.getTag());
            controller.removePurchasePredicate("theuser", storeID, saleID);
        }
    }
    }
}