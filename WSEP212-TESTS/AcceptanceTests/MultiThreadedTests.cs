using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class MultiThreadedTests
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
            //Authentication.Instance.usersInfo.TryAdd("a", Authentication.Instance.encryptPassword("123"));
            UserRepository.Instance.users.TryAdd(user2, true);
            //Authentication.Instance.usersInfo.TryAdd("b", Authentication.Instance.encryptPassword("123456"));
            UserRepository.Instance.users.TryAdd(user3, true);
            //Authentication.Instance.usersInfo.TryAdd("r", Authentication.Instance.encryptPassword("1234"));
            UserRepository.Instance.users.TryAdd(systemManager, true);
           // Authentication.Instance.usersInfo.TryAdd("big manager", Authentication.Instance.encryptPassword("78910"));
            
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
            
            ConcurrentLinkedList<Permissions> per = new ConcurrentLinkedList<Permissions>();
            per.TryAdd(Permissions.AllPermissions);
            SellerPermissions sellerPermissions = SellerPermissions.getSellerPermissions(user2,store,null,per);
            store.addNewStoreSeller(sellerPermissions);
            user2.sellerPermissions.TryAdd(sellerPermissions);
            StoreRepository.Instance.stores.TryAdd(store.storeID, store);
        }
        
        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            //Authentication.Instance.usersInfo.Clear();
            StoreRepository.Instance.stores.Clear();
            user1.purchases.Clear();
            user2.purchases.Clear();
            user3.purchases.Clear();
            user1.shoppingCart.shoppingBags.Clear();
            user2.shoppingCart.shoppingBags.Clear();
            user3.shoppingCart.shoppingBags.Clear();
            user1.sellerPermissions = null;
            user2.sellerPermissions = null;
            user3.sellerPermissions = null;
        }

        [TestMethod]
        public void registerTest()
        {
            SystemController systemController = new SystemController();
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok"), res3 = new Ok("ok");
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.register("iris", "12345");
                }
                catch (NotImplementedException)
                {
                    res1 = new Failure("not implemented exception");
                }
            });
            Thread t2 = new Thread(() =>
            {
                try
                {
                    res2 = systemController.register("iris", "12345");
                }
                catch (NotImplementedException)
                {
                    res2 = new Failure("not implemented exception");
                }
            });
            Thread t3 = new Thread(() =>
            {
                try
                {
                    res3 = systemController.register("itay", "12345");
                }
                catch (NotImplementedException)
                {
                    res3 = new Failure("not implemented exception");
                }
            });

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();

            Assert.IsTrue(res3.getTag());
            Assert.IsTrue((!res1.getTag() && res2.getTag()) || (res1.getTag() && !res2.getTag())); //only one can be successful
        }

        [TestMethod]
        public void loginTest()
        {
            SystemController systemController = new SystemController();
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok");
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.login("a", "123");
                }
                catch (NotImplementedException)
                {
                    res1 = new Failure("not implemented exception");
                }
                
            });
            Thread t2 = new Thread(() =>
            {
                try
                {
                    res2 = systemController.login("a", "123");
                }
                catch (NotImplementedException)
                {
                    res2 = new Failure("not implemented exception");
                }
            });

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Assert.IsTrue((!res1.getTag() && res2.getTag()) || (res1.getTag() && !res2.getTag())); //only one can be successful
        }

        [TestMethod]
        public void logout()
        {
            SystemController systemController = new SystemController();
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok"), res3 = new Ok("ok");
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.logout("a");
                }
                catch (NotImplementedException)
                {
                    res1 = new Failure("not implemented exception");
                }
                
            });
            Thread t2 = new Thread(() =>
            {
                try
                {
                    res2 = systemController.logout("b");
                }
                catch (NotImplementedException)
                {
                    res2 = new Failure("not implemented exception");
                }
            });
            Thread t3 = new Thread(() =>
            {
                try
                {
                    res3 = systemController.logout("b");
                }
                catch (NotImplementedException)
                {
                    res3 = new Failure("not implemented exception");
                }
            });

            t1.Start();
            t2.Start();
            t3.Start();
            
            t1.Join();
            t2.Join();
            t3.Join();
            
            Assert.IsFalse(res1.getTag()); //the user is a guest user - can't logout
            Assert.IsTrue((!res3.getTag() && res2.getTag()) || (res3.getTag() && !res2.getTag())); //only one can be successful
        }

        [TestMethod]
        public void addItemToShoppingCartTest()
        {
            SystemController systemController = new SystemController();
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok"), res3 = new Ok("ok");
            
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.addItemToShoppingCart("a",store.storeID, item.itemID, 2);
                }
                catch (NotImplementedException)
                {
                    res1 = new Failure("not implemented exception");
                }
                
            });
            Thread t2 = new Thread(() =>
            {
                try
                {
                    res2 = systemController.addItemToShoppingCart("b",store.storeID, item.itemID, 28);
                }
                catch (NotImplementedException)
                {
                    res2 = new Failure("not implemented exception");
                }
            });
            Thread t3 = new Thread(() =>
            {
                try
                {
                    res3 = systemController.addItemToShoppingCart("r",store.storeID, item.itemID, 58);
                }
                catch (NotImplementedException)
                {
                    res3 = new Failure("not implemented exception");
                }
            });

            t1.Start();
            t2.Start();
            t3.Start();
            
            t1.Join();
            t2.Join();
            t3.Join();
            
            Assert.IsTrue(res1.getTag()); 
            Assert.IsTrue(res2.getTag()); 
            Assert.IsFalse(res3.getTag());
        }

        [TestMethod]
        public void removeItemFromShoppingCart()
        {
            SystemController systemController = new SystemController();
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok"), res3 = new Ok("ok");
            
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.addItemToShoppingCart("a",store.storeID, item.itemID, 2);
                    if (res1.getTag())
                        res1 = systemController.removeItemFromShoppingCart("a", store.storeID, item.itemID);
                }
                catch (NotImplementedException)
                {
                    res1 = new Failure("not implemented exception");
                }
                
            });
            Thread t2 = new Thread(() =>
            {
                try
                {
                    res2 = systemController.addItemToShoppingCart("b",store.storeID, item.itemID, 28);
                    if (res2.getTag())
                        res2 = systemController.removeItemFromShoppingCart("b", store.storeID, -1);
                }
                catch (NotImplementedException)
                {
                    res2 = new Failure("not implemented exception");
                }
            });
            Thread t3 = new Thread(() =>
            {
                try
                {
                    res3 = systemController.addItemToShoppingCart("r",store.storeID, item.itemID, 12);
                    if(res3.getTag())
                        res3 = systemController.removeItemFromShoppingCart("r", -1, item.itemID);
                        
                }
                catch (NotImplementedException)
                {
                    res3 = new Failure("not implemented exception");
                }
            });

            t1.Start();
            t2.Start();
            t3.Start();
            
            t1.Join();
            t2.Join();
            t3.Join();
            
            Assert.IsTrue(res1.getTag()); 
            Assert.IsFalse(res2.getTag()); 
            Assert.IsFalse(res3.getTag());
        }

        [TestMethod]
        public void purchaseItemsTest()
        {
            SystemController systemController = new SystemController();
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok"), res3 = new Ok("ok");
            
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.addItemToShoppingCart("a",store.storeID, item.itemID, 2);
                    if (res1.getTag())
                        res1 = systemController.purchaseItems("a", "ashdod");
                }
                catch (NotImplementedException)
                {
                    res1 = new Failure("not implemented exception");
                }
                
            });
            Thread t2 = new Thread(() =>
            {
                try
                {
                    res2 = systemController.addItemToShoppingCart("b",store.storeID, item.itemID, 28);
                    if (res2.getTag())
                        res2 = systemController.purchaseItems("b", "ness ziona");
                }
                catch (NotImplementedException)
                {
                    res2 = new Failure("not implemented exception");
                }
            });
            Thread t3 = new Thread(() =>
            {
                try
                {
                    res3 = systemController.addItemToShoppingCart("r",store.storeID, item.itemID, 28);
                    if(res3.getTag())
                        res3 = systemController.purchaseItems("r", "holon");

                }
                catch (NotImplementedException)
                {
                    res3 = new Failure("not implemented exception");
                }
            });

            t1.Start();
            t2.Start();
            t3.Start();
            
            t1.Join();
            t2.Join();
            t3.Join();
            
            //only b or r will be able to purchase the items because they are taking the last products
            Assert.IsTrue((!res3.getTag() && res2.getTag()) || (res3.getTag() && !res2.getTag())); //only one can be successful
            Assert.IsTrue(res1.getTag());
        }

        [TestMethod]
        public void openStoreTest()
        {
            SystemController systemController = new SystemController();
            ResultWithValue<int> res1 = new OkWithValue<int>("ok",-1), res2 = new OkWithValue<int>("ok",-1), res3 = new OkWithValue<int>("ok",-1);
            
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.openStore("a", "no", "nessi", "DEFAULT", "DEFAULT");
                }
                catch (NotImplementedException)
                {
                    res1 = new FailureWithValue<int>("not implemented exception",-1);
                }
                
            });
            Thread t2 = new Thread(() =>
            {
                try
                {
                    res2 = systemController.openStore("b", "HAMAMA", "ashdod", "DEFAULT", "DEFAULT");
                }
                catch (NotImplementedException)
                {
                    res2 = new FailureWithValue<int>("not implemented exception",-1);
                }
            });
            Thread t3 = new Thread(() =>
            {
                try
                {
                    res3 = systemController.openStore("r", "HAMAMA", "ashdod", "DEFAULT", "DEFAULT");
                }
                catch (NotImplementedException)
                {
                    res3 = new FailureWithValue<int>("not implemented exception",-1);
                }
            });

            t1.Start();
            t2.Start();
            t3.Start();
            
            t1.Join();
            t2.Join();
            t3.Join();
            
            Assert.IsFalse(res1.getTag()); //a is guest user - no permission to perform this action
            //b and r trying to open a store with the same name in the same place - only one will be able to perform this action
            Assert.IsTrue((!res3.getTag() && res2.getTag()) || (res3.getTag() && !res2.getTag())); //only one can be successful
        }

        [TestMethod]
        public void deleteItemAndTryToBuyItTest()
        {
            SystemController systemController = new SystemController();
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok");
            
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.removeItemFromStorage("b", store.storeID, item.itemID);
                }
                catch (NotImplementedException)
                {
                    res1 = new Failure("not implemented exception");
                }
                
            });
            Thread t2 = new Thread(() =>
            {
                try
                {
                    res2 = systemController.addItemToShoppingCart("r",store.storeID, item.itemID, 28);
                    if (res2.getTag())
                        res2 = systemController.purchaseItems("r", "ness ziona");
                }
                catch (NotImplementedException)
                {
                    res2 = new Failure("not implemented exception");
                }
            });

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
            
            Assert.IsTrue((res1.getTag() && res2.getTag()) || (res1.getTag() && !res2.getTag()));
        }

        [TestMethod]
        public void selectTheSameUserToBeAManager()
        {
            User user = new User("moshe"); //another store owner
            user.changeState(new LoggedBuyerState(user));
            UserRepository.Instance.users.TryAdd(user, true);
            //Authentication.Instance.usersInfo.TryAdd("moshe", Authentication.Instance.encryptPassword("1234567"));
            ConcurrentLinkedList<Permissions> per = new ConcurrentLinkedList<Permissions>();
            per.TryAdd(Permissions.AllPermissions);
            SellerPermissions sellerPermissions = SellerPermissions.getSellerPermissions(user,store,user2,per);
            store.addNewStoreSeller(sellerPermissions);
            user.sellerPermissions.TryAdd(sellerPermissions);
            
            SystemController systemController = new SystemController();
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok");
            
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.appointStoreManager("b", "r", store.storeID);
                }
                catch (NotImplementedException)
                {
                    res1 = new Failure("not implemented exception");
                }
                
            });
            Thread t2 = new Thread(() =>
            {
                try
                {
                    res2 = systemController.appointStoreManager("moshe", "r", store.storeID);
                }
                catch (NotImplementedException)
                {
                    res2 = new Failure("not implemented exception");
                }
            });

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
            
            Assert.IsTrue((!res1.getTag() && res2.getTag()) || (res1.getTag() && !res2.getTag()));//only one
        }
    }
}