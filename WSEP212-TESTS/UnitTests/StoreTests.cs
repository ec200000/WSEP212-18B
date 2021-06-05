using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WSEP212;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class StoreTests
    {
        private static Store store;
        private static int sodaID;
        private static User user;

        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
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
            
            UserRepository.Instance.initRepo();
            User admin = new User("admin", 80);
            UserRepository.Instance.insertNewUser(admin, "123456");
            user = new User("avi", 20);
            UserRepository.Instance.insertNewUser(user, "123456");
            
            ConcurrentLinkedList<PurchaseType> purchaseRoutes = new ConcurrentLinkedList<PurchaseType>();
            purchaseRoutes.TryAdd(PurchaseType.ImmediatePurchase);
            ResultWithValue<int> addStoreRes = StoreRepository.Instance.addStore("Mega", "Holon", new SalePolicyMock(), new PurchasePolicyMock(), admin);
            store = StoreRepository.Instance.getStore(addStoreRes.getValue()).getValue();
            sodaID = store.addItemToStorage(200, "soda-stream", "great drink", 150, ItemCategory.Drinks).getValue();
        }

        public void cleanUpStoreSeller(string sellerName)
        {
            store.removeStoreSeller(sellerName);
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
            Item bamba = new Item(3, "bamba", "tasty snack", 4.8, ItemCategory.Snacks);
            RegularResult available = store.isAvailableInStorage(bamba.itemID, 1);
            Assert.IsFalse(available.getTag());
            available = store.isAvailableInStorage(sodaID, 205);
            Assert.IsFalse(available.getTag());
        }

        [TestMethod]
        public void addItemToStorageTest()
        {
            ResultWithValue<int> itemIDRes = store.addItemToStorage(40, "nike bag", "sport bag", 220, ItemCategory.Sport);
            Assert.IsTrue(itemIDRes.getTag());
        }

        [TestMethod]
        public void addItemToStorageNegPriceTest()
        {
            ResultWithValue<int> itemIDRes = store.addItemToStorage(60, "bisli", "monosodium glutamate", -4.8, ItemCategory.Snacks);
            Assert.IsFalse(itemIDRes.getTag());
        }

        [TestMethod]
        public void addItemToStorageEmptyNameTest()
        {
            ResultWithValue<int> itemIDRes = store.addItemToStorage(60, "", "monosodium glutamate", 4.8, ItemCategory.Snacks);
            Assert.IsFalse(itemIDRes.getTag());
        }

        [TestMethod]
        public void addItemToStorageNegQuantityTest()
        {
            ResultWithValue<int> itemIDRes = store.addItemToStorage(-1, "bisli", "monosodium glutamate", 4.8, ItemCategory.Snacks);
            Assert.IsFalse(itemIDRes.getTag());
        }

        [TestMethod]
        public void removeItemFromStorageTest()
        {
            int bisliID = store.addItemToStorage(60, "bisli", "monosodium glutamate", 4.8, ItemCategory.Snacks).getValue();
            RegularResult removed = store.removeItemFromStorage(bisliID);
            Assert.IsTrue(removed.getTag());
            Assert.IsFalse(store.isAvailableInStorage(bisliID, 1).getTag());
        }

        [TestMethod]
        public void removeItemNotInStorageTest()
        {
            Item cola = new Item(40, "cola", "lot of sugar", 7, ItemCategory.Drinks);
            RegularResult removed = store.removeItemFromStorage(cola.itemID);
            Assert.IsFalse(removed.getTag());
        }

        [TestMethod]
        public void changeItemQuantityTest()
        {
            store.changeItemQuantity(sodaID, 4);
            int quantity = store.getItemById(sodaID).getValue().quantity;
            Assert.AreEqual(4, quantity);
        }

        [TestMethod]
        public void changeItemQuantityNotExistTest()
        {
            Item tesla = new Item(3, "tesla-3", "great car", 150000, ItemCategory.Electronics);
            RegularResult changed = store.changeItemQuantity(tesla.itemID, 150);
            Assert.IsFalse(changed.getTag());
        }

        [TestMethod]
        public void editItemTest()
        {
            store.editItem(sodaID, "soda-stream", "great drink", 1000.0, ItemCategory.Drinks, 3);
            double price = store.getItemById(sodaID).getValue().price;
            Assert.AreEqual(1000.0, price);
            int teslaID = store.addItemToStorage(3, "tesla-3", "great car", 150000, ItemCategory.Electronics).getValue();
            RegularResult edited = store.editItem(teslaID, "tesla-3", "nice car", 200000, ItemCategory.Electronics, 5);
            Assert.IsTrue(edited.getTag());   
        }

        [TestMethod]
        public void editItemEmptyNameTest()
        {
            RegularResult edited = store.editItem(sodaID, "", "great drink", 1000.0, ItemCategory.Drinks, 3);
            Assert.IsFalse(edited.getTag());
        }

        [TestMethod]
        public void editItemNegPriceTest()
        {
            RegularResult edited = store.editItem(sodaID, "soda-stream", "great drink", -1000.0, ItemCategory.Drinks, 3);
            Assert.IsFalse(edited.getTag());
        }

        [TestMethod()]
        public void purchaseItemsTest()
        {
            int oliveOilID = store.addItemToStorage(10, "olive-oil", "from olive", 50, ItemCategory.Drinks).getValue();
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(sodaID, 1);
            items.TryAdd(oliveOilID, 1);
            ConcurrentDictionary<int, double> itemsPrices = new ConcurrentDictionary<int, double>();
            itemsPrices.TryAdd(sodaID, 150);
            itemsPrices.TryAdd(oliveOilID, 50);
            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
            StoreRepository.Instance.stores[store.storeID].deliverySystem = DeliverySystemMock.Instance;

            ResultWithValue<ConcurrentDictionary<int, double>> totalPrice = store.purchaseItems(user, items, itemsPrices);
            Assert.IsTrue(totalPrice.getTag());
            Assert.AreEqual(2, totalPrice.getValue().Count);
            Assert.IsTrue(totalPrice.getValue().ContainsKey(oliveOilID));
            Assert.AreEqual(50, totalPrice.getValue()[oliveOilID]);
            Assert.IsTrue(totalPrice.getValue().ContainsKey(sodaID));
            Assert.AreEqual(150, totalPrice.getValue()[sodaID]);
        }

        [TestMethod()]
        public void applySalesPolicyTest()
        {
            ConcurrentDictionary<Item, int> items = new ConcurrentDictionary<Item, int>();
            ConcurrentDictionary<int, double> itemsPrices = new ConcurrentDictionary<int, double>();
            items.TryAdd(store.getItemById(sodaID).getValue(), 1);
            itemsPrices.TryAdd(sodaID, 150);
            PurchaseDetails purchaseDetails = new PurchaseDetails(user, items, itemsPrices);
            ConcurrentDictionary<Item, double> objItems = new ConcurrentDictionary<Item, double>();
            objItems.TryAdd(store.getItemById(sodaID).getValue(), 150);
            ConcurrentDictionary<int, double> itemPrices = store.applySalesPolicy(objItems, purchaseDetails);
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
            ConcurrentDictionary<int, double> itemsPrices = new ConcurrentDictionary<int, double>();
            itemsPrices.TryAdd(sodaID, 150);
            PurchaseDetails purchaseDetails = new PurchaseDetails(user, items, itemsPrices);
            RegularResult approved = store.applyPurchasePolicy(purchaseDetails);
            Assert.IsTrue(approved.getTag());
        }

        [TestMethod]
        public void purchaseItemsAvailableTest()
        {
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(sodaID, 1);
            RegularResult successfulPurchase = store.purchaseItemsIfAvailable(items);
            Assert.IsTrue(successfulPurchase.getTag());
        }

        [TestMethod]
        public void purchaseItemsNotAvailableTest()
        {
            int oliveOilID = store.addItemToStorage(10, "olive-oil", "from olive", 50, ItemCategory.Drinks).getValue();
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
            int oliveOilID = store.addItemToStorage(10, "olive-oil", "from olive", 50, ItemCategory.Drinks).getValue();
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(oliveOilID, 1);
            int oilQuantity = store.getItemById(oliveOilID).getValue().quantity;
            store.rollBackPurchase(items);
            Item oilFromStorage = store.getItemById(oliveOilID).getValue();
            Assert.AreEqual(oilQuantity + 1, oilFromStorage.quantity);
        }

        [TestMethod]
        public void deliverItemsTest()
        {
            StoreRepository.Instance.stores[store.storeID].deliverySystem = DeliverySystemMock.Instance;
            DeliveryParameters deliveryParameters = new DeliveryParameters("guest", "Holon", "Holon", "Israel", "5552601");
            Assert.AreNotEqual(-1, store.deliverItems(deliveryParameters));
        }

        [TestMethod]
        public void addNewStoreSellerTest()
        {
            ConcurrentLinkedList<Permissions> perms = new ConcurrentLinkedList<Permissions>();
            perms.TryAdd(Permissions.AllPermissions);
            SellerPermissions aviTheSeller = SellerPermissions.getSellerPermissions(user.userName, store.storeID, "admin", perms);
            RegularResult addNewStoreSellerBool2 = store.addNewStoreSeller(aviTheSeller);
            Assert.IsTrue(addNewStoreSellerBool2.getTag());

            cleanUpStoreSeller(user.userName);
        }

        [TestMethod]
        public void removeStoreSellerTest()
        {
            ConcurrentLinkedList<Permissions> perms = new ConcurrentLinkedList<Permissions>();
            perms.TryAdd(Permissions.AllPermissions);
            SellerPermissions aviTheSeller = SellerPermissions.getSellerPermissions(user.userName, store.storeID, "admin", perms);
            RegularResult removeStoreSellerBool1 = store.removeStoreSeller(user.userName);
            Assert.IsTrue(removeStoreSellerBool1.getTag());
            RegularResult removeStoreSellerBool2 = store.removeStoreSeller(user.userName);
            Assert.IsFalse(removeStoreSellerBool2.getTag());
        }

        [TestMethod]
        public void getStoreSellerPermissionsTest()
        {
            ConcurrentLinkedList<Permissions> perms = new ConcurrentLinkedList<Permissions>();
            perms.TryAdd(Permissions.AllPermissions);
            SellerPermissions aviTheSeller = SellerPermissions.getSellerPermissions(user.userName, store.storeID, "admin", perms);
            ResultWithValue<SellerPermissions> result = store.getStoreSellerPermissions("avi");
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(aviTheSeller, result.getValue());
            
            cleanUpStoreSeller(user.userName);
        }

        [TestMethod]
        public void getStoreOfficialsInfoTest()
        {
            ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>> info = store.getStoreOfficialsInfo();
            ConcurrentLinkedList<Permissions> perms = new ConcurrentLinkedList<Permissions>();
            perms.TryAdd(Permissions.AllPermissions);
            int numOfRecords = info.Count;
            Assert.AreEqual(numOfRecords, 1);
            SellerPermissions aviTheSeller = SellerPermissions.getSellerPermissions(user.userName, store.storeID, "admin", perms);
            store.addNewStoreSeller(aviTheSeller);
            Assert.AreEqual(store.getStoreOfficialsInfo().Count, 2);
            
            cleanUpStoreSeller(user.userName);
        }
    }
}