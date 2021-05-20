using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.ServiceLayer.Result;
using WSEP212_TEST.UnitTests.UnitTestMocks;

namespace WSEP212_TESTS.UnitTests
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
            ResultWithValue<int> addStoreRes = StoreRepository.Instance.addStore("Mega", "Holon", new SalePolicyMock(), new PurchasePolicyMock(), new User("admin"));
            this.store = StoreRepository.Instance.getStore(addStoreRes.getValue()).getValue();
            this.sodaID = this.store.addItemToStorage(3, "soda-stream", "great drink", 150, "drink").getValue();
        }

        [TestCleanup]
        public void Cleanup()
        {
            StoreRepository.Instance.removeStore(store.storeID);
        }

        [TestMethod]
        public void availableInStorageTest()
        {
            RegularResult available = store.isAvailableInStorage(sodaID, 2);
            Assert.IsTrue(available.getTag());
        }

        [TestMethod]
        public void notAvailableInStorageTest()
        {
            Item bamba = new Item(3, "bamba", "tasty snack", 4.8, "snack");
            RegularResult available = store.isAvailableInStorage(bamba.itemID, 1);
            Assert.IsFalse(available.getTag());
            available = store.isAvailableInStorage(sodaID, 5);
            Assert.IsFalse(available.getTag());
        }

        [TestMethod]
        public void addItemToStorageTest()
        {
            ResultWithValue<int> itemIDRes = store.addItemToStorage(40, "nike bag", "sport bag", 220, "bag");
            Assert.IsTrue(itemIDRes.getTag());
        }

        [TestMethod]
        public void addItemToStorageNegPriceTest()
        {
            ResultWithValue<int> itemIDRes = store.addItemToStorage(60, "bisli", "monosodium glutamate", -4.8, "snack");
            Assert.IsFalse(itemIDRes.getTag());
        }

        [TestMethod]
        public void addItemToStorageEmptyNameTest()
        {
            ResultWithValue<int> itemIDRes = store.addItemToStorage(60, "", "monosodium glutamate", 4.8, "snack");
            Assert.IsFalse(itemIDRes.getTag());
        }

        [TestMethod]
        public void addItemToStorageNegQuantityTest()
        {
            ResultWithValue<int> itemIDRes = store.addItemToStorage(-1, "bisli", "monosodium glutamate", 4.8, "snack");
            Assert.IsFalse(itemIDRes.getTag());
        }

        [TestMethod]
        public void addItemToStorageEmptyCategoryTest()
        {
            ResultWithValue<int> itemIDRes = store.addItemToStorage(1, "bisli", "monosodium glutamate", 4.8, "");
            Assert.IsFalse(itemIDRes.getTag());
        }

        [TestMethod]
        public void removeItemFromStorageTest()
        {
            int bisliID = store.addItemToStorage(60, "bisli", "monosodium glutamate", 4.8, "snack").getValue();
            RegularResult removed = store.removeItemFromStorage(bisliID);
            Assert.IsTrue(removed.getTag());
            Assert.IsFalse(store.isAvailableInStorage(bisliID, 1).getTag());
        }

        [TestMethod]
        public void removeItemNotInStorageTest()
        {
            Item cola = new Item(40, "cola", "lot of sugar", 7, "drink");
            RegularResult removed = store.removeItemFromStorage(cola.itemID);
            Assert.IsFalse(removed.getTag());
        }

        [TestMethod]
        public void changeItemQuantityTest()
        {
            store.changeItemQuantity(sodaID, 4);
            int quantity = store.getItemById(sodaID).getValue().quantity;
            Assert.AreEqual(7, quantity);
        }

        [TestMethod]
        public void changeItemQuantityNotExistTest()
        {
            Item tesla = new Item(3, "tesla-3", "great car", 150000, "car");
            RegularResult changed = store.changeItemQuantity(tesla.itemID, 150);
            Assert.IsFalse(changed.getTag());
        }

        [TestMethod]
        public void editItemTest()
        {
            store.editItem(sodaID, "soda-stream", "great drink", 1000.0, "drink", 3);
            double price = store.getItemById(sodaID).getValue().price;
            Assert.AreEqual(1000.0, price);
            int teslaID = store.addItemToStorage(3, "tesla-3", "great car", 150000, "car").getValue();
            RegularResult edited = store.editItem(teslaID, "tesla-3", "nice car", 200000, "car", 5);
            Assert.IsTrue(edited.getTag());   
        }

        [TestMethod]
        public void editItemEmptyCategoryTest()
        {
            RegularResult edited = store.editItem(sodaID, "soda-stream", "great drink", 1000.0, "", 3);
            Assert.IsFalse(edited.getTag());
        }

        [TestMethod]
        public void editItemEmptyNameTest()
        {
            RegularResult edited = store.editItem(sodaID, "", "great drink", 1000.0, "", 3);
            Assert.IsFalse(edited.getTag());
        }

        [TestMethod]
        public void editItemNegPriceTest()
        {
            RegularResult edited = store.editItem(sodaID, "soda-stream", "great drink", -1000.0, "", 3);
            Assert.IsFalse(edited.getTag());
        }

        [TestMethod()]
        public void purchaseItemsTest()
        {
            int oliveOilID = store.addItemToStorage(10, "olive-oil", "from olive", 50, "oil").getValue();
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(sodaID, 1);
            items.TryAdd(oliveOilID, 2);
            ConcurrentDictionary<int, PurchaseType> itemsPurchaseType = new ConcurrentDictionary<int, PurchaseType>();
            itemsPurchaseType.TryAdd(sodaID, PurchaseType.ImmediatePurchase);
            itemsPurchaseType.TryAdd(oliveOilID, PurchaseType.ImmediatePurchase);
            User miki = new User("miki");
            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
            StoreRepository.Instance.stores[store.storeID].deliverySystem = DeliverySystemMock.Instance;

            ResultWithValue<double> totalPrice = store.purchaseItems(miki, items, itemsPurchaseType);
            Assert.IsTrue(totalPrice.getTag());
            Assert.AreEqual(250.0, totalPrice.getValue());
        }

        [TestMethod()]
        public void applySalesPolicyTest()
        {
            ConcurrentDictionary<Item, int> items = new ConcurrentDictionary<Item, int>();
            ConcurrentDictionary<int, PurchaseType> itemsPurchaseType = new ConcurrentDictionary<int, PurchaseType>();
            items.TryAdd(store.getItemById(sodaID).getValue(), 1);
            itemsPurchaseType.TryAdd(sodaID, PurchaseType.ImmediatePurchase);
            User miki = new User("miki");
            PurchaseDetails purchaseDetails = new PurchaseDetails(miki, items, itemsPurchaseType);
            ConcurrentDictionary<int, double> itemPrices = store.applySalesPolicy(items, purchaseDetails);
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
            ConcurrentDictionary<Item, int> items = new ConcurrentDictionary<Item, int>();
            items.TryAdd(store.getItemById(sodaID).getValue(), 1);
            User miki = new User("miki");
            ConcurrentDictionary<int, PurchaseType> itemsPurchaseType = new ConcurrentDictionary<int, PurchaseType>();
            itemsPurchaseType.TryAdd(sodaID, PurchaseType.ImmediatePurchase);
            PurchaseDetails purchaseDetails = new PurchaseDetails(miki, items, itemsPurchaseType);
            RegularResult approved = store.applyPurchasePolicy(purchaseDetails);
            Assert.IsTrue(approved.getTag());
        }


        [TestMethod]
        public void purchaseItemsAvailableTest()
        {
            int oliveOilID = store.addItemToStorage(10, "olive-oil", "from olive", 50, "oil").getValue();
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(sodaID, 1);
            RegularResult successfulPurchase = store.purchaseItemsIfAvailable(items);
            Assert.IsTrue(successfulPurchase.getTag());
            store.removeItemFromStorage(oliveOilID);
        }

        [TestMethod]
        public void purchaseItemsNotAvailableTest()
        {
            int oliveOilID = store.addItemToStorage(10, "olive-oil", "from olive", 50, "oil").getValue();
            ConcurrentDictionary<int, int> badQuantityItems = new ConcurrentDictionary<int, int>();
            badQuantityItems.TryAdd(oliveOilID, 11);
            Item soda = store.getItemById(sodaID).getValue();
            badQuantityItems.TryAdd(sodaID, soda.quantity + 1);
            RegularResult failedPurchase = store.purchaseItemsIfAvailable(badQuantityItems);
            Assert.IsFalse(failedPurchase.getTag());
        }

        [TestMethod]
        public void rollBackPurchaseTest()
        {
            int oliveOilID = store.addItemToStorage(10, "olive-oil", "from olive", 50, "oil").getValue();
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(oliveOilID, 1);
            items.TryAdd(sodaID, 1);
            int oilQuantity = store.getItemById(oliveOilID).getValue().quantity;
            int sodaQuantity = store.getItemById(sodaID).getValue().quantity;
            store.rollBackPurchase(items);
            Item oilFromStorage = store.getItemById(oliveOilID).getValue();
            Item sodaFromStorage = store.getItemById(sodaID).getValue();
            Assert.AreEqual(oilQuantity + 1, oilFromStorage.quantity);
            Assert.AreEqual(sodaQuantity + 1, sodaFromStorage.quantity);
        }

        [TestMethod]
        public void deliverItemsTest()
        {
            Assert.IsFalse(store.deliverItems("Holon", new ConcurrentDictionary<int, int>()).getTag());
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(sodaID, 10);
            Assert.IsTrue(store.deliverItems("Holon", items).getTag());
        }

        [TestMethod]
        public void addNewStoreSellerTest()
        {
            ConcurrentLinkedList<Permissions> perms = new ConcurrentLinkedList<Permissions>();
            perms.TryAdd(Permissions.AllPermissions);
            SellerPermissions aviTheSeller = SellerPermissions.getSellerPermissions(new User("avi"), this.store, new User("admin"), perms);
            RegularResult addNewStoreSellerBool1 = store.addNewStoreSeller(aviTheSeller);
            Assert.IsTrue(addNewStoreSellerBool1.getTag());
            RegularResult addNewStoreSellerBool2 = store.addNewStoreSeller(aviTheSeller);
            Assert.IsFalse(addNewStoreSellerBool2.getTag());
        }

        [TestMethod]
        public void removeStoreSellerTest()
        {
            ConcurrentLinkedList<Permissions> perms = new ConcurrentLinkedList<Permissions>();
            perms.TryAdd(Permissions.AllPermissions);
            SellerPermissions aviTheSeller = SellerPermissions.getSellerPermissions(new User("avi"), this.store, new User("admin"), perms);
            store.addNewStoreSeller(aviTheSeller);
            RegularResult removeStoreSellerBool1 = store.removeStoreSeller("avi");
            Assert.IsTrue(removeStoreSellerBool1.getTag());
            RegularResult removeStoreSellerBool2 = store.removeStoreSeller("avi");
            Assert.IsFalse(removeStoreSellerBool2.getTag());
        }

        [TestMethod]
        public void getStoreSellerPermissionsTest()
        {
            ConcurrentLinkedList<Permissions> perms = new ConcurrentLinkedList<Permissions>();
            perms.TryAdd(Permissions.AllPermissions);
            SellerPermissions aviTheSeller = SellerPermissions.getSellerPermissions(new User("avi"), this.store, new User("admin"), perms);
            store.addNewStoreSeller(aviTheSeller);
            ResultWithValue<SellerPermissions> result = store.getStoreSellerPermissions("avi");
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(aviTheSeller, result.getValue());
        }

        [TestMethod]
        public void getStoreOfficialsInfoTest()
        {
            ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>> info = store.getStoreOfficialsInfo();
            ConcurrentLinkedList<Permissions> perms = new ConcurrentLinkedList<Permissions>();
            perms.TryAdd(Permissions.AllPermissions);
            int numOfRecords = info.Count;
            Assert.AreEqual(numOfRecords, 1);
            SellerPermissions aviTheSeller = SellerPermissions.getSellerPermissions(new User("avi"), this.store, new User("admin"), perms);
            store.addNewStoreSeller(aviTheSeller);
            Assert.AreEqual(store.getStoreOfficialsInfo().Count, 2);
        }
    }
}