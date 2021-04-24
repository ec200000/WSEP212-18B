using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TESTS
{
    [TestClass]
    public class ShoppingBagTests
    {
        private Store shoppingBagStore;
        private int storeItemID;
        private ShoppingBag shoppingBag;

        [TestInitialize]
        public void beforeTests()
        {
            ConcurrentLinkedList<PurchaseType> purchaseRoutes = new ConcurrentLinkedList<PurchaseType>();
            purchaseRoutes.TryAdd(PurchaseType.ImmediatePurchase);
            ResultWithValue<int> addStoreRes = StoreRepository.Instance.addStore("SUPER PHARAM", "Ashdod", new SalePolicy("DEFAULT", new ConcurrentLinkedList<Sale>()), new PurchasePolicy("DEFAULT", purchaseRoutes, new ConcurrentLinkedList<Sale>()), new User("admin"));
            shoppingBagStore = StoreRepository.Instance.getStore(addStoreRes.getValue()).getValue();
            storeItemID = shoppingBagStore.addItemToStorage(500, "black masks", "protects against infection of covid-19", 10, "health").getValue();
            shoppingBag = new ShoppingBag(shoppingBagStore);
        }

        [TestCleanup]
        public void afterTests()
        {
            StoreRepository.Instance.removeStore(shoppingBagStore.storeID);
        }

        [TestMethod]
        public void ShoppingBagTest()
        {
            Assert.IsTrue(shoppingBag.isEmpty());
            Assert.AreEqual(shoppingBagStore, shoppingBag.store);
        }

        [TestMethod]
        public void isEmptyTest()
        {
            Assert.IsTrue(shoppingBag.isEmpty());
            shoppingBag.addItem(storeItemID, 2);
            Assert.IsFalse(shoppingBag.isEmpty());

            shoppingBag.clearShoppingBag();
        }

        [TestMethod]
        public void addItemTest()
        {
            int itemID = storeItemID;

            Assert.IsTrue(shoppingBag.addItem(itemID, 5).getTag());
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out int quantity));
            Assert.AreEqual(5, quantity);

            Assert.IsTrue(shoppingBag.addItem(itemID, 15).getTag());
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out quantity));
            Assert.AreEqual(20, quantity);

            Assert.IsFalse(shoppingBag.addItem(itemID, 481).getTag());   // should fail because there is no enough of the item in storage
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out quantity));
            Assert.AreEqual(20, quantity);

            shoppingBag.removeItem(itemID);

            shoppingBag.clearShoppingBag();
        }

        [TestMethod]
        public void addItemTestFail()
        {
            int itemID = storeItemID;
            
            Assert.IsFalse(shoppingBag.addItem(-1, 5).getTag());   // should fail because there is no such item ID
            Assert.IsFalse(shoppingBag.items.ContainsKey(-1));

            Assert.IsFalse(shoppingBag.addItem(itemID, 0).getTag());   // should fail because it is not possible to add a item with quantity 0
            Assert.IsFalse(shoppingBag.items.ContainsKey(itemID));

            Assert.IsFalse(shoppingBag.addItem(itemID, -5).getTag());   // should fail because it is not possible to add a item with negative quantity
            Assert.IsFalse(shoppingBag.items.ContainsKey(itemID));

        }

        [TestMethod]
        public void removeItemTest()
        {
            int itemID = storeItemID;

            Assert.IsFalse(shoppingBag.removeItem(itemID).getTag());   // should fail because there is no such item ID in the shopping bag
            Assert.IsFalse(shoppingBag.removeItem(-1).getTag());   // should fail because there is no such item ID

            shoppingBag.addItem(itemID, 5);
            Assert.IsTrue(shoppingBag.removeItem(itemID).getTag());
            Assert.IsFalse(shoppingBag.items.ContainsKey(itemID));
        }

        [TestMethod]
        public void changeItemQuantityTest()
        {
            int itemID = storeItemID;
            shoppingBag.addItem(itemID, 5);

            Assert.IsTrue(shoppingBag.changeItemQuantity(itemID, 10).getTag());
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out int quantity));
            Assert.AreEqual(10, quantity);

            Assert.IsTrue(shoppingBag.changeItemQuantity(itemID, 3).getTag());
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out quantity));
            Assert.AreEqual(3, quantity); 
        }

        [TestMethod]
        public void changeItemQuantityTestFail()
        {
            int itemID = storeItemID;
            shoppingBag.addItem(itemID, 3);
            
            Assert.IsFalse(shoppingBag.changeItemQuantity(itemID, 1000).getTag());   // should fail because there is no enough of the item in storage
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out int quantity));
            Assert.AreEqual(3, quantity);   // same as previous quantity, no need to change it

            Assert.IsFalse(shoppingBag.changeItemQuantity(itemID, -1).getTag());   // should fail because it is not possible to add a item with negative quantity
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out quantity));
            Assert.AreEqual(3, quantity);   // same as previous quantity, no need to change it

            Assert.IsTrue(shoppingBag.changeItemQuantity(itemID, 0).getTag());
            Assert.IsFalse(shoppingBag.items.ContainsKey(itemID));   // item with quantity 0 doesnt need to be in the shopping bag

            Assert.IsFalse(shoppingBag.changeItemQuantity(-1, 10).getTag());   // should fail because there is no such item ID
        }

        [TestMethod]
        public void purchaseItemsInBagTest()
        {
            int itemID = storeItemID;
            shoppingBag.addItem(itemID, 5);
            User user = new User("admin");
            ConcurrentDictionary<int, PurchaseType> purchaseType = new ConcurrentDictionary<int, PurchaseType>();
            purchaseType.TryAdd(itemID, PurchaseType.ImmediatePurchase);

            ResultWithValue<double> result = shoppingBag.purchaseItemsInBag(user, purchaseType);
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(10 * 5, result.getValue());
        }

        [TestMethod]
        public void clearShoppingBagTest()
        {
            int itemID = storeItemID;
            shoppingBag.addItem(itemID, 5);
            Assert.IsFalse(shoppingBag.isEmpty());   // should not be empty - 5 items

            shoppingBag.clearShoppingBag();
            Assert.IsTrue(shoppingBag.isEmpty());   // should be empty after clearing the shopping bag
        }
    }
}