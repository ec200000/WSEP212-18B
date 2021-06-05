using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication;
using WSEP212;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class StoreAcceptanceTests
    {
        public static SystemController controller = SystemController.Instance;
        public static int itemID;
        public static int storeID;

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
            RegularResult result = controller.register("theuser123", 18, "123456");
            controller.login("theuser123", "123456");
            storeID = controller.openStore("theuser123", "store", "somewhere", "DEFAULT", "DEFAULT").getValue();
            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
        }

        public void testInit()
        {
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, ItemReview>(), 2.4, (int)ItemCategory.Dairy);
            itemID = controller.addItemToStorage("theuser123", storeID, item).getValue();
        }

        [TestMethod]
        public void purchaseItemsTest()
        {
            testInit();

            DeliveryParametersDTO deliveryParameters = new DeliveryParametersDTO("theuser123", "habanim", "Haifa", "Israel", "786598");
            PaymentParametersDTO paymentParameters = new PaymentParametersDTO("68957221011", "1", "2021", "theuser123", "086", "207885623");

            ResultWithValue<NotificationDTO> result = controller.addItemToShoppingCart("theuser123", storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 2.4);//adding an item
            Assert.IsTrue(result.getTag());
            ResultWithValue<NotificationDTO> result2 = controller.purchaseItems("theuser123", deliveryParameters, paymentParameters);
            Assert.IsTrue(result2.getTag());

            controller.removeItemFromShoppingCart("theuser123", storeID, itemID);
        }

        [TestMethod]
        public void purchaseItemsEmptyCartTest()
        {
            testInit();

            DeliveryParametersDTO deliveryParameters = new DeliveryParametersDTO("a", "habanim", "Haifa", "Israel", "786598");
            PaymentParametersDTO paymentParameters = new PaymentParametersDTO("68957221011", "1", "2021", "a", "086", "207885623");

            ResultWithValue<NotificationDTO> result2 = controller.purchaseItems("a", deliveryParameters, paymentParameters); //nothing in the cart
            Assert.IsFalse(result2.getTag());
        }

        [TestMethod]
        public void openStoreTest()
        {
            testInit();

            ResultWithValue<int> result = controller.openStore("theuser123", "HAMAMAH", "Ashdod", "DEFAULT", "DEFAULT");
            Console.WriteLine(result.getMessage());
            Assert.IsTrue(result.getTag());
        }

        [TestMethod]
        public void openStoreTwiceTest()
        {
            testInit();

            ResultWithValue<int> result = controller.openStore("theuser123", "store", "somewhere", "DEFAULT", "DEFAULT");
            Assert.IsFalse(result.getTag()); //store already exists
        }

        [TestMethod]
        public void openStoreWithNullTest()
        {
            testInit();

            ResultWithValue<int> result = controller.openStore("theuser123", null, "somewhere", "DEFAULT", "DEFAULT");
            Assert.IsFalse(result.getTag()); //null store name
            result = controller.openStore("theuser123", "store2", null, "DEFAULT", "DEFAULT");
            Assert.IsFalse(result.getTag()); //null address
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void openStoreWithGuestTest()
        {
            testInit();

            ResultWithValue<int> res = controller.openStore("a", "gg", "kk", "default", "default"); //guest cant open
            Assert.IsFalse(true); //should throw exception - if its here, nothing was thrown
        }

        [TestMethod]
        public void itemReviewTest()
        {
            testInit();

            DeliveryParametersDTO deliveryParameters = new DeliveryParametersDTO("theuser123", "habanim", "Haifa", "Israel", "786598");
            PaymentParametersDTO paymentParameters = new PaymentParametersDTO("68957221011", "1", "2021", "theuser123", "086", "207885623");

            ResultWithValue<NotificationDTO> res = controller.addItemToShoppingCart("theuser123", storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 2.4);
            Assert.IsTrue(res.getTag());
            ResultWithValue<NotificationDTO> res2 = controller.purchaseItems("theuser123", deliveryParameters, paymentParameters);
            Assert.IsTrue(res2.getTag());
            ResultWithValue<NotificationDTO> result = controller.itemReview("theuser123", "wow", itemID, storeID); //logged
            Assert.IsTrue(result.getTag());

            controller.removeItemFromShoppingCart("theuser123", storeID, itemID);
        }

        [TestMethod]
        public void itemEmptyReviewTest()
        {
            testInit();

            ResultWithValue<NotificationDTO> res = controller.addItemToShoppingCart("theuser123", storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 2.4);
            Assert.IsTrue(res.getTag());
            ResultWithValue<NotificationDTO> result = controller.itemReview("theuser123", null, itemID, storeID);
            Assert.IsFalse(result.getTag());

            controller.removeItemFromShoppingCart("theuser123", storeID, itemID);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void itemReviewByGuestest()
        {
            testInit();

            controller.itemReview("moon", "boo", itemID, storeID); //guest user can't perform this action
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void addItemToStorageTest()
        {
            ItemDTO itemDto = new ItemDTO(storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, ItemReview>(), 1.34, (int)ItemCategory.Snacks);

            ResultWithValue<int> res = controller.addItemToStorage("theuser123", storeID, itemDto);
            Assert.IsTrue(res.getTag());
            res = controller.addItemToStorage("theuser123", storeID, itemDto); //already in storage
            Assert.IsTrue(res.getTag());

            int bisliID = res.getValue();
            controller.removeItemFromStorage("theuser123", storeID, bisliID);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void addItemToStorageByGuestTest()
        {
            ItemDTO itemDto = new ItemDTO(storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, ItemReview>(), 1.34, (int)ItemCategory.Snacks);

            controller.addItemToStorage("moon", storeID, itemDto);//guest user - can't perform this action
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void removeItemFromStorageTest()
        {
            testInit();

            RegularResult result = controller.removeItemFromStorage("theuser123", storeID, itemID);
            Assert.IsTrue(result.getTag());
        }

        [TestMethod]
        public void removeItemFromStorageNoSuchItemTest()
        {
            testInit();

            RegularResult result = controller.removeItemFromStorage("theuser123", storeID, -1); //no such item
            Assert.IsFalse(result.getTag());
        }

        [TestMethod]
        public void removeItemFromStorageNoSuchStoreTest()
        {
            testInit();

            RegularResult result = controller.removeItemFromStorage("theuser123", -1, itemID); //no such store
            Assert.IsFalse(result.getTag());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void removeItemFromStorageByGuestTest()
        {
            testInit();

            RegularResult result = controller.removeItemFromStorage("booo", storeID, itemID);//guest user - can't perform this action
            Assert.IsFalse(result.getTag());
        }

        [TestMethod]
        public void editItemDetailsTest()
        {
            ItemDTO itemDto = new ItemDTO(storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, ItemReview>(), 1.34, (int)ItemCategory.Snacks);

            ResultWithValue<int> res = controller.addItemToStorage("theuser123", storeID, itemDto);
            Assert.IsTrue(res.getTag());
            Assert.AreNotEqual(-1, res.getValue());
            itemDto.itemID = res.getValue();
            itemDto.price = 7.9;
            RegularResult result = controller.editItemDetails("theuser123", storeID, itemDto);
            Assert.IsTrue(res.getTag());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void editItemDetailsByGuestTest()
        {
            ItemDTO itemDto = new ItemDTO(storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, ItemReview>(), 1.34, (int)ItemCategory.Snacks);

            controller.editItemDetails("blue", storeID, itemDto); //guest user - can't perform this action
            Assert.IsFalse(true);
        }
    }
}