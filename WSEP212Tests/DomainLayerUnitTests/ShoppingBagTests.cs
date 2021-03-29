using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WSEP212.DomainLayer.Tests
{
    [TestClass]
    public class ShoppingBagTests
    {
        private Store shoppingBagStore;
        private Item storeItem;
        private ShoppingBag shoppingBag;

        [TestInitialize]
        public void beforeTests()
        {
            shoppingBagStore = new Store(new SalesPolicy(), new PurchasePolicy(), new User("admin"));
            Item item = new Item(500, "black masks", "protects against infection of covid-19", 10, "health");
            storeItem = item;
            shoppingBagStore.addItemToStorage(item);

            StoreRepository.getInstance().addStore(shoppingBagStore);
        }

        [TestCleanup]
        public void afterTests()
        {
            StoreRepository.getInstance().removeStore(shoppingBagStore.storeID);
        }

        [TestMethod]
        public void ShoppingBagTest()
        {
            shoppingBag = new ShoppingBag(shoppingBagStore);
            Assert.IsTrue(shoppingBag.isEmpty());
        }

        [TestMethod]
        public void isEmptyTest()
        {
            Assert.IsTrue(shoppingBag.isEmpty());
            int itemID = storeItem.itemID;
            shoppingBag.addItem(itemID, 2);
            Assert.IsFalse(shoppingBag.isEmpty());

            shoppingBag.clearShoppingBag();
        }

        [TestMethod]
        public void addItemTest()
        {
            int itemID = storeItem.itemID;

            Assert.IsTrue(shoppingBag.addItem(itemID, 5));
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out int quantity));
            Assert.AreEqual(5, quantity);

            Assert.IsTrue(shoppingBag.addItem(itemID, 15));
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out quantity));
            Assert.AreEqual(20, quantity);

            Assert.IsFalse(shoppingBag.addItem(itemID, 481));   // should fail because there is no enough of the item in storage
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out quantity));
            Assert.AreEqual(20, quantity);

            shoppingBag.removeItem(itemID);

            Assert.IsFalse(shoppingBag.addItem(-1, 5));   // should fail because there is no such item ID
            Assert.IsFalse(shoppingBag.items.ContainsKey(-1));

            Assert.IsFalse(shoppingBag.addItem(itemID, 0));   // should fail because it is not possible to add a item with quantity 0
            Assert.IsFalse(shoppingBag.items.ContainsKey(itemID));

            Assert.IsFalse(shoppingBag.addItem(itemID, -5));   // should fail because it is not possible to add a item with negative quantity
            Assert.IsFalse(shoppingBag.items.ContainsKey(itemID));

            shoppingBag.clearShoppingBag();
        }

        [TestMethod]
        public void removeItemTest()
        {
            int itemID = storeItem.itemID;

            Assert.IsFalse(shoppingBag.removeItem(itemID));   // should fail because there is no such item ID in the shopping bag
            Assert.IsFalse(shoppingBag.removeItem(-1));   // should fail because there is no such item ID

            shoppingBag.addItem(itemID, 5);
            Assert.IsTrue(shoppingBag.removeItem(itemID));
            Assert.IsFalse(shoppingBag.items.ContainsKey(itemID));
        }

        [TestMethod]
        public void changeItemQuantityTest()
        {
            int itemID = storeItem.itemID;
            shoppingBag.addItem(itemID, 5);

            Assert.IsTrue(shoppingBag.changeItemQuantity(itemID, 10));
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out int quantity));
            Assert.AreEqual(10, quantity);

            Assert.IsTrue(shoppingBag.changeItemQuantity(itemID, 3));
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out quantity));
            Assert.AreEqual(3, quantity);

            Assert.IsFalse(shoppingBag.changeItemQuantity(itemID, 1000));   // should fail because there is no enough of the item in storage
            Assert.IsTrue(shoppingBag.items.TryGetValue(itemID, out quantity));
            Assert.AreEqual(3, quantity);   // same as previous quantity, no need to change it

            Assert.IsTrue(shoppingBag.changeItemQuantity(itemID, 0));
            Assert.IsFalse(shoppingBag.items.ContainsKey(itemID));   // item with quantity 0 doesnt need to be in the shopping bag

            Assert.IsFalse(shoppingBag.changeItemQuantity(-1, 10));   // should fail because there is no such item ID
        }
    }
}