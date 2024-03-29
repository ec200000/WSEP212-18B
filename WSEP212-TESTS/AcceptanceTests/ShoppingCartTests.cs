﻿using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication;
using WSEP212;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.ExternalPaymentSystem;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
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

        [TestInitialize]
        public void SetupAuth()
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
        }
        
        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            foreach (Store store in StoreRepository.Instance.stores.Values)
            {
                store.storage.Clear();
            }
            StoreRepository.Instance.stores.Clear();
        }

        public void testInit()
        {
            controller.register("a", 18, "123");
            controller.register("b", 18, "123456");
            RegularResult result = controller.login("b", "123456");
            storeID = controller.openStore("b", "store1", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 100, "yammy", "wow", new ConcurrentDictionary<string, ItemReview>(), 2.4, (int)ItemCategory.Dairy);
            itemID = controller.addItemToStorage("b", storeID, item).getValue();
        }
        
        public void testInitTwoCarts()
        {
            controller.register("a", 18, "123");
            controller.register("b", 18, "123456");
            RegularResult result1 = controller.login("b", "123456");
            storeID = controller.openStore("b", "store2", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, ItemReview>(), 2.4, (int)ItemCategory.Dairy);
            itemID = controller.addItemToStorage("b", storeID, item).getValue();
            ResultWithValue<NotificationDTO> result = controller.addItemToShoppingCart("b", storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 2.4); //logged user
            result = controller.addItemToShoppingCart("a", storeID, itemID, 8, (int)PurchaseType.ImmediatePurchase, 2.4); //guest user
            Assert.IsTrue(result.getTag());
        }
        
        public void testInitStoreWithItem()
        {
            controller.register("ab", 18, "123");
            controller.register("bc", 18, "123456");
            controller.login("bc", "123456");
            storeID = controller.openStore("bc", "store3", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, ItemReview>(), 2.4, (int)ItemCategory.Dairy);
            itemID = controller.addItemToStorage("bc", storeID, item).getValue();
            HandlePurchases.Instance.paymentSystem = BadPaymentSystemMock.Instance;
        }
        
        public void testInitBadDelivery()
        {
            controller.register("aa", 18, "123");
            controller.register("bb", 18, "123456");
            controller.login("bb", "123456");
            storeID = controller.openStore("bb", "store4", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, ItemReview>(), 2.4, (int)ItemCategory.Dairy);
            itemID = controller.addItemToStorage("bb", storeID, item).getValue();
            StoreRepository.Instance.stores[storeID].deliverySystem = BadDeliverySystemMock.Instance;
        }

        [TestMethod]
        public void addItemToShoppingCartTest()
        {
            testInit();
            
            ResultWithValue<NotificationDTO> result = controller.addItemToShoppingCart("b", storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 2.4); //logged user
            Assert.IsTrue(result.getTag());
            result = controller.addItemToShoppingCart("a", storeID, itemID, 8, (int)PurchaseType.ImmediatePurchase, 2.4); //guest user
            Assert.IsTrue(result.getTag());
        }

        [TestMethod]
        public void addItemToShoppingCartOverQuantityTest()
        {
            testInit();

            ResultWithValue<NotificationDTO> result = controller.addItemToShoppingCart("b", storeID, itemID, 101, (int)PurchaseType.ImmediatePurchase, 2.4); //over quantity
            Assert.IsFalse(result.getTag());
        }

        [TestMethod]
        public void addItemNotExistToShoppingCartTest()
        {
            testInit();

            ResultWithValue<NotificationDTO> result = controller.addItemToShoppingCart("b", storeID, -1, 1, (int)PurchaseType.ImmediatePurchase, 2.4); //item does not exists
            Assert.IsFalse(result.getTag());
        }

        [TestMethod]
        public void addItemToShoppingCartStoreNotExistTest()
        {
            testInit();

            ResultWithValue<NotificationDTO> result = controller.addItemToShoppingCart("b", -1, itemID, 1, (int)PurchaseType.ImmediatePurchase, 2.4); //store doest not exists
            Assert.IsFalse(result.getTag());
        }

        [TestMethod]
        public void removeItemFromShoppingCartTest()
        {
            testInitTwoCarts();

            RegularResult result = controller.removeItemFromShoppingCart("b",storeID, itemID); //removing it
            Assert.IsTrue(result.getTag());
            result = controller.removeItemFromShoppingCart("a",storeID, itemID); //removing it
            Assert.IsTrue(result.getTag());
            result = controller.removeItemFromShoppingCart("a",storeID, itemID); //nothing left in the cart
            Assert.IsFalse(result.getTag());
        }

        [TestMethod]
        public void removeItemFromShoppingCartWrongStoreTest()
        {
            testInitTwoCarts();

            RegularResult result = controller.removeItemFromShoppingCart("b", -1, itemID); //wrong store id
            Assert.IsFalse(result.getTag());
        }

        [TestMethod]
        public void removeItemFromShoppingCartWrongItemTest()
        {
            testInitTwoCarts();

            RegularResult result = controller.removeItemFromShoppingCart("b", storeID, -1); //wrong item id
            Assert.IsFalse(result.getTag());
        }

        [TestMethod]
        public void purchaseItemsBadPaymentTest()
        {
            testInitStoreWithItem();
            HandlePurchases.Instance.paymentSystem = new BadPaymentSystemMock();

            DeliveryParametersDTO deliveryParameters = new DeliveryParametersDTO("bc", "habanim", "Haifa", "Israel", "786598");
            PaymentParametersDTO paymentParameters = new PaymentParametersDTO("68957221011", "1", "2021", "bc", "086", "207885623");

            ResultWithValue<NotificationDTO> res = controller.addItemToShoppingCart("bc",storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 2.4);
            Assert.IsTrue(res.getTag());
            ResultWithValue<NotificationDTO> res1 = controller.purchaseItems("bc", deliveryParameters, paymentParameters);

            Assert.IsFalse(res1.getTag()); // bad purchase mock
        }
        
        [TestMethod]
        public void purchaseItemsBadDeliveryTest()
        {
            testInitBadDelivery();

            DeliveryParametersDTO deliveryParameters = new DeliveryParametersDTO("bb", "habanim", "Haifa", "Israel", "786598");
            PaymentParametersDTO paymentParameters = new PaymentParametersDTO("68957221011", "1", "2021", "bb", "086", "207885623");

            ResultWithValue<NotificationDTO> res = controller.addItemToShoppingCart("bb", storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 2.4);
            Assert.IsTrue(res.getTag());
            ResultWithValue<NotificationDTO> res1 = controller.purchaseItems("bb", deliveryParameters, paymentParameters);

            Assert.IsFalse(res1.getTag()); 
        }
        
        [TestMethod]
        public void itemsQuantitiesTest()
        {
            testInit();
            
            controller.addItemToShoppingCart("a", storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 2.4);
            var quantitiesRes = controller.bagItemsQuantities("a", storeID);
            Assert.IsTrue(quantitiesRes.getTag());
            Assert.AreEqual(1, quantitiesRes.getValue().Count); // one item
            Assert.AreEqual(2, quantitiesRes.getValue()[itemID]); // quantity
        }
        
        [TestMethod]
        public void itemsPriceBeforeSaleTest()
        {
            testInit();
            
            controller.addItemToShoppingCart("a", storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 2.4);
            var priceRes = controller.getItemsBeforeSalePrices("a");
            Assert.IsTrue(priceRes.getTag());
            Assert.AreEqual(1, priceRes.getValue().Count); // one shopping bag
            Assert.AreEqual(1, priceRes.getValue()[storeID].Count); // one item
            Assert.AreEqual(2.4, priceRes.getValue()[storeID][itemID].Key); // price
        }
        
        [TestMethod]
        public void itemsPriceAfterSaleTest()
        {
            testInit();
            controller.addSale("b", storeID, 50, new SaleOnItem(itemID), "New Sale!");
            
            controller.addItemToShoppingCart("a", storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 2.4);
            var priceRes = controller.getItemsAfterSalePrices("a");
            Assert.IsTrue(priceRes.getTag());
            Assert.AreEqual(1, priceRes.getValue().Count); // one shopping bag
            Assert.AreEqual(1, priceRes.getValue()[storeID].Count); // one item
            Assert.AreEqual(1.2, priceRes.getValue()[storeID][itemID].Key); // price
        }
    }
}