using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.ExternalPaymentSystem;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212_TESTS.IntegrationTests
{
    [TestClass]
    public class NotificationsTest
    {
        public static SystemController controller = SystemController.Instance;
        public static int itemID;
        public static int storeID;
        private static DeliveryParametersDTO deliveryParameters;
        private static PaymentParametersDTO paymentParameters;

        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            StoreRepository.Instance.stores.Clear();
        }
        
        public void createUserWithStore()
        {
            RegularResult result = controller.register("theuser", 18, "123456");
            controller.login("theuser", "123456");
            storeID = controller.openStore("theuser", "store", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, ItemReview>(), 2.4, (int)ItemCategory.Dairy);
            itemID = controller.addItemToStorage("theuser", storeID, item).getValue();
        }
        
        public void createUser()
        {
            RegularResult result = controller.register("theotheruser", 18, "123456");
            controller.login("theotheruser", "123456");
        }

        private void itemReviewInit()
        {
            createUserWithStore();
            createUser();
            controller.addItemToShoppingCart("theotheruser", storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 2.4);
            initPurchaseParameters();
            controller.purchaseItems("theotheruser", deliveryParameters, paymentParameters);
        }
        
        private void initPurchaseParameters()
        {
            deliveryParameters = new DeliveryParametersDTO("theotheruser", "habanim", "Haifa", "Israel", "786598");
            paymentParameters = new PaymentParametersDTO("68957221011", "1", "2021", "theotheruser", "086", "207885623");
        }

        [TestMethod]
        public void itemReviewSuccessful()
        {
            itemReviewInit();
            var res = controller.itemReview("theotheruser", "wow", itemID, storeID);
            Assert.IsTrue(res.getTag());
            Assert.AreEqual("theuser", res.getValue().usersToSend.First.Value);
        }
        
        [TestMethod]
        public void itemReviewNoSuchItem()
        {
            createUser();
            var res = controller.itemReview("theotheruser", "wow", -1, storeID);
            Assert.IsFalse(res.getTag());
            Assert.IsNull(res.getValue());
        }

        private void purchaseItemsInit()
        {
            createUserWithStore();
            createUser();
            controller.addItemToShoppingCart("theotheruser", storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 2.4);
            initPurchaseParameters();
        }

        [TestMethod]
        public void purchaseItemsSuccessful()
        {
            purchaseItemsInit();
            var res = controller.purchaseItems("theotheruser", deliveryParameters, paymentParameters);
            Assert.IsTrue(res.getTag());
            Assert.AreEqual("theuser", res.getValue().usersToSend.First.Value);
        }

        [TestMethod]
        public void purchaseItemsNoSuchUser()
        {
            initPurchaseParameters();
            var res = controller.purchaseItems("theotheruser", deliveryParameters, paymentParameters);
            Assert.IsFalse(res.getTag());
            Assert.IsNull(res.getValue());
        }

        private void removeManagerInit()
        {
            createUserWithStore();
            createUser();
            controller.appointStoreManager("theuser", "theotheruser", storeID);
        }

        [TestMethod]
        public void removeManagerSuccessful()
        {
            removeManagerInit();
            var res = controller.removeStoreManager("theuser", "theotheruser", storeID);
            Assert.IsTrue(res.getTag());
        }
        
        [TestMethod]
        public void removeManagerNoSuchManager()
        {
            removeManagerInit();
            var res = controller.removeStoreManager("theuser", "noName", storeID);
            Assert.IsFalse(res.getTag());
            Assert.IsNull(res.getValue());
        }
        
        private void removeOwnerInit()
        {
            createUserWithStore();
            createUser();
            controller.appointStoreOwner("theuser", "theotheruser", storeID);
        }
        
        [TestMethod]
        public void removeOwnerNoSuchManager()
        {
            removeOwnerInit();
            var res = controller.removeStoreManager("theuser", "noName", storeID);
            Assert.IsFalse(res.getTag());
            Assert.IsNull(res.getValue());
        }
    }
}