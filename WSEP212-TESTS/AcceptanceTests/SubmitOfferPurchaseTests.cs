using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class SubmitOfferPurchaseTests
    {
        private static SystemController controller = SystemController.Instance;
        private static String userName;
        private static int storeID;
        private static int itemIDA;
        private static int itemIDB;
        private static DeliveryParametersDTO deliveryParameters;
        private static PaymentParametersDTO paymentParameters;

        private static int submitOfferPurchaseType = 1;
        private int approvedType = 0;
        private int rejectedType = 2;
        
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

            userName = "Din";
            controller.register(userName, 21, "123456");
            controller.login(userName, "123456");
            ResultWithValue<int> storeIDRes = controller.openStore(userName, "DinStore", "somewhere", "DEFAULT", "DEFAULT");
            storeID = storeIDRes.getValue();
            controller.supportPurchaseType(userName, storeID, submitOfferPurchaseType);
            ItemDTO itemA = new ItemDTO(storeID, 100, "yammy", "wow", new ConcurrentDictionary<string, WSEP212.DomainLayer.ItemReview>(), 5.0, (int)WSEP212.DomainLayer.ItemCategory.Dairy);
            itemIDA = controller.addItemToStorage(userName, storeID, itemA).getValue();
            ItemDTO itemB = new ItemDTO(storeID, 100, "tasty", "wow", new ConcurrentDictionary<string, WSEP212.DomainLayer.ItemReview>(), 7.5, (int)WSEP212.DomainLayer.ItemCategory.Dairy);
            itemIDB = controller.addItemToStorage(userName, storeID, itemB).getValue();

            deliveryParameters = new DeliveryParametersDTO("guest", "habanim", "Haifa", "Israel", "786598");
            paymentParameters = new PaymentParametersDTO("68957221011", "1", "2021", "guest", "086", "207885623");
        }

        [TestCleanup]
        public void cleanUp()
        {
            controller.removeItemFromShoppingCart("guest", storeID, itemIDA);
            controller.removeItemFromShoppingCart("guest", storeID, itemIDB);
        }

        [TestMethod]
        public void simpleSubmitOfferPurchaseTest()
        {
            ResultWithValue<NotificationDTO> res1 = controller.addItemToShoppingCart("guest", storeID, itemIDA, 1, submitOfferPurchaseType, 4.5);
            controller.confirmPriceStatus(userName, "guest", storeID, itemIDA, approvedType);
            ResultWithValue<NotificationDTO> res = controller.purchaseItems("guest", deliveryParameters, paymentParameters);
            Assert.IsTrue(res.getTag());
        }

        [TestMethod]
        public void rejectSubmitOfferPurchaseTest()
        {
            controller.addItemToShoppingCart("guest", storeID, itemIDA, 1, submitOfferPurchaseType, 1.0);
            controller.confirmPriceStatus(userName, "guest", storeID, itemIDA, rejectedType);
            ResultWithValue<NotificationDTO> res = controller.purchaseItems("guest", deliveryParameters, paymentParameters);
            Assert.IsFalse(res.getTag());

            controller.removeItemFromShoppingCart("guest", storeID, itemIDA);
        }

        [TestMethod]
        public void complexSubmitOfferPurchaseTest()
        {
            controller.addItemToShoppingCart("guest", storeID, itemIDA, 1, submitOfferPurchaseType, 4.5);
            controller.addItemToShoppingCart("guest", storeID, itemIDB, 1, submitOfferPurchaseType, 6.0);

            controller.confirmPriceStatus(userName, "guest", storeID, itemIDA, approvedType);
            controller.confirmPriceStatus(userName, "guest", storeID, itemIDB, approvedType);

            ResultWithValue<NotificationDTO> res = controller.purchaseItems("guest", deliveryParameters, paymentParameters);
            Assert.IsTrue(res.getTag());
        }

        [TestMethod]
        public void counterOfferPurchaseTest()
        {
            ResultWithValue<NotificationDTO> r = controller.addItemToShoppingCart("guest", storeID, itemIDA, 1, submitOfferPurchaseType, 2.0);
            Console.WriteLine(r.getMessage());
            controller.itemCounterOffer(userName, "guest", storeID, itemIDA, 4.5);
            ResultWithValue<NotificationDTO> res = controller.purchaseItems("guest", deliveryParameters, paymentParameters);
            Assert.IsTrue(res.getTag());
        }
    }
}
