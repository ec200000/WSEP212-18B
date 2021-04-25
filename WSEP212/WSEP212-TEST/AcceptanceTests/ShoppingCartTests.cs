using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class ShoppingCartTests
    {
        SystemController controller = SystemController.Instance; 
        int itemID;
        int storeID;

        public void testInit()
        {
            controller.register("a", 18, "123");
            controller.register("b", 18, "123456");
            RegularResult result = controller.login("b", "123456");
            storeID = controller.openStore("b", "store1", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, string>(), 2.4, "diary");
            itemID = controller.addItemToStorage("b", storeID, item).getValue();
        }
        
        public void testInit2()
        {
            controller.register("a", 18, "123");
            controller.register("b", 18, "123456");
            //RegularResult result1 = controller.login("b", "123456");
            storeID = controller.openStore("b", "store2", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, string>(), 2.4, "diary");
            itemID = controller.addItemToStorage("b", storeID, item).getValue();
            RegularResult result = controller.addItemToShoppingCart("b", storeID, itemID, 2); //logged user
            result = controller.addItemToShoppingCart("a", storeID, itemID, 8); //guest user
            Assert.IsTrue(result.getTag());
        }

        [TestMethod]
        public void addItemToShoppingCartTest()
        {
            testInit();
            
            RegularResult result = controller.addItemToShoppingCart("b", storeID, itemID, 2); //logged user
            Assert.IsTrue(result.getTag());

            result = controller.addItemToShoppingCart("a", storeID, itemID, 8); //guest user
            Assert.IsTrue(result.getTag());

            result = controller.addItemToShoppingCart("b", storeID, itemID, 100); //over quantity
            Assert.IsFalse(result.getTag());
            
            result = controller.addItemToShoppingCart("b", storeID, -1, 1); //item does not exists
            Assert.IsFalse(result.getTag());
            
            result = controller.addItemToShoppingCart("b",-1, itemID, 1); //store doest not exists
            Assert.IsFalse(result.getTag());
            
        }
        
        [TestMethod]
        public void removeItemFromShoppingCartTest()
        {
            testInit2();
            
            RegularResult result = controller.removeItemFromShoppingCart("b",-1, itemID); //wrong store id
            Assert.IsFalse(result.getTag());
            
            result = controller.removeItemFromShoppingCart("b",storeID, -1); //wrong item id
            Assert.IsFalse(result.getTag());
            
            result = controller.removeItemFromShoppingCart("b",storeID, itemID); //removing it
            Assert.IsTrue(result.getTag());
            
            result = controller.removeItemFromShoppingCart("a",storeID, itemID); //removing it
            Assert.IsTrue(result.getTag());
            
            result = controller.removeItemFromShoppingCart("a",storeID, itemID); //nothing in the cart
            Assert.IsFalse(result.getTag());
        }
    }
}