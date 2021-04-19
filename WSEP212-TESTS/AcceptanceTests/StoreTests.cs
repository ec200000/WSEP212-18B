using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class StoreTests
    {
        SystemController controller = new SystemController();
        int itemID;
        int storeID;

        public void testInit()
        {
            //controller.register("a", "123");
            RegularResult result = controller.register("user", "123456");
            Console.WriteLine(result.getMessage());
            controller.login("user", "123456");
            //Console.WriteLine(result.getMessage());
            storeID = controller.openStore("user", "store", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, string>(), 2.4, "diary");
            itemID = controller.addItemToStorage("user", storeID, item).getValue();
        }

        public void testInit2()
        {
            storeID = controller.openStore("user", "store2", "somewhere", "DEFAULT", "DEFAULT").getValue();
            Console.WriteLine(storeID);
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, string>(), 2.4, "diary");
            itemID = controller.addItemToStorage("user", storeID, item).getValue();
        }

        public void testInit3()
        {
            storeID = controller.openStore("user", "store3", "somewhere", "DEFAULT", "DEFAULT").getValue();
            Console.WriteLine(storeID);
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, string>(), 2.4, "diary");
            itemID = controller.addItemToStorage("user", storeID, item).getValue();
            RegularResult result = controller.addItemToShoppingCart("user", storeID, itemID, 2); //logged user
        }

        public void testInit4()
        {
            storeID = controller.openStore("user", "store4", "somewhere", "DEFAULT", "DEFAULT").getValue();
        }

        public void testInit5()
        {
            storeID = controller.openStore("user", "store5", "somewhere", "DEFAULT", "DEFAULT").getValue();
            Console.WriteLine(storeID);
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, string>(), 2.4, "diary");
            itemID = controller.addItemToStorage("user", storeID, item).getValue();
        }

        public void testInit6()
        {
            storeID = controller.openStore("user", "store6", "somewhere", "DEFAULT", "DEFAULT").getValue();
        }

        [TestMethod]
        public void purchaseItemsTest()
        {
            testInit();

            RegularResult result = controller.addItemToShoppingCart("user", storeID, itemID, 2);//adding an item
            Assert.IsTrue(result.getTag());

            result = controller.purchaseItems("a", "beer sheva"); //nothing in the cart
            Assert.IsFalse(result.getTag());

            result = controller.purchaseItems("user", null); //wrong item id
            Assert.IsFalse(result.getTag());

            result = controller.purchaseItems("user", "ashdod");
            Assert.IsTrue(result.getTag());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void openStoreTest()
        {
            testInit2();

            ResultWithValue<int> result = controller.openStore("user", "store2", "somewhere", "DEFAULT", "DEFAULT");
            Assert.IsFalse(result.getTag()); //store already exists

            result = controller.openStore(null, "store2", "somewhere", "DEFAULT", "DEFAULT");
            Assert.IsFalse(result.getTag()); //null user name

            result = controller.openStore("user", null, "somewhere", "DEFAULT", "DEFAULT");
            Assert.IsFalse(result.getTag()); //null store name

            result = controller.openStore("user", "store2", null, "DEFAULT", "DEFAULT");
            Assert.IsFalse(result.getTag()); //null address

            result = controller.openStore("user", "store2", "somewhere", null, "DEFAULT");
            Assert.IsFalse(result.getTag()); //null purchase policy

            result = controller.openStore("user", "store2", "somewhere", "DEFAULT", null);
            Assert.IsFalse(result.getTag()); //null sales policy

            result = controller.openStore("user", "HAMAMA", "Ashdod", "DEFAULT", "DEFAULT");
            Assert.IsTrue(result.getTag());

            ResultWithValue<int> res = controller.openStore("a", "gg", "kk", "default", "default"); //guest cant open
            Assert.IsFalse(true); //should throw exception - if its here, nothing was thrown
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void itemReviewTest()
        {
            testInit3();

            RegularResult res = controller.addItemToShoppingCart("user", storeID, itemID, 2);
            Assert.IsTrue(res.getTag());
            res = controller.purchaseItems("user", "ashdod");
            Assert.IsTrue(res.getTag());
            RegularResult result = controller.itemReview("user", "wow", itemID, storeID); //logged
            Assert.IsTrue(result.getTag());

            result = controller.itemReview(null, "boo", itemID, storeID);
            Assert.IsFalse(result.getTag());

            result = controller.itemReview("user", null, itemID, storeID);
            Assert.IsFalse(result.getTag());

            controller.itemReview("a", "boo", itemID, storeID); //guest user can't perform this action
            Assert.IsFalse(true);

        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void addItemToStorageTest()
        {
            testInit4();

            ResultWithValue<int> storeId = controller.openStore("user", "HAMAMA", "Beer Sheva", "default", "default");
            Assert.IsTrue(storeId.getTag());
            //Store store = StoreRepository.Instance.stores[storeId.getValue()];
            ItemDTO itemDto = new ItemDTO(storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, string>(), 1.34, "snacks");

            ResultWithValue<int> res = controller.addItemToStorage("user", storeID, itemDto);
            Assert.IsTrue(res.getTag());

            res = controller.addItemToStorage("user", storeID, itemDto); //already in storage
            Assert.IsTrue(res.getTag());

            res = controller.addItemToStorage("user", storeID, null);
            Assert.IsFalse(res.getTag());

            controller.addItemToStorage("a", storeID, itemDto);//guest user - can't perform this action

        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void removeItemFromStorageTest()
        {
            testInit5();

            RegularResult result = controller.removeItemFromStorage("user", storeID, itemID);
            Assert.IsTrue(result.getTag());

            result = controller.removeItemFromStorage("user", storeID, -1); //no such item
            Assert.IsFalse(result.getTag());

            result = controller.removeItemFromStorage("user", -1, itemID); //no such store
            Assert.IsFalse(result.getTag());

            result = controller.removeItemFromStorage("a", storeID, itemID);//guest user - can't perform this action
            Assert.IsFalse(result.getTag());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void editItemDetailsTest()
        {
            testInit6();

            ItemDTO itemDto = new ItemDTO(storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, string>(), 1.34, "snacks");

            ResultWithValue<int> res = controller.addItemToStorage("user", storeID, itemDto);
            Assert.IsTrue(res.getTag());
            Assert.AreNotEqual(-1, res.getValue());
            itemDto.itemID = res.getValue();

            itemDto.price = 7.9;
            RegularResult result = controller.editItemDetails("user", storeID, itemDto);
            Assert.IsTrue(res.getTag());

            result = controller.editItemDetails("user", storeID, null);
            Assert.IsFalse(result.getTag());

            controller.editItemDetails("a", storeID, itemDto); //guest user - can't perform this action
        }
    }
}