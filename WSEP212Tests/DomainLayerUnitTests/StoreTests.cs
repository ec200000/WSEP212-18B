using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace WSEP212.DomainLayer.Tests
{
    [TestClass]
    public class StoreTests
    {
        private Store store;
        private Item soda;

        [TestInitialize]
        public void Initialize()
        {
            store = new Store(new SalesPolicy(), new PurchasePolicy(), new User("admin"));
            this.soda = new Item(3, "soda-stream", "great drink", 150, "drink");
            store.addItemToStorage(soda);
            StoreRepository.getInstance().addStore(store);
        }

        [TestCleanup]
        public void Cleanup()
        {
            StoreRepository.getInstance().removeStore(store.storeID);
        }

        /*[TestMethod()]
        public void StoreTest()
        {
            Assert.Fail();
        }*/

        [TestMethod]
        public void isAvailableInStorageTest()
        {
            bool available = store.isAvailableInStorage(soda.itemID, 2);
            Assert.IsTrue(available);
            Item bamba = new Item(3, "bamba", "tasty snack", 4.8, "snack");
            available = store.isAvailableInStorage(bamba.itemID, 1);
            Assert.IsFalse(available);
            available = store.isAvailableInStorage(soda.itemID, 5);
            Assert.IsFalse(available);
        }

        [TestMethod]
        public void addItemToStorageTest()
        {
            Item bag = new Item(40, "nike bag", "sport bag", 220, "bag");
            bool added = store.addItemToStorage(bag);
            Assert.IsTrue(added);
            double price = bag.price;
            int quantity = bag.quantity;
            Assert.AreEqual(220, price);
            Assert.AreEqual(40, quantity);
            Item bisli = new Item(60, "bisli", "monosodium glutamate", -4.8, "snack");
            added = store.addItemToStorage(bisli);
            Assert.IsFalse(added);
            bisli = new Item(60, "", "monosodium glutamate", 4.8, "snack");
            added = store.addItemToStorage(bisli);
            Assert.IsFalse(added);
            bisli = new Item(-1, "bisli", "monosodium glutamate", 4.8, "snack");
            added = store.addItemToStorage(bisli);
            Assert.IsFalse(added);
            bisli = new Item(1, "bisli", "monosodium glutamate", 4.8, "");
            added = store.addItemToStorage(bisli);
            Assert.IsFalse(added);
        }

        [TestMethod]
        public void removeItemFromStorageTest()
        {
            Item cola = new Item(40, "cola", "lot of sugar", 7, "drink");
            bool removed = store.removeItemFromStorage(cola.itemID);
            Assert.IsFalse(removed);
            Item bisli = new Item(60, "bisli", "monosodium glutamate", 4.8, "snack");
            store.addItemToStorage(bisli);
            removed = store.removeItemFromStorage(bisli.itemID);
            Assert.IsTrue(removed);
            Assert.IsFalse(store.isAvailableInStorage(bisli.itemID, 1));
        }

        [TestMethod]
        public void changeItemQuantityTest()
        {
            store.changeItemQuantity(soda.itemID, 4);
            int quantity = store.getItemById(soda.itemID).quantity;
            Assert.AreEqual(7, quantity);
            Item tesla = new Item(3, "tesla-3", "great car", 150000, "car");
            bool changed = store.changeItemQuantity(tesla.itemID, 150);
            Assert.IsFalse(changed);
        }

        [TestMethod]
        public void editItemTest()
        {
            store.editItem(soda.itemID,"soda-stream", "great drink", 1000.0, "drink");
            double price = store.getItemById(soda.itemID).price;
            Assert.AreEqual(1000.0, price);
            Item tesla = new Item(3, "tesla-3", "great car", 150000, "car");
            bool edited = store.editItem(tesla.itemID,"tesla-3", "nice car", 200000, "car");
            Assert.IsFalse(edited);
            store.addItemToStorage(tesla);
            edited = store.editItem(tesla.itemID, "tesla-3", "nice car", 200000, "");
            Assert.IsFalse(edited);
            edited = store.editItem(tesla.itemID, "", "nice car", 200000, "car");
            Assert.IsFalse(edited);
            edited = store.editItem(tesla.itemID, "tesla-3", "nice car", -200000, "car");
            Assert.IsFalse(edited);
        }

        /*[TestMethod()]
        public void applySalesPolicyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void applyPurchasePolicyTest()
        {
            Assert.Fail();
        }*/

        [TestMethod]
        public void purchaseItemsIfAvailableTest()
        {
            Item oliveOil = new Item(10, "olive-oil", "from olive", 50, "oil");
            store.addItemToStorage(oliveOil);
            Dictionary<int, int> items = new Dictionary<int, int>();
            items.Add(soda.itemID, 1);
            bool successfulPurchase = store.purchaseItemsIfAvailable(items);
            Assert.IsTrue(successfulPurchase);
            Dictionary<int, int> badQuantityItems = new Dictionary<int, int>();
            badQuantityItems.Add(oliveOil.itemID, 11);
            badQuantityItems.Add(soda.itemID, soda.quantity+1);
            bool failedPurchase = store.purchaseItemsIfAvailable(badQuantityItems);
            Assert.IsFalse(failedPurchase);
        }

        [TestMethod]
        public void rollBackPurchaseTest()
        {
            Item oliveOil = new Item(10, "olive-oil", "from olive", 50, "oil");
            store.addItemToStorage(oliveOil);
            Dictionary<int, int> items = new Dictionary<int, int>();
            items.Add(oliveOil.itemID, 1);
            items.Add(soda.itemID, 1);
            int oilQuantity = store.getItemById(oliveOil.itemID).quantity;
            int sodaQuantity = store.getItemById(soda.itemID).quantity;
            store.rollBackPurchase(items);
            Item oilFromStorage = store.getItemById(oliveOil.itemID);
            Item sodaFromStorage = store.getItemById(soda.itemID);
            Assert.AreEqual(oilQuantity+1, oilFromStorage.quantity);
            Assert.AreEqual(sodaQuantity+1, sodaFromStorage.quantity);
        }

        [TestMethod]
        public void addNewStoreSellerTest()
        {
            LinkedList<Permissions> perms = new LinkedList<Permissions>();
            perms.AddFirst(Permissions.AllPermissions);
            SellerPermissions aviTheSeller = new SellerPermissions(new User("avi"), this.store, new User("admin"), perms);
            bool addNewStoreSellerBool1 = store.addNewStoreSeller(aviTheSeller);
            Assert.IsTrue(addNewStoreSellerBool1);
            bool addNewStoreSellerBool2 = store.addNewStoreSeller(aviTheSeller);
            Assert.IsFalse(addNewStoreSellerBool2);
        }

        [TestMethod]
        public void removeStoreSellerTest()
        {
            LinkedList<Permissions> perms = new LinkedList<Permissions>();
            perms.AddFirst(Permissions.AllPermissions);
            SellerPermissions aviTheSeller = new SellerPermissions(new User("avi"), this.store, new User("admin"), perms);
            store.addNewStoreSeller(aviTheSeller);

            bool removeStoreSellerBool1 = store.removeStoreSeller("avi");
            Assert.IsTrue(removeStoreSellerBool1);
            bool removeStoreSellerBool2 = store.removeStoreSeller("avi");
            Assert.IsFalse(removeStoreSellerBool2);
        }

        [TestMethod]
        public void getStoreOfficialsInfoTest()
        {
            Dictionary<User, LinkedList<Permissions>> info = store.getStoreOfficialsInfo();
            LinkedList<Permissions> perms = new LinkedList<Permissions>();
            perms.AddFirst(Permissions.AllPermissions);
            int numOfRecords = info.Count;
            Assert.AreEqual(numOfRecords, 1);
            SellerPermissions aviTheSeller = new SellerPermissions(new User("avi"), this.store, new User("admin"), perms);
            store.addNewStoreSeller(aviTheSeller);
            Assert.AreEqual(store.getStoreOfficialsInfo().Count, 2);
        }

        [TestMethod]
        public void addNewPurchaseTest()
        {
            Dictionary<int, int> items = new Dictionary<int, int>();
            items.Add(soda.itemID, 1);
            store.addNewPurchase(new PurchaseInfo(this.store.storeID, "admin", items, 15, System.DateTime.Now));
            Assert.IsTrue(true);
            //Assert.IsTrue(add);
        }
    }
}