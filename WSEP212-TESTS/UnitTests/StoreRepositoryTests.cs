using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using WebApplication;
using WSEP212;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class StoreRepositoryTests
    {
        private static Store store;
        private static int itemAID;
        private static int itemBID;
        private static User user;

        [ClassInitialize]
        public static void SetupAuth(TestContext context)
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
            StoreRepository.Instance.stores.Clear();

            UserRepository.Instance.initRepo();
            user = new User("admin", 80);
            UserRepository.Instance.insertNewUser(user, "123456");
            
            ConcurrentLinkedList<PurchaseType> purchaseRoutes = new ConcurrentLinkedList<PurchaseType>();
            purchaseRoutes.TryAdd(PurchaseType.ImmediatePurchase);
            ResultWithValue<int> storeIdRes = StoreRepository.Instance.addStore("Mega", "Ashdod", new SalePolicyMock(), new PurchasePolicyMock(), user);
            store = StoreRepository.Instance.getStore(storeIdRes.getValue()).getValue();
            itemAID = store.addItemToStorage(500, "black masks", "protects against infection of covid-19", 10, ItemCategory.Health).getValue();
            itemBID = store.addItemToStorage(50, "white masks", "protects against infection of covid-19", 10, ItemCategory.Health).getValue();
        }

        [TestMethod]
        public void addStoreTest()
        {
            ResultWithValue<int> addStoreBool = StoreRepository.Instance.addStore("Mega2", "Holon", new SalePolicyMock(), new PurchasePolicyMock(), user);
            Assert.IsTrue(addStoreBool.getTag());
            StoreRepository.Instance.removeStore(addStoreBool.getValue());
        }

        [TestMethod]
        public void removeStoreTest()
        {
            ResultWithValue<int> addStoreBool = StoreRepository.Instance.addStore("Mega3", "Holon", new SalePolicyMock(), new PurchasePolicyMock(), user);
            int storeID = addStoreBool.getValue();
            
            RegularResult removeStoreBool = StoreRepository.Instance.removeStore(storeID);
            Assert.IsTrue(removeStoreBool.getTag());
        }

        [TestMethod]
        public void getStoreTest()
        {
            int storeID = store.storeID;
            ResultWithValue<Store> getStoreBool = StoreRepository.Instance.getStore(storeID);
            Assert.IsTrue(getStoreBool.getTag());
        }

        [TestMethod]
        public void getStoreNotInRepoTest()
        {
            ResultWithValue<Store> getStoreBool = StoreRepository.Instance.getStore(-1);
            Assert.IsFalse(getStoreBool.getTag());
        }

        [TestMethod]
        public void searchItemByNameTest()
        {
            SearchItems searchItemsByName = new SearchItems(new SearchItemsDTO("masks", "", Double.MinValue, Double.MaxValue, 0));
            ConcurrentDictionary<Item, int> itemsByName = StoreRepository.Instance.searchItem(searchItemsByName);
            Assert.AreEqual(2, itemsByName.Count);
            SearchItems searchItemsByName2 = new SearchItems(new SearchItemsDTO("white", "", Double.MinValue, Double.MaxValue, 0));
            ConcurrentDictionary<Item, int> itemsByName2 = StoreRepository.Instance.searchItem(searchItemsByName2);
            Assert.AreEqual(1, itemsByName2.Count);

            SearchItems searchItemsByKeyWords = new SearchItems(new SearchItemsDTO("", "covid-19", Double.MinValue, Double.MaxValue, 0));
            ConcurrentDictionary<Item, int> itemsByKeyWords = StoreRepository.Instance.searchItem(searchItemsByKeyWords);
            Assert.AreEqual(2, itemsByKeyWords.Count);
        }

        [TestMethod]
        public void searchItemByKeywordsTest()
        {
            SearchItems searchItemsByKeyWords = new SearchItems(new SearchItemsDTO("", "covid-19", Double.MinValue, Double.MaxValue, 0));
            ConcurrentDictionary<Item, int> itemsByKeyWords = StoreRepository.Instance.searchItem(searchItemsByKeyWords);
            Assert.AreEqual(2, itemsByKeyWords.Count);
        }

        [TestMethod]
        public void getAllStoresPurchsesHistoryTest()
        {
            int storeID = store.storeID;
            ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>> storesPurchases = StoreRepository.Instance.getAllStoresPurchsesHistory();
            Assert.AreEqual(1, storesPurchases.Count);
            Assert.AreEqual(0, storesPurchases[storeID].Count);
        }

        [TestMethod]
        public void getStoresAndItemsInfoTest()
        {
            ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> storeItemsInfo = StoreRepository.Instance.getStoresAndItemsInfo();
            Assert.AreEqual(1, storeItemsInfo.Count);
            ConcurrentLinkedList<Item> items = storeItemsInfo[store];
            Node<Item> itemNode = items.First;
            while (itemNode.Value != null)
            {
                Assert.IsTrue(itemNode.Value.itemID == itemAID || itemNode.Value.itemID == itemBID);
                itemNode = itemNode.Next;
            }
        }
    }
}