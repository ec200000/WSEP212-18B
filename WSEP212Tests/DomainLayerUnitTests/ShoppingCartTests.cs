﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace WSEP212.DomainLayer.Tests
{
    [TestClass]
    public class ShoppingCartTests
    {
        private Store storeA;
        private Store storeB;
        private Item itemA;
        private Item itemB;
        private ShoppingCart shoppingCart;

        [TestInitialize]
        public void beforeTests()
        {
            SalesPolicy salesPolicy = new SalesPolicy();
            PurchasePolicy purchasePolicy = new PurchasePolicy();
            User user = new User("admin");

            storeA = new Store(salesPolicy, purchasePolicy, user);
            storeB = new Store(salesPolicy, purchasePolicy, user);
            itemA = new Item(500, "black masks", "protects against infection of covid-19", 10, "health");
            itemB = new Item(50, "black masks", "protects against infection of covid-19", 10, "health");
            storeA.addItemToStorage(itemA);
            storeB.addItemToStorage(itemB);

            StoreRepository.getInstance().addStore(storeA);
            StoreRepository.getInstance().addStore(storeB);

            shoppingCart = new ShoppingCart();
        }

        [TestCleanup]
        public void afterTests()
        {
            StoreRepository.getInstance().removeStore(storeA.storeID);
            StoreRepository.getInstance().removeStore(storeB.storeID);
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
            shoppingCart.addItemToShoppingBag(storeA.storeID, itemA.itemID, 5);
            Assert.IsFalse(shoppingCart.isEmpty());

            shoppingCart.clearShoppingCart();
        }

        [TestMethod]
        public void addItemToShoppingBagTest()
        {
            int storeAID = storeA.storeID, itemAID = itemA.itemID;
            int storeBID = storeB.storeID, itemBID = itemB.itemID;

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(-1, itemAID, 5));   // should fail because there is no such store ID
            Assert.IsTrue(shoppingCart.isEmpty());

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(storeAID, itemBID, 5));   // should fail because there is no such item ID in the store
            Assert.IsTrue(shoppingCart.isEmpty());

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(storeAID, itemAID, 600));   // should fail because there is no enough of the item in storage
            Assert.IsTrue(shoppingCart.isEmpty());

            Assert.IsFalse(shoppingCart.addItemToShoppingBag(storeAID, itemAID, -1));   // should fail because it is not possible to add a item with negative quantity
            Assert.IsTrue(shoppingCart.isEmpty());

            Assert.IsTrue(shoppingCart.addItemToShoppingBag(storeAID, itemAID, 10));
            Assert.IsTrue(shoppingCart.shoppingBags.ContainsKey(storeAID));
            Assert.IsTrue(shoppingCart.shoppingBags[storeAID].items.ContainsKey(itemAID));
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeAID].items[itemAID]);

            Assert.IsTrue(shoppingCart.addItemToShoppingBag(storeBID, itemBID, 20));
            Assert.AreEqual(2, shoppingCart.shoppingBags.Count);
            Assert.IsTrue(shoppingCart.shoppingBags.ContainsKey(storeBID));
            Assert.IsTrue(shoppingCart.shoppingBags[storeBID].items.ContainsKey(itemBID));
            Assert.AreEqual(20, shoppingCart.shoppingBags[storeBID].items[itemBID]);
        }

        [TestMethod]
        public void removeItemFromShoppingBagTest()
        {
            int storeAID = storeA.storeID, itemAID = itemA.itemID;
            int storeBID = storeB.storeID, itemBID = itemB.itemID;
            shoppingCart.addItemToShoppingBag(storeAID, itemAID, 10);
            shoppingCart.addItemToShoppingBag(storeBID, itemBID, 20);

            Assert.IsFalse(shoppingCart.removeItemFromShoppingBag(-1, itemAID));   // should fail because there is no such store ID
            Assert.IsFalse(shoppingCart.shoppingBags[storeAID].isEmpty());
            Assert.IsFalse(shoppingCart.shoppingBags[storeBID].isEmpty());

            Assert.IsFalse(shoppingCart.removeItemFromShoppingBag(storeAID, itemBID));   // should fail because there is no such item ID in the store
            Assert.IsFalse(shoppingCart.shoppingBags[storeAID].isEmpty());
            Assert.IsFalse(shoppingCart.shoppingBags[storeBID].isEmpty());

            Assert.IsTrue(shoppingCart.removeItemFromShoppingBag(storeAID, itemAID));
            Assert.AreEqual(1, shoppingCart.shoppingBags.Count);
            Assert.IsFalse(shoppingCart.shoppingBags.ContainsKey(storeAID));
            Assert.IsFalse(shoppingCart.shoppingBags[storeBID].isEmpty());

            Assert.IsTrue(shoppingCart.removeItemFromShoppingBag(storeBID, itemBID));
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void changeItemQuantityInShoppingBagTest()
        {
            int storeID = storeA.storeID, itemID = itemA.itemID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(-1, itemID, 5));   // should fail because there is no such store ID
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(storeID, -1, 5));    // should fail because there is no such item ID in the store
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, 600));   // should fail because there is no enough of the item in storage
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);

            Assert.IsFalse(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, -1));   // should fail because -1 is not a valid quantity
            Assert.AreEqual(10, shoppingCart.shoppingBags[storeID].items[itemID]);

            Assert.IsTrue(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, 5)); 
            Assert.AreEqual(5, shoppingCart.shoppingBags[storeID].items[itemID]);

            Assert.IsTrue(shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, 0));
            Assert.IsFalse(shoppingCart.shoppingBags.ContainsKey(storeID));
            Assert.IsTrue(shoppingCart.isEmpty());
        }

        [TestMethod]
        public void clearShoppingCartTest()
        {
            int storeID = storeA.storeID, itemID = itemA.itemID;
            shoppingCart.addItemToShoppingBag(storeID, itemID, 10);
            Assert.IsFalse(shoppingCart.isEmpty());   // should not be empty - 1 shopping bag with 10 items

            shoppingCart.clearShoppingCart();
            Assert.IsTrue(shoppingCart.isEmpty());   // should be empty after clearing the shopping cart
        }
    }
}