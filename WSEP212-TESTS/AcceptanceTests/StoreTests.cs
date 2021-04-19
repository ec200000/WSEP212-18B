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

        [TestInitialize]
        public void testInitial()
        {
            RegularResult result = controller.register("theuser", "123456");
        }
        
        public void testInit()
        {
            controller.login("theuser", "123456");
            storeID = controller.openStore("theuser", "store", "somewhere", "DEFAULT", "DEFAULT").getValue();
            Console.WriteLine(storeID);
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, string>(), 2.4, "diary");
            itemID = controller.addItemToStorage("theuser", storeID, item).getValue();
        }

        public void testInit2()
        {
            controller.login("theuser", "123456");
            storeID = controller.openStore("theuser", "store2", "somewhere", "DEFAULT", "DEFAULT").getValue();
            Console.WriteLine(storeID);
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, string>(), 2.4, "diary");
            itemID = controller.addItemToStorage("theuser", storeID, item).getValue();
        }

        public void testInit3()
        {
            controller.login("theuser", "123456");
            storeID = controller.openStore("theuser", "store3", "somewhere", "DEFAULT", "DEFAULT").getValue();
            Console.WriteLine(storeID);
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, string>(), 2.4, "diary");
            itemID = controller.addItemToStorage("theuser", storeID, item).getValue();
            RegularResult result = controller.addItemToShoppingCart("theuser", storeID, itemID, 2); //logged user
        }

        public void testInit4()
        {
            controller.login("theuser", "123456");
            ResultWithValue<int> res2 = controller.openStore("theuser", "store4", "somewhere", "DEFAULT", "DEFAULT");
            storeID = res2.getValue();
            Console.WriteLine(res2.getTag());
        }

        public void testInit5()
        {
            controller.login("theuser", "123456");
            storeID = controller.openStore("theuser", "store5", "somewhere", "DEFAULT", "DEFAULT").getValue();
            Console.WriteLine(storeID);
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, string>(), 2.4, "diary");
            itemID = controller.addItemToStorage("theuser", storeID, item).getValue();
        }

        public void testInit6()
        {
            controller.login("theuser", "123456");
            storeID = controller.openStore("theuser", "store6", "somewhere", "DEFAULT", "DEFAULT").getValue();
            Console.WriteLine(storeID);
        }

        [TestMethod]
        public void purchaseItemsTest()
        {
            testInit();

            RegularResult result = controller.addItemToShoppingCart("theuser", storeID, itemID, 2);//adding an item
            Assert.IsTrue(result.getTag());

            result = controller.purchaseItems("a", "beer sheva"); //nothing in the cart
            Assert.IsFalse(result.getTag());

            result = controller.purchaseItems("theuser", null); //wrong item id
            Assert.IsFalse(result.getTag());

            result = controller.purchaseItems("theuser", "ashdod");
            Assert.IsTrue(result.getTag());

            controller.logout("theuser");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void openStoreTest()
        {
            testInit2();

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
            Assert.IsTrue(result.getTag());
            
            controller.logout("theuser");
            
            ResultWithValue<int> res = controller.openStore("a", "gg", "kk", "default", "default"); //guest cant open
            Assert.IsFalse(true); //should throw exception - if its here, nothing was thrown
            
            
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void itemReviewTest()
        {
            testInit3();

            RegularResult res = controller.addItemToShoppingCart("theuser", storeID, itemID, 2);
            Assert.IsTrue(res.getTag());
            res = controller.purchaseItems("theuser", "ashdod");
            Assert.IsTrue(res.getTag());
            RegularResult result = controller.itemReview("theuser", "wow", itemID, storeID); //logged
            Assert.IsTrue(result.getTag());

            result = controller.itemReview(null, "boo", itemID, storeID);
            Assert.IsFalse(result.getTag());

            result = controller.itemReview("theuser", null, itemID, storeID);
            Assert.IsFalse(result.getTag());
            
            controller.logout("theuser");

            controller.itemReview("moon", "boo", itemID, storeID); //guest user can't perform this action
            Assert.IsFalse(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void addItemToStorageTest()
        {
            testInit4();
            
            ItemDTO itemDto = new ItemDTO(storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, string>(), 1.34, "snacks");

            ResultWithValue<int> res = controller.addItemToStorage("theuser", storeID, itemDto);
            Assert.IsTrue(res.getTag());

            res = controller.addItemToStorage("theuser", storeID, itemDto); //already in storage
            Assert.IsTrue(res.getTag());

            res = controller.addItemToStorage("theuser", storeID, null);
            Assert.IsFalse(res.getTag());
            
            controller.logout("theuser");

            controller.addItemToStorage("moon", storeID, itemDto);//guest user - can't perform this action
            Assert.IsFalse(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void removeItemFromStorageTest()
        {
            testInit5();

            RegularResult result = controller.removeItemFromStorage("theuser", storeID, itemID);
            Assert.IsTrue(result.getTag());

            result = controller.removeItemFromStorage("theuser", storeID, -1); //no such item
            Assert.IsFalse(result.getTag());

            result = controller.removeItemFromStorage("theuser", -1, itemID); //no such store
            Assert.IsFalse(result.getTag());

            result = controller.removeItemFromStorage("booo", storeID, itemID);//guest user - can't perform this action
            Assert.IsFalse(result.getTag());
            
            controller.logout("theuser");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void editItemDetailsTest()
        {
            testInit6();

            ItemDTO itemDto = new ItemDTO(storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, string>(), 1.34, "snacks");

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
        }
    }
}