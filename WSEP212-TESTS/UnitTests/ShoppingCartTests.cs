using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;
using WSEP212_TEST.UnitTests.UnitTestMocks;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class ShoppingCartTests
    {
        private Store storeA;
        private Store storeB;
        private int itemAID;
        private int itemBID;
        private ShoppingCart shoppingCart;
        private ItemPurchaseType purchaseType;

        [TestInitialize]
        public void beforeTests()
        {
            ConcurrentLinkedList<PurchaseType> purchaseRoutes = new ConcurrentLinkedList<PurchaseType>();
            purchaseRoutes.TryAdd(PurchaseType.ImmediatePurchase);
            SalePolicyMock salesPolicy = new SalePolicyMock();
            PurchasePolicyMock purchasePolicy = new PurchasePolicyMock();
            User user = new User("admin");

            ResultWithValue<int> addStoreARes = StoreRepository.Instance.addStore("SUPER PHARAM", "Ashdod", salesPolicy, purchasePolicy, user);
            ResultWithValue<int> addStoreBRes = StoreRepository.Instance.addStore("SUPER PHARAM", "Holon", salesPolicy, purchasePolicy, user);
            storeA = StoreRepository.Instance.getStore(addStoreARes.getValue()).getValue();
            storeB = StoreRepository.Instance.getStore(addStoreBRes.getValue()).getValue();

            itemAID = storeA.addItemToStorage(500, "black masks", "protects against infection of covid-19", 10, "health").getValue();
            itemBID = storeB.addItemToStorage(50, "black masks", "protects against infection of covid-19", 10, "health").getValue();
            purchaseType = new ItemImmediatePurchase(10);

            User buyer = new User("Sagiv", 21);
            shoppingCart = new ShoppingCart(buyer);
        }

        [TestCleanup]
        public void afterTests()
        {
            StoreRepository.Instance.removeStore(storeA.storeID);
            StoreRepository.Instance.removeStore(storeB.storeID);
        }

        [TestMethod]
        public void ShoppingCartTest()
        {
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void isEmptyTest()
        {
            Assert.IsTrue(shoppingCart.isEmpty());
            shoppingCart.addItemToShoppingBag(storeA.storeID, itemAID, 5, purchaseType);
            Assert.IsFalse(shoppingCart.isEmpty());

            shoppingCart.clearShoppingCart();
        }

        [TestMethod]
        public void addItemToShoppingBagTest()
        {
            int storeAID = storeA.storeID;
            int storeBID = storeB.storeID;

            Assert.IsTrue(shoppingCart.addItemToShoppingBag(storeAID, itemAID, 10, purchaseType).getTag());
            Assert.IsTrue(shoppingCart.shoppingBags.ContainsKey(storeAID));
            Assert.IsTrue(shoppingCart.shoppingBags[storeAID].items.ContainsKey(itemAID));
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeAID].items[itemAID]);

            Assert.IsTrue(shoppingCart.addItemToShoppingBag(storeBID, itemBID, 20, purchaseType).getTag());
            Assert.AreEqual(2, shoppingCart.shoppingBags.Count);
            Assert.IsTrue(shoppingCart.shoppingBags.ContainsKey(storeBID));
            Assert.IsTrue(shoppingCart.shoppingBags[storeBID].items.ContainsKey(itemBID));
            Assert.AreEqual(20, shoppingCart.shoppingBags[storeBID].items[itemBID]);
        }

        [TestMethod]
        public void addItemToBagNoSuchStoreTest()
        {
            int storeAID = storeA.storeID;

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(-1, itemAID, 5, purchaseType).getTag());   // should fail because there is no such store ID
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void addItemToBagNoSuchItemTest()
        {
            int storeAID = storeA.storeID;

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(storeAID, itemBID, 5, purchaseType).getTag());   // should fail because there is no such item ID in the store
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void addItemToBagNotEnoughInStorageTest()
        {
            int storeAID = storeA.storeID;

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(storeAID, itemAID, 600, purchaseType).getTag());   // should fail because there is no enough of the item in storage
            Assert.IsTrue(shoppingCart.isEmpty());

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(storeAID, itemAID, -1, purchaseType).getTag());   // should fail because it is not possible to add a item with negative quantity
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void addItemToBagWithNegQuantityTest()
        {
            int storeAID = storeA.storeID;

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(storeAID, itemAID, -1, purchaseType).getTag());   // should fail because it is not possible to add a item with negative quantity
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void removeItemFromShoppingBagTest()
        {
            int storeAID = storeA.storeID;
            int storeBID = storeB.storeID;
            shoppingCart.addItemToShoppingBag(storeAID, itemAID, 10, purchaseType);
            shoppingCart.addItemToShoppingBag(storeBID, itemBID, 20, purchaseType);

            Assert.IsTrue(shoppingCart.removeItemFromShoppingBag(storeAID, itemAID).getTag());
            Assert.AreEqual(1, shoppingCart.shoppingBags.Count);
            Assert.IsFalse(shoppingCart.shoppingBags.ContainsKey(storeAID));
            Assert.IsFalse(shoppingCart.shoppingBags[storeBID].isEmpty());

            Assert.IsTrue(shoppingCart.removeItemFromShoppingBag(storeBID, itemBID).getTag());
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void removeItemFromBagNoSuchStoreTest()
        {
            int storeAID = storeA.storeID;
            int storeBID = storeB.storeID;
            shoppingCart.addItemToShoppingBag(storeAID, itemAID, 10, purchaseType);
            shoppingCart.addItemToShoppingBag(storeBID, itemBID, 20, purchaseType);

            Assert.IsFalse(shoppingCart.removeItemFromShoppingBag(-1, itemAID).getTag());   // should fail because there is no such store ID
            Assert.IsFalse(shoppingCart.shoppingBags[storeAID].isEmpty());
            Assert.IsFalse(shoppingCart.shoppingBags[storeBID].isEmpty());
        }

        [TestMethod]
        public void removeItemFromBagNoSuchItemTest()
        {
            int storeAID = storeA.storeID;
            int storeBID = storeB.storeID;
            shoppingCart.addItemToShoppingBag(storeAID, itemAID, 10, purchaseType);
            shoppingCart.addItemToShoppingBag(storeBID, itemBID, 20, purchaseType);

            Assert.IsFalse(shoppingCart.removeItemFromShoppingBag(storeAID, itemBID).getTag());   // should fail because there is no such item ID in the store
            Assert.IsFalse(shoppingCart.shoppingBags[storeAID].isEmpty());
            Assert.IsFalse(shoppingCart.shoppingBags[storeBID].isEmpty());
        }

        [TestMethod]
        public void changeItemQuantityInShoppingBagTest()
        {
            int storeID = storeA.storeID, itemID = itemAID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10, purchaseType);

            Assert.IsTrue(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, 5).getTag()); 
            Assert.AreEqual(5, shoppingCart.shoppingBags[storeID].items[itemID]);

            Assert.IsTrue(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, 0).getTag());
            Assert.IsFalse(shoppingCart.shoppingBags.ContainsKey(storeID));
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void changeItemQuantityInBagNoSuchStoreTest()
        {
            int storeID = storeA.storeID, itemID = itemAID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10, purchaseType);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(-1, itemID, 5).getTag());   // should fail because there is no such store ID
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);
        }

        [TestMethod]
        public void changeItemQuantityInBagNoSuchItemTest()
        {
            int storeID = storeA.storeID, itemID = itemAID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10, purchaseType);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(storeID, -1, 5).getTag());    // should fail because there is no such item ID in the store
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);
        }

        [TestMethod]
        public void changeItemQuantityInBagNotEnoughStorageTest()
        {
            int storeID = storeA.storeID, itemID = itemAID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10, purchaseType);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, 600).getTag());   // should fail because there is no enough of the item in storage
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, -1).getTag());   // should fail because -1 is not a valid quantity
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);
        }

        [TestMethod]
        public void changeItemNegQuantityInBagTest()
        {
            int storeID = storeA.storeID, itemID = itemAID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10, purchaseType);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, -1).getTag());   // should fail because -1 is not a valid quantity
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);
        }

        [TestMethod]
        public void purchaseItemsInCartTest()
        {
            shoppingCart.addItemToShoppingBag(storeA.storeID, itemAID, 3, purchaseType);
            shoppingCart.addItemToShoppingBag(storeB.storeID, itemBID, 5, purchaseType);
            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
            StoreRepository.Instance.stores[storeA.storeID].deliverySystem = DeliverySystemMock.Instance;
            StoreRepository.Instance.stores[storeB.storeID].deliverySystem = DeliverySystemMock.Instance;

            ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> result = shoppingCart.purchaseItemsInCart();
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(10 * 3, result.getValue()[storeA.storeID].getPurchaseTotalPrice());
            Assert.AreEqual(10 * 5, result.getValue()[storeB.storeID].getPurchaseTotalPrice());
        }

        [TestMethod]
        public void clearShoppingCartTest()
        {
            int storeID = storeA.storeID, itemID = itemAID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10, purchaseType);
            Assert.IsFalse(shoppingCart.isEmpty());   // should not be empty - 1 shopping bag with 10 items

            shoppingCart.clearShoppingCart();
            Assert.IsTrue(shoppingCart.isEmpty());   // should be empty after clearing the shopping cart
        }
    }
}