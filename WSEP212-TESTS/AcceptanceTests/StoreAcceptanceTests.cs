using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
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
            RegularResult result = controller.register("theuser", 18, "123456");
            controller.login("theuser", "123456");
            storeID = controller.openStore("theuser", "store", "somewhere", "DEFAULT", "DEFAULT").getValue();
        }

        public void testInit()
        {
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, ItemUserReviews>(), 2.4, "diary");
            itemID = controller.addItemToStorage("theuser", storeID, item).getValue();
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void purchaseItemsTest()
        {
            testInit();

            RegularResult result = controller.addItemToShoppingCart("theuser", storeID, itemID, 2);//adding an item
            Assert.IsTrue(result.getTag());

            ResultWithValue<NotificationDTO> result2 = controller.purchaseItems("a", "beer sheva"); //nothing in the cart
            Assert.IsFalse(result2.getTag());

            result2 = controller.purchaseItems("theuser", null); //wrong item id
            Assert.IsFalse(result2.getTag());

            result2 = controller.purchaseItems("theuser", "ashdod");
            Assert.IsTrue(result2.getTag());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void openStoreTest()
        {
            testInit();

            ResultWithValue<int> result = controller.openStore("theuser", "store2", "somewhere", "DEFAULT", "DEFAULT");
            Assert.IsFalse(result.getTag()); //store already exists

            result = controller.openStore(null, "store2", "somewhere", "DEFAULT", "DEFAULT");
            Assert.IsFalse(result.getTag()); //null user name

            result = controller.openStore("theuser", null, "somewhere", "DEFAULT", "DEFAULT");
            Assert.IsFalse(result.getTag()); //null store name

            result = controller.openStore("theuser", "store2", null, "DEFAULT", "DEFAULT");
            Assert.IsFalse(result.getTag()); //null address

            result = controller.openStore("theuser", "store2", "somewhere", null, "DEFAULT");
            Assert.IsFalse(result.getTag()); //null purchase policy

            result = controller.openStore("theuser", "store2", "somewhere", "DEFAULT", null);
            Assert.IsFalse(result.getTag()); //null sales policy

            result = controller.openStore("theuser", "HAMAMA", "Ashdod", "DEFAULT", "DEFAULT");
            Console.WriteLine(result.getMessage());
            Assert.IsTrue(result.getTag());
            
            controller.logout("theuser");
            
            ResultWithValue<int> res = controller.openStore("a", "gg", "kk", "default", "default"); //guest cant open
            Assert.IsFalse(true); //should throw exception - if its here, nothing was thrown
            
            controller.login("theuser", "123456");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void itemReviewTest()
        {
            testInit();

            RegularResult res = controller.addItemToShoppingCart("theuser", storeID, itemID, 2);
            Assert.IsTrue(res.getTag());
            ResultWithValue<NotificationDTO> res2 = controller.purchaseItems("theuser", "ashdod");
            Assert.IsTrue(res2.getTag());
            ResultWithValue<NotificationDTO> result = controller.itemReview("theuser", "wow", itemID, storeID); //logged
            Assert.IsTrue(result.getTag());

            result = controller.itemReview(null, "boo", itemID, storeID);
            Assert.IsFalse(result.getTag());

            result = controller.itemReview("theuser", null, itemID, storeID);
            Assert.IsFalse(result.getTag());
            
            controller.logout("theuser");

            controller.itemReview("moon", "boo", itemID, storeID); //guest user can't perform this action
            Assert.IsFalse(true);

            controller.login("theuser", "123456");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void addItemToStorageTest()
        {
            ItemDTO itemDto = new ItemDTO(storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, ItemUserReviews>(), 1.34, "snacks");

            ResultWithValue<int> res = controller.addItemToStorage("theuser", storeID, itemDto);
            Assert.IsTrue(res.getTag());

            res = controller.addItemToStorage("theuser", storeID, itemDto); //already in storage
            Assert.IsTrue(res.getTag());
            int bisliID = res.getValue();

            res = controller.addItemToStorage("theuser", storeID, null);
            Assert.IsFalse(res.getTag());

            controller.removeItemFromStorage("theuser", storeID, bisliID);
            
            controller.logout("theuser");

            controller.addItemToStorage("moon", storeID, itemDto);//guest user - can't perform this action
            Assert.IsFalse(true);

            controller.login("theuser", "123456");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void removeItemFromStorageTest()
        {
            testInit();

            RegularResult result = controller.removeItemFromStorage("theuser", storeID, itemID);
            Assert.IsTrue(result.getTag());

            result = controller.removeItemFromStorage("theuser", storeID, -1); //no such item
            Assert.IsFalse(result.getTag());

            result = controller.removeItemFromStorage("theuser", -1, itemID); //no such store
            Assert.IsFalse(result.getTag());

            result = controller.removeItemFromStorage("booo", storeID, itemID);//guest user - can't perform this action
            Assert.IsFalse(result.getTag());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void editItemDetailsTest()
        {

            ItemDTO itemDto = new ItemDTO(storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, ItemUserReviews>(), 1.34, "snacks");

            ResultWithValue<int> res = controller.addItemToStorage("theuser", storeID, itemDto);
            Assert.IsTrue(res.getTag());
            Assert.AreNotEqual(-1, res.getValue());
            itemDto.itemID = res.getValue();

            itemDto.price = 7.9;
            RegularResult result = controller.editItemDetails("theuser", storeID, itemDto);
            Assert.IsTrue(res.getTag());

            result = controller.editItemDetails("theuser", storeID, null);
            Assert.IsFalse(result.getTag());
            
            controller.logout("theuser");

            controller.editItemDetails("blue", storeID, itemDto); //guest user - can't perform this action
            Assert.IsFalse(true);

            controller.login("theuser", "123456");
        }
    }
}