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
            
            SystemDBMock.Instance.Bids.RemoveRange(SystemDBMock.Instance.Bids);
            SystemDBMock.Instance.Carts.RemoveRange(SystemDBMock.Instance.Carts);
            SystemDBMock.Instance.Invoices.RemoveRange(SystemDBMock.Instance.Invoices);
            SystemDBMock.Instance.Items.RemoveRange(SystemDBMock.Instance.Items);
            SystemDBMock.Instance.Permissions.RemoveRange(SystemDBMock.Instance.Permissions);
            SystemDBMock.Instance.Stores.RemoveRange(SystemDBMock.Instance.Stores);
            SystemDBMock.Instance.Users.RemoveRange(SystemDBMock.Instance.Users);
            SystemDBMock.Instance.DelayedNotifications.RemoveRange(SystemDBMock.Instance.DelayedNotifications);
            SystemDBMock.Instance.ItemReviewes.RemoveRange(SystemDBMock.Instance.ItemReviewes);
            SystemDBMock.Instance.UsersInfo.RemoveRange(SystemDBMock.Instance.UsersInfo);

            userName = "Sagiv";
            controller.register(userName, 21, "123456");
            controller.login(userName, "123456");
            storeID = controller.openStore(userName, "SagivStore", "somewhere", "DEFAULT", "DEFAULT").getValue();
            controller.supportPurchaseType(userName, storeID, submitOfferPurchaseType);
            ItemDTO itemA = new ItemDTO(storeID, 10, "yammy", "wow", new ConcurrentDictionary<string, WSEP212.DomainLayer.ItemReview>(), 5.0, (int)WSEP212.DomainLayer.ItemCategory.Dairy);
            itemIDA = controller.addItemToStorage(userName, storeID, itemA).getValue();
            ItemDTO itemB = new ItemDTO(storeID, 10, "tasty", "wow", new ConcurrentDictionary<string, WSEP212.DomainLayer.ItemReview>(), 7.5, (int)WSEP212.DomainLayer.ItemCategory.Dairy);
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
            controller.addItemToShoppingCart("guest", storeID, itemIDA, 1, submitOfferPurchaseType, 4.5);
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
            controller.addItemToShoppingCart("guest", storeID, itemIDA, 1, submitOfferPurchaseType, 2.0);
            controller.itemCounterOffer(userName, "guest", storeID, itemIDA, 4.5);
            ResultWithValue<NotificationDTO> res = controller.purchaseItems("guest", deliveryParameters, paymentParameters);
            Assert.IsTrue(res.getTag());
        }
    }
}
