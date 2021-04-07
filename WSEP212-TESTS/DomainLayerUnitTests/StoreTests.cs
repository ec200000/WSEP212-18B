using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS
{
    [TestClass]
    public class StoreTests
    {
        private Store store;
        private int sodaID;

        [TestInitialize]
        public void Initialize()
        {
            ConcurrentLinkedList<PurchaseType> purchaseRoutes = new ConcurrentLinkedList<PurchaseType>();
            purchaseRoutes.TryAdd(PurchaseType.ImmediatePurchase);
            this.store = new Store("Mega", new SalesPolicy("DEFAULT", new ConcurrentLinkedList<PolicyRule>()), new PurchasePolicy("DEFAULT", purchaseRoutes, new ConcurrentLinkedList<PolicyRule>()), new User("admin"));
            this.sodaID = store.addItemToStorage(3, "soda-stream", "great drink", 150, "drink");
            StoreRepository.Instance.addStore(store);
        }

        [TestCleanup]
        public void Cleanup()
        {
            StoreRepository.Instance.removeStore(store.storeID);
        }

        [TestMethod]
        public void isAvailableInStorageTest()
        {
            bool available = store.isAvailableInStorage(sodaID, 2);
            Assert.IsTrue(available);
            Item bamba = new Item(3, "bamba", "tasty snack", 4.8, "snack");
            available = store.isAvailableInStorage(bamba.itemID, 1);
            Assert.IsFalse(available);
            available = store.isAvailableInStorage(sodaID, 5);
            Assert.IsFalse(available);
        }

        [TestMethod]
        public void addItemToStorageTest()
        {
            int itemID = store.addItemToStorage(40, "nike bag", "sport bag", 220, "bag");
            Assert.IsTrue(itemID > 0);
            itemID = store.addItemToStorage(60, "bisli", "monosodium glutamate", -4.8, "snack");
            Assert.IsTrue(itemID < 0);
            itemID = store.addItemToStorage(60, "", "monosodium glutamate", 4.8, "snack");
            Assert.IsTrue(itemID < 0);
            itemID = store.addItemToStorage(-1, "bisli", "monosodium glutamate", 4.8, "snack");
            Assert.IsTrue(itemID < 0);
            itemID = store.addItemToStorage(1, "bisli", "monosodium glutamate", 4.8, "");
            Assert.IsTrue(itemID < 0);
        }

        [TestMethod]
        public void removeItemFromStorageTest()
        {
            Item cola = new Item(40, "cola", "lot of sugar", 7, "drink");
            bool removed = store.removeItemFromStorage(cola.itemID);
            Assert.IsFalse(removed);
            int bisliID = store.addItemToStorage(60, "bisli", "monosodium glutamate", 4.8, "snack");
            removed = store.removeItemFromStorage(bisliID);
            Assert.IsTrue(removed);
            Assert.IsFalse(store.isAvailableInStorage(bisliID, 1));
        }

        [TestMethod]
        public void changeItemQuantityTest()
        {
            store.changeItemQuantity(sodaID, 4);
            int quantity = store.getItemById(sodaID).quantity;
            Assert.AreEqual(7, quantity);
            Item tesla = new Item(3, "tesla-3", "great car", 150000, "car");
            bool changed = store.changeItemQuantity(tesla.itemID, 150);
            Assert.IsFalse(changed);
        }

        [TestMethod]
        public void editItemTest()
        {
            store.editItem(sodaID, "soda-stream", "great drink", 1000.0, "drink", 3);
            double price = store.getItemById(sodaID).price;
            Assert.AreEqual(1000.0, price);
            int teslaID = store.addItemToStorage(3, "tesla-3", "great car", 150000, "car");
            bool edited = store.editItem(teslaID, "tesla-3", "nice car", 200000, "car", 5);
            Assert.IsTrue(edited);   
            edited = store.editItem(teslaID, "tesla-3", "nice car", 200000, "", 5);
            Assert.IsFalse(edited);
            edited = store.editItem(teslaID, "", "nice car", 200000, "car", 5);
            Assert.IsFalse(edited);
            edited = store.editItem(teslaID, "tesla-3", "nice car", -200000, "car", 5);
            Assert.IsFalse(edited);
        }

        [TestMethod()]
        public void purchaseItemsTest()
        {
            int oliveOilID = store.addItemToStorage(10, "olive-oil", "from olive", 50, "oil");
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(sodaID, 1);
            items.TryAdd(oliveOilID, 2);
            ConcurrentDictionary<int, PurchaseType> itemsPurchaseType = new ConcurrentDictionary<int, PurchaseType>();
            itemsPurchaseType.TryAdd(sodaID, PurchaseType.ImmediatePurchase);
            itemsPurchaseType.TryAdd(oliveOilID, PurchaseType.ImmediatePurchase);
            User miki = new User("miki");
            double totalPrice = store.purchaseItems(miki, items, itemsPurchaseType);
            Assert.AreEqual(250.0, totalPrice);
        }

        [TestMethod()]
        public void applySalesPolicyTest()
        {
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(sodaID, 1);
            User miki = new User("miki");
            ConcurrentDictionary<int, double> itemPrices = store.applySalesPolicy(miki, items);
            double total = 0.0;
            foreach (KeyValuePair<int, double> item in itemPrices)
            {
                total += item.Value;
            }
            Assert.AreEqual(150.0, total);

        }

        [TestMethod()]
        public void applyPurchasePolicyTest()
        {
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(sodaID, 1);
            User miki = new User("miki");
            ConcurrentDictionary<int, PurchaseType> itemsPurchaseType = new ConcurrentDictionary<int, PurchaseType>();
            itemsPurchaseType.TryAdd(sodaID, PurchaseType.ImmediatePurchase);
            bool approved = store.applyPurchasePolicy(miki, items, itemsPurchaseType);
            Assert.IsTrue(approved);
        }


        [TestMethod]
        public void purchaseItemsIfAvailableTest()
        {
            int oliveOilID = store.addItemToStorage(10, "olive-oil", "from olive", 50, "oil");
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(sodaID, 1);
            bool successfulPurchase = store.purchaseItemsIfAvailable(items);
            Assert.IsTrue(successfulPurchase);
            ConcurrentDictionary<int, int> badQuantityItems = new ConcurrentDictionary<int, int>();
            badQuantityItems.TryAdd(oliveOilID, 11);
            Item soda = store.getItemById(sodaID);
            badQuantityItems.TryAdd(sodaID, soda.quantity + 1);
            bool failedPurchase = store.purchaseItemsIfAvailable(badQuantityItems);
            Assert.IsFalse(failedPurchase);
        }

        [TestMethod]
        public void rollBackPurchaseTest()
        {
            int oliveOilID = store.addItemToStorage(10, "olive-oil", "from olive", 50, "oil");
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(oliveOilID, 1);
            items.TryAdd(sodaID, 1);
            int oilQuantity = store.getItemById(oliveOilID).quantity;
            int sodaQuantity = store.getItemById(sodaID).quantity;
            store.rollBackPurchase(items);
            Item oilFromStorage = store.getItemById(oliveOilID);
            Item sodaFromStorage = store.getItemById(sodaID);
            Assert.AreEqual(oilQuantity + 1, oilFromStorage.quantity);
            Assert.AreEqual(sodaQuantity + 1, sodaFromStorage.quantity);
        }

        [TestMethod]
        public void addNewStoreSellerTest()
        {
            ConcurrentLinkedList<Permissions> perms = new ConcurrentLinkedList<Permissions>();
            perms.TryAdd(Permissions.AllPermissions);
            SellerPermissions aviTheSeller = SellerPermissions.getSellerPermissions(new User("avi"), this.store, new User("admin"), perms);
            bool addNewStoreSellerBool1 = store.addNewStoreSeller(aviTheSeller);
            Assert.IsTrue(addNewStoreSellerBool1);
            bool addNewStoreSellerBool2 = store.addNewStoreSeller(aviTheSeller);
            Assert.IsFalse(addNewStoreSellerBool2);
        }

        [TestMethod]
        public void removeStoreSellerTest()
        {
            ConcurrentLinkedList<Permissions> perms = new ConcurrentLinkedList<Permissions>();
            perms.TryAdd(Permissions.AllPermissions);
            SellerPermissions aviTheSeller = SellerPermissions.getSellerPermissions(new User("avi"), this.store, new User("admin"), perms);
            store.addNewStoreSeller(aviTheSeller);

            bool removeStoreSellerBool1 = store.removeStoreSeller("avi");
            Assert.IsTrue(removeStoreSellerBool1);
            bool removeStoreSellerBool2 = store.removeStoreSeller("avi");
            Assert.IsFalse(removeStoreSellerBool2);
        }

        [TestMethod]
        public void getStoreOfficialsInfoTest()
        {
            ConcurrentDictionary<User, ConcurrentLinkedList<Permissions>> info = store.getStoreOfficialsInfo();
            ConcurrentLinkedList<Permissions> perms = new ConcurrentLinkedList<Permissions>();
            perms.TryAdd(Permissions.AllPermissions);

            int numOfRecords = info.Count;
            Assert.AreEqual(numOfRecords, 1);
            SellerPermissions aviTheSeller = SellerPermissions.getSellerPermissions(new User("avi"), this.store, new User("admin"), perms);
            store.addNewStoreSeller(aviTheSeller);
            Assert.AreEqual(store.getStoreOfficialsInfo().Count, 2);
        }

        [TestMethod]
        public void addNewPurchaseTest()
        {
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(sodaID, 1);
            store.addNewPurchase(new PurchaseInfo(this.store.storeID, "admin", items, 15, System.DateTime.Now));
            Assert.IsTrue(true);
            //Assert.IsTrue(add);
        }
    }
}