using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.Result;
using WSEP212.ServiceLayer;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class SingleThreadedTests
    {
        private User user1;
        private User user2;
        private User user3;
        private User systemManager;
        private Store store;
        private Item item;
        
        [TestInitialize]
        public void testInit()
        {
            systemManager = new User("big manager", true); //system manager
            systemManager.changeState(new SystemManagerState(systemManager));
            
            user1 = new User("a"); //guest
            user2 = new User("b"); //logged
            user3 = new User("r"); //logged
            user2.changeState(new LoggedBuyerState(user2));
            user3.changeState(new LoggedBuyerState(user3));
            UserRepository.Instance.users.TryAdd(user1, false);
            Authentication.Instance.usersInfo.TryAdd("a", Authentication.Instance.encryptPassword("123"));
            UserRepository.Instance.users.TryAdd(user2, true);
            Authentication.Instance.usersInfo.TryAdd("b", Authentication.Instance.encryptPassword("123456"));
            UserRepository.Instance.users.TryAdd(user3, true);
            Authentication.Instance.usersInfo.TryAdd("r", Authentication.Instance.encryptPassword("1234"));
            UserRepository.Instance.users.TryAdd(systemManager, true);
            Authentication.Instance.usersInfo.TryAdd("big manager", Authentication.Instance.encryptPassword("78910"));
            
            ConcurrentLinkedList<PurchaseType> purchaseRoutes = new ConcurrentLinkedList<PurchaseType>();
            purchaseRoutes.TryAdd(PurchaseType.ImmediatePurchase);
            SalesPolicy salesPolicy = new SalesPolicy("DEFAULT", new ConcurrentLinkedList<PolicyRule>());
            PurchasePolicy purchasePolicy = new PurchasePolicy("DEFAULT", purchaseRoutes, new ConcurrentLinkedList<PolicyRule>());
            store = new Store("t","bb",salesPolicy,purchasePolicy,user2);
            
            item = new Item(30, "shoko", "taim retzah!", 12, "milk products");
            ConcurrentDictionary<int, PurchaseType> itemsPurchaseType = new ConcurrentDictionary<int, PurchaseType>();
            itemsPurchaseType.TryAdd(item.itemID, PurchaseType.ImmediatePurchase);
            store.storage.TryAdd(item.itemID, item);
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(item.itemID, 1);
            store.applyPurchasePolicy(user2, items, itemsPurchaseType);
            StoreRepository.Instance.stores.TryAdd(store.storeID, store);
        }
        
        [TestCleanup]
        public void testClean()
        {
            foreach (var u in UserRepository.Instance.users)
            {
                u.Key.purchases.Clear();
                u.Key.shoppingCart.shoppingBags.Clear();
                u.Key.sellerPermissions = null;
            }
            UserRepository.Instance.users.Clear();
            Authentication.Instance.usersInfo.Clear();
            StoreRepository.Instance.stores.Clear();
            user1.purchases.Clear();
            user2.purchases.Clear();
            user3.purchases.Clear();
            systemManager.purchases.Clear();
            user1.shoppingCart.shoppingBags.Clear();
            user2.shoppingCart.shoppingBags.Clear();
            user3.shoppingCart.shoppingBags.Clear();
            systemManager.shoppingCart.shoppingBags.Clear();
            user1.sellerPermissions = null;
            user2.sellerPermissions = null;
            user3.sellerPermissions = null;
            systemManager.sellerPermissions = null;
        }

        [TestMethod]
        public void registerTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.register("abcd", "1234");
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(UserRepository.Instance.users.Count, 5);
            
            RegularResult result2 = controller.register("a", "123");
            Assert.IsFalse(result2.getTag());
            Assert.AreEqual(UserRepository.Instance.users.Count, 5);
        }

        [TestMethod]
        public void checkInitialSystem()
        {
            bool found = false;
            foreach (var u in UserRepository.Instance.users.Keys)
            {
                if (u.state is SystemManagerState)
                    found = true;
            }
            Assert.IsTrue(found);
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException), "user that is logged in, can perform this action again")]
        public void loginTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.login("a", "123");
            Assert.IsTrue(result.getTag());
            UserRepository.Instance.users.TryGetValue(user1, out var res); //is saved as logged in
            Assert.IsTrue(res);
            Assert.AreEqual(UserRepository.Instance.users.Count, 4);
            
            RegularResult result2 = controller.login("b", "123456"); //already logged
            //should  throw exception
            Assert.IsFalse(true); //if it gets here - no exception was thrown
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void logoutTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.logout("b");
            Assert.IsTrue(result.getTag());
            UserRepository.Instance.users.TryGetValue(user2, out var res); //is saved as logged out
            Assert.IsFalse(res);
            Assert.AreEqual(UserRepository.Instance.users.Count, 4);
            
            RegularResult result2 = controller.logout("a"); //not logged in
            //should  throw exception
            Assert.IsFalse(true); //if it gets here - no exception was thrown
        }
        
        [TestMethod]
        public void addItemToShoppingCartTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.addItemToShoppingCart("b",store.storeID, item.itemID, 2); //logged user
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 1);
            user2.shoppingCart.shoppingBags.Clear();
            
            result = controller.addItemToShoppingCart("a",store.storeID, item.itemID, 8); //guest user
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user1.shoppingCart.shoppingBags.Count, 1);
            user1.shoppingCart.shoppingBags.Clear();
            
            result = controller.addItemToShoppingCart("b",store.storeID, item.itemID, 100); //over quantity
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 0);
            
            result = controller.addItemToShoppingCart("b",store.storeID, -1, 1); //item does not exists
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 0);
            
            result = controller.addItemToShoppingCart("b",-1, item.itemID, 1); //store doest not exists
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 0);
        }
        
        [TestMethod]
        public void removeItemFromShoppingCartTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.addItemToShoppingCart("b",store.storeID, item.itemID, 2);//adding an item
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 1);
            
            result = controller.removeItemFromShoppingCart("b",-1, item.itemID); //wrong store id
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 1);
            
            result = controller.removeItemFromShoppingCart("b",store.storeID, -1); //wrong item id
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 1);
            
            result = controller.removeItemFromShoppingCart("b",store.storeID, item.itemID); //removing it
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 0);
            
            Console.WriteLine($"before: {user1.shoppingCart.shoppingBags.Count}");
            result = controller.removeItemFromShoppingCart("a",store.storeID, item.itemID); //nothing in the cart
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user1.shoppingCart.shoppingBags.Count, 1);
        }
        
        [TestMethod]
        public void purchaseItemsTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.addItemToShoppingCart("b",store.storeID, item.itemID, 2);//adding an item
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 1);
            
            result = controller.purchaseItems("a","beer sheva"); //nothing in the cart
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user1.purchases.Count, 0);
            
            result = controller.purchaseItems("b",null); //wrong item id
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.purchases.Count, 0);
            
            result = controller.purchaseItems("b","ashdod");
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user2.purchases.Count, 1);
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void openStoreTest()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> result = controller.openStore("b",store.storeName,store.storeAddress,"DEFAULT","DEFAULT"); 
            Assert.IsFalse(result.getTag()); //store already exists
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 1);
            
            result = controller.openStore(null,store.storeName,store.storeAddress,"DEFAULT","DEFAULT"); 
            Assert.IsFalse(result.getTag()); //null user name
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 1);
            
            result = controller.openStore("b",null,store.storeAddress,"DEFAULT","DEFAULT"); 
            Assert.IsFalse(result.getTag()); //null store name
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 1);
            
            result = controller.openStore("b",store.storeName,null,"DEFAULT","DEFAULT"); 
            Assert.IsFalse(result.getTag()); //null address
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 1);
            
            result = controller.openStore("b",store.storeName,store.storeAddress,null,"DEFAULT"); 
            Assert.IsFalse(result.getTag()); //null purchase policy
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 1);
            
            result = controller.openStore("b",store.storeName,store.storeAddress,"DEFAULT",null); 
            Assert.IsFalse(result.getTag()); //null sales policy
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 1);
            
            result = controller.openStore("b","HAMAMA","Ashdod","DEFAULT","DEFAULT"); 
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(StoreRepository.Instance.stores.Count, 2);
            
            ResultWithValue<int> res = controller.openStore("a","gg", "kk", "default", "default"); //guest cant open
            Assert.IsFalse(true); //should throw exception - if its here, nothing was thrown
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void itemReviewTest()
        {
            SystemController controller = new SystemController();
            RegularResult res = controller.addItemToShoppingCart("b", store.storeID, item.itemID, 2);
            Assert.IsTrue(res.getTag());
            res = controller.purchaseItems("b", "ashdod");
            Assert.IsTrue(res.getTag());
            RegularResult result = controller.itemReview("b","wow",item.itemID,store.storeID); //logged
            Assert.IsTrue(result.getTag()); 
            Assert.AreEqual(item.reviews.Count, 1);

            result = controller.itemReview(null,"boo",item.itemID,store.storeID); 
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(item.reviews.Count, 1);
            
            result = controller.itemReview("b",null,item.itemID,store.storeID);
            Assert.IsFalse(result.getTag()); 
            Assert.AreEqual(item.reviews.Count, 1);
            
            controller.itemReview("a","boo",item.itemID,store.storeID); //guest user can't perform this action
            Assert.IsFalse(true);
            
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void addItemToStorageTest()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> storeId = controller.openStore("b", "HAMAMA", "Beer Sheva", "deault", "default");
            Assert.IsTrue(storeId.getTag());
            Store store = StoreRepository.Instance.stores[storeId.getValue()];
            ItemDTO itemDto = new ItemDTO(store.storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, string>(), 1.34, "snacks");
            
            ResultWithValue<int> res = controller.addItemToStorage("b", store.storeID, itemDto);
            Assert.IsTrue(res.getTag());
            Assert.AreNotEqual(-1,res.getValue());
            Assert.AreEqual(1, store.storage.Count);
            
            res = controller.addItemToStorage("b", store.storeID, itemDto); //already in storage
            Assert.IsTrue(res.getTag());
            Assert.AreNotEqual(-1,res.getValue());
            Assert.AreEqual(1, store.storage.Count);
            
            res = controller.addItemToStorage("b", store.storeID, null);
            Assert.IsFalse(res.getTag());
            Assert.AreEqual(-1,res.getValue());
            Assert.AreEqual(1, store.storage.Count);
            
            controller.addItemToStorage("a", store.storeID, itemDto);//guest user - can't perform this action
            
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void removeItemFromStorageTest()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> storeId = controller.openStore("b", "HAMAMA", "Beer Sheva", "deault", "default");
            Assert.IsTrue(storeId.getTag());
            Store store = StoreRepository.Instance.stores[storeId.getValue()];
            ItemDTO itemDto = new ItemDTO(store.storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, string>(), 1.34, "snacks");
            
            ResultWithValue<int> res = controller.addItemToStorage("b", store.storeID, itemDto);
            Assert.IsTrue(res.getTag());
            Assert.AreNotEqual(-1,res.getValue());
            itemDto.itemID = res.getValue();
            Assert.AreEqual(1, store.storage.Count);
            
            RegularResult result = controller.removeItemFromStorage("b", store.storeID, itemDto.itemID);
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(0, store.storage.Count);
            
            result = controller.removeItemFromStorage("b", store.storeID, -1); //no such item
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(0, store.storage.Count);
            
            result = controller.removeItemFromStorage("b", -1, item.itemID); //no such store
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(2, StoreRepository.Instance.stores.Count);
            
            controller.removeItemFromStorage("a", store.storeID, itemDto.itemID);//guest user - can't perform this action
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void editItemDetailsTest()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> storeId = controller.openStore("b", "HAMAMA", "Beer Sheva", "deault", "default");
            Assert.IsTrue(storeId.getTag());
            Store store = StoreRepository.Instance.stores[storeId.getValue()];
            ItemDTO itemDto = new ItemDTO(store.storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, string>(), 1.34, "snacks");
            
            ResultWithValue<int> res = controller.addItemToStorage("b", store.storeID, itemDto);
            Assert.IsTrue(res.getTag());
            Assert.AreNotEqual(-1,res.getValue());
            itemDto.itemID = res.getValue();
            Assert.AreEqual(1, store.storage.Count);

            itemDto.price = 7.9;
            RegularResult result = controller.editItemDetails("b", store.storeID, itemDto);
            Assert.IsTrue(res.getTag());
            Assert.AreEqual(7.9, StoreRepository.Instance.stores[store.storeID].storage[itemDto.itemID].price);
            
            result = controller.editItemDetails("b", store.storeID, null);
            Assert.IsFalse(result.getTag());

            controller.editItemDetails("a", store.storeID, itemDto); //guest user - can't perform this action
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void appointStoreManagerTest()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> storeId = controller.openStore("b", "HAMAMA", "Beer Sheva", "deault", "default");
            Assert.IsTrue(storeId.getTag());
            Store store = StoreRepository.Instance.stores[storeId.getValue()];

            RegularResult res = controller.appointStoreManager("b", "r", store.storeID);
            Assert.IsTrue(res.getTag());
            Assert.IsTrue(user3.sellerPermissions.size == 1); //new seller permission was added
            Assert.AreEqual(Permissions.GetOfficialsInformation, user3.sellerPermissions.First.Value.permissionsInStore.First.Value);
            
            res = controller.appointStoreManager("b", "r", store.storeID);
            Assert.IsFalse(res.getTag());
            
            res = controller.appointStoreManager("b", "r", -1);
            Assert.IsFalse(res.getTag());
            Assert.IsTrue(user3.sellerPermissions.size == 1); 
            
            res = controller.appointStoreManager("b", "no such user", store.storeID);
            Assert.IsFalse(res.getTag());
            
            controller.appointStoreManager("a", "r", store.storeID); //cant perform this action
            Assert.IsFalse(true);

        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void appointStoreOwnerTest()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> storeId = controller.openStore("b", "HAMAMA", "Beer Sheva", "deault", "default");
            Assert.IsTrue(storeId.getTag());
            Store store = StoreRepository.Instance.stores[storeId.getValue()];

            RegularResult res = controller.appointStoreOwner("b", "r", -1);
            Assert.IsFalse(res.getTag());
            Assert.IsTrue(user3.sellerPermissions.size == 0);
            
            res = controller.appointStoreOwner("b", "no such user", store.storeID);
            Assert.IsFalse(res.getTag());
            
            res = controller.appointStoreOwner("b", "r", store.storeID);
            Assert.IsTrue(res.getTag());
            Console.WriteLine($"the size is: {user3.sellerPermissions.size}");
            Assert.IsTrue(user3.sellerPermissions.size == 1); //new seller permission was added - all permissions was given
            Assert.AreEqual(Permissions.AllPermissions, user3.sellerPermissions.First.Value.permissionsInStore.First.Value);
            
            res = controller.appointStoreOwner("b", "r", store.storeID);
            Assert.IsFalse(res.getTag());
            
            controller.appointStoreManager("a", "r", store.storeID); //cant perform this action
            Assert.IsFalse(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void editManagerPermissionsTest()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> storeId = controller.openStore("b", "HAMAMA", "Beer Sheva", "deault", "default");
            Assert.IsTrue(storeId.getTag());
            Store store = StoreRepository.Instance.stores[storeId.getValue()];

            RegularResult res = controller.appointStoreManager("b", "r", store.storeID);
            Assert.IsTrue(res.getTag());
            
            //r is store manager with only GetOfficialsInformation permission
            ConcurrentLinkedList<int> newPermissions = new ConcurrentLinkedList<int>();
            newPermissions.TryAdd((int)Permissions.AppointStoreManager);
            newPermissions.TryAdd((int)Permissions.RemoveStoreManager);
            res = controller.editManagerPermissions("b", "r", newPermissions, store.storeID);
            Assert.IsTrue(res.getTag());
            Assert.AreEqual(1, user3.sellerPermissions.size);
            Assert.AreEqual(2,user3.sellerPermissions.First.Value.permissionsInStore.size);
            Node<Permissions> per = user3.sellerPermissions.First.Value.permissionsInStore.First;
            Assert.AreEqual(Permissions.AppointStoreManager, per.Value);
            per = per.Next;
            Assert.AreEqual(Permissions.RemoveStoreManager, per.Value);
            
            res = controller.editManagerPermissions("b", "no such user", newPermissions, store.storeID);
            Assert.IsFalse(res.getTag());
            
            res = controller.editManagerPermissions("b", "r", new ConcurrentLinkedList<int>(), -1);
            Assert.IsFalse(res.getTag());
            
            controller.editManagerPermissions("a", "r", newPermissions, store.storeID); //no permission to do so
            Assert.IsFalse(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void removeStoreManagerTest()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> storeId = controller.openStore("b", "HAMAMA", "Beer Sheva", "deault", "default");
            Assert.IsTrue(storeId.getTag());
            Store store = StoreRepository.Instance.stores[storeId.getValue()];

            RegularResult res = controller.appointStoreManager("b", "r", store.storeID);
            Assert.IsTrue(res.getTag()); //r is now a store manager

            res = controller.removeStoreManager("b", "no such user", store.storeID);
            Assert.IsFalse(res.getTag());
            
            res = controller.removeStoreManager("b", "r", -1);
            Assert.IsFalse(res.getTag());
            
            res = controller.removeStoreManager("b", "r", store.storeID);
            Assert.IsTrue(res.getTag());
            Assert.AreEqual(0, user3.sellerPermissions.size);
            
            controller.removeStoreManager("a", "r", store.storeID); //no permission to do so
            Assert.IsFalse(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void getOfficialsInformationTest()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> storeId = controller.openStore("b", "HAMAMA", "Beer Sheva", "deault", "default");
            Assert.IsTrue(storeId.getTag());
            Store store = StoreRepository.Instance.stores[storeId.getValue()];

            RegularResult res = controller.appointStoreManager("b", "r", store.storeID);
            Assert.IsTrue(res.getTag()); //r is now a store manager

            ResultWithValue<ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>> result =
                controller.getOfficialsInformation("b", store.storeID);
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(2, result.getValue().Count); //only store owner b and store manager r
            
            result = controller.getOfficialsInformation("r", store.storeID); //as store manager r has permission to perform this action
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(2, result.getValue().Count); //only store owner b and store manager r
            
            controller.getOfficialsInformation("a", store.storeID); //guest user - no permissions to do so
            Assert.IsFalse(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void getStorePurchaseHistory()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> storeId = controller.openStore("b", "HAMAMA", "Beer Sheva", "default", "default");
            Assert.IsTrue(storeId.getTag());
            Store store = StoreRepository.Instance.stores[storeId.getValue()];
            
            ItemDTO itemDto1 = new ItemDTO(store.storeID, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, string>(), 1.34, "snacks");
            ItemDTO itemDto2 = new ItemDTO(store.storeID, 65, "bamba", "best with bisli",
                new ConcurrentDictionary<string, string>(), 1.14, "snacks");
            
            ResultWithValue<int> res = controller.addItemToStorage("b", store.storeID, itemDto1);
            Assert.IsTrue(res.getTag());
            itemDto1.itemID = res.getValue();
            
            res = controller.addItemToStorage("b", store.storeID, itemDto2);
            Assert.IsTrue(res.getTag());
            itemDto2.itemID = res.getValue();
            
            RegularResult result = controller.addItemToShoppingCart("b",store.storeID, itemDto1.itemID, 2); //logged user
            Assert.IsTrue(result.getTag());
            
            result = controller.addItemToShoppingCart("b",store.storeID, itemDto2.itemID, 2); //logged user
            Assert.IsTrue(result.getTag());

            result = controller.purchaseItems("b", "ashdod");
            Assert.IsTrue(result.getTag());

            ResultWithValue<ConcurrentBag<PurchaseInfo>> res2 = controller.getStorePurchaseHistory("b", store.storeID);
            Assert.IsTrue(res2.getTag());
            Assert.AreEqual(1,res2.getValue().Count);
            
            res2 = controller.getStorePurchaseHistory("b", -1);
            Assert.IsFalse(res2.getTag());
            Assert.IsNull(res2.getValue());

            controller.getStorePurchaseHistory("a", store.storeID); //no permission to do so
            Assert.IsFalse(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void getUsersPurchaseHistoryTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.addItemToShoppingCart("b",store.storeID, item.itemID, 2);
            Assert.IsTrue(result.getTag());
            
            result = controller.purchaseItems("b","beer sheva");
            Assert.IsTrue(result.getTag());
            
            result = controller.addItemToShoppingCart("r",store.storeID, item.itemID, 2);
            Assert.IsTrue(result.getTag());
            
            result = controller.purchaseItems("r","ashdod");
            Assert.IsTrue(result.getTag());

            ResultWithValue<ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>>> res =
                controller.getUsersPurchaseHistory(systemManager.userName);
            Console.WriteLine(res.getMessage());
            Assert.IsTrue(res.getTag());
            Assert.AreEqual(4, res.getValue().Count); //4 users - but only 2 of them actually baught something
            Assert.AreEqual(1, res.getValue()["b"].Count);
            Assert.AreEqual(1, res.getValue()["r"].Count);
            
            res = controller.getUsersPurchaseHistory("b"); //only system manager can perform this action
            Assert.IsFalse(true);
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void getStoresPurchaseHistoryTest()
        {
            SystemController controller = new SystemController();
            ResultWithValue<int> result = controller.openStore("b","HAMAMA","Ashdod","DEFAULT","DEFAULT"); 
            Assert.IsTrue(result.getTag());
            int storeId = result.getValue();
            
            ItemDTO itemDto1 = new ItemDTO(storeId, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, string>(), 1.34, "snacks");
            
            result = controller.addItemToStorage("b", storeId, itemDto1);
            Assert.IsTrue(result.getTag());
            itemDto1.itemID = result.getValue();
            
            result = controller.openStore("b","SuperDuper","Ashdod","DEFAULT","DEFAULT"); 
            Assert.IsTrue(result.getTag());
            int storeId2 = result.getValue();
            
            ItemDTO itemDto2 = new ItemDTO(storeId2, 57, "bisli", "very good snack",
                new ConcurrentDictionary<string, string>(), 1.34, "snacks");

            result = controller.addItemToStorage("b", storeId2, itemDto2);
            Assert.IsTrue(result.getTag());
            itemDto2.itemID = result.getValue();
            
            RegularResult res = controller.addItemToShoppingCart("b",storeId, itemDto1.itemID, 2); 
            Assert.IsTrue(res.getTag());
            
            res = controller.addItemToShoppingCart("b",storeId2, itemDto2.itemID, 11);
            Assert.IsTrue(res.getTag());
            
            res = controller.purchaseItems("b","ashdod");
            Assert.IsTrue(res.getTag());

            ResultWithValue<ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>>> finalRes =
                controller.getStoresPurchaseHistory(systemManager.userName);
            Assert.IsTrue(finalRes.getTag());
            Assert.AreEqual(3, finalRes.getValue().Count);
            Assert.AreEqual(1, finalRes.getValue()[storeId].Count);
            Assert.AreEqual(1, finalRes.getValue()[storeId2].Count);
            
            controller.getStoresPurchaseHistory("a"); //no permission to do so
            Assert.IsFalse(true);
        }
        
        [TestMethod]
        public void searchItemsTest()
        {
            SystemController controller = new SystemController();

            ConcurrentDictionary<Item, int> result =
                controller.searchItems(itemName: "shoko");
            Assert.AreEqual(1, result.Count); 
            
            result = controller.searchItems(category: "milk products"); 
            Assert.AreEqual(1, result.Count);
            
            result = controller.searchItems(keyWords: "taim"); 
            Assert.AreEqual(1, result.Count);
            
            result = controller.searchItems(keyWords: "iphone");
            Assert.AreEqual(0, result.Count);

            result = controller.searchItems(itemName: "shoko", category: "iphone");
            Assert.AreEqual(0, result.Count); 
 
            result = controller.searchItems(itemName: "shoko", maxPrice: 10);
            Assert.AreEqual(0, result.Count); 
        }
        
        [TestMethod]
        public void getItemsInStoresInformationTest()
        {
            SystemController controller = new SystemController();

            ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> result =
                controller.getItemsInStoresInformation();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[store].size);
        }
    }
}