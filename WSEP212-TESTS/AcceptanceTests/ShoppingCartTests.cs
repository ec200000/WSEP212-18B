using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.DomainLayer;
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
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, ItemUserReviews>(), 2.4, "diary");
            itemID = controller.addItemToStorage("b", storeID, item).getValue();
        }
        
        public void testInitTwoCarts()
        {
            controller.register("a", 18, "123");
            controller.register("b", 18, "123456");
            //RegularResult result1 = controller.login("b", "123456");
            storeID = controller.openStore("b", "store2", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, ItemUserReviews>(), 2.4, "diary");
            itemID = controller.addItemToStorage("b", storeID, item).getValue();
            RegularResult result = controller.addItemToShoppingCart("b", storeID, itemID, 2); //logged user
            result = controller.addItemToShoppingCart("a", storeID, itemID, 8); //guest user
            Assert.IsTrue(result.getTag());
        }
        
        public void testInitStoreWithItem()
        {
            controller.register("ab", 18, "123");
            controller.register("bc", 18, "123456");
            controller.login("bc", "123456");
            storeID = controller.openStore("bc", "store3", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, ItemUserReviews>(), 2.4, "diary");
            itemID = controller.addItemToStorage("bc", storeID, item).getValue();
            HandlePurchases.Instance.paymentSystem = new BadPaymentSystemMock();
        }
        
        public void testInitBadDelivery()
        {
            controller.register("aa", 18, "123");
            controller.register("bb", 18, "123456");
            controller.login("bb", "123456");
            storeID = controller.openStore("bb", "store4", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, ItemUserReviews>(), 2.4, "diary");
            itemID = controller.addItemToStorage("bb", storeID, item).getValue();
            StoreRepository.Instance.stores[storeID].deliverySystem = new BadDeliverySystemMock();
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
            testInitTwoCarts();
            
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
        
        [TestMethod]
        public void purchaseItemsBadPaymentTest()
        {
            testInitStoreWithItem();
            HandlePurchases.Instance.paymentSystem = new BadPaymentSystemMock();

            RegularResult res;
            ResultWithValue<NotificationDTO> res1 = new OkWithValue<NotificationDTO>("ok",null);
            
            res = controller.addItemToShoppingCart("bc",storeID, itemID, 2);
            if (res.getTag())
                res1 = controller.purchaseItems("bc", "ashdod");

            Assert.IsFalse(res1.getTag()); // bad purchase mock
        }
        
        [TestMethod]
        public void purchaseItemsBadDeliveryTest()
        {
            testInitBadDelivery();
            
            RegularResult res;
            ResultWithValue<NotificationDTO> res1 = new OkWithValue<NotificationDTO>("ok",null);
            
            res = controller.addItemToShoppingCart("bb",storeID, itemID, 2);
            if (res.getTag())
                res1 = controller.purchaseItems("bb", "ashdod");

            Assert.IsFalse(res1.getTag()); // bad purchase mock
        }
    }
}