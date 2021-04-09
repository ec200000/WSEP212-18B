using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.Result;
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
            systemManager = new User("big manager"); //system manager
            systemManager.changeState(new SystemManagerState(systemManager));
            
            user1 = new User("a"); //guest
            user2 = new User("b"); //logged
            user3 = new User("r"); //logged
            user2.changeState(new LoggedBuyerState(user2));
            user3.changeState(new LoggedBuyerState(user3));
            UserRepository.Instance.users.TryAdd(user1, false);
            UserRepository.Instance.usersInfo.TryAdd("a", Authentication.Instance.encryptPassword("123"));
            UserRepository.Instance.users.TryAdd(user2, true);
            UserRepository.Instance.usersInfo.TryAdd("b", Authentication.Instance.encryptPassword("123456"));
            UserRepository.Instance.users.TryAdd(user3, true);
            UserRepository.Instance.usersInfo.TryAdd("r", Authentication.Instance.encryptPassword("1234"));
            UserRepository.Instance.users.TryAdd(systemManager, true);
            UserRepository.Instance.usersInfo.TryAdd("big manager", Authentication.Instance.encryptPassword("78910"));

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
            UserRepository.Instance.users.Clear();
            UserRepository.Instance.usersInfo.Clear();
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
            Thread t1 = new Thread(() => systemController.register("iris","12345"));
            Thread t2 = new Thread(() => systemController.register("iris","12345"));
            Thread t3 = new Thread(() => systemController.register("itay","12345"));

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();
            
            Assert.AreEqual(6, UserRepository.Instance.users.Count); //4 is from test init, 2 from now
            Assert.AreEqual(6, UserRepository.Instance.usersInfo.Count); //4 is from test init, 2 from now
            Assert.IsNotNull(UserRepository.Instance.findUserByUserName("iris"));
            Assert.IsNotNull(UserRepository.Instance.findUserByUserName("itay"));
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
                catch (NotImplementedException e)
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
                catch (NotImplementedException e)
                {
                    res2 = new Failure("not implemented exception");
                }
            });

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Assert.IsTrue((!res1.getTag() && res2.getTag()) || (res1.getTag() && !res2.getTag())); //only one can be successful
            Assert.AreEqual(4, UserRepository.Instance.users.Count);
            Assert.AreEqual(4, UserRepository.Instance.usersInfo.Count);
            Assert.IsTrue(UserRepository.Instance.users[user1]);
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
                catch (NotImplementedException e)
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
                catch (NotImplementedException e)
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
                catch (NotImplementedException e)
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
            Assert.AreEqual(4, UserRepository.Instance.users.Count);
            Assert.AreEqual(4, UserRepository.Instance.usersInfo.Count);
            Assert.IsFalse(UserRepository.Instance.users[user2]);
        }

        [TestMethod]
        public void addItemToStorageTest()
        {
            SystemController systemController = new SystemController();
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok"), res3 = new Ok("ok");
            
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.addItemToShoppingCart("a",store.storeID, item.itemID, 2);
                }
                catch (NotImplementedException e)
                {
                    res1 = new Failure("not implemented exception");
                }
                
            });
            Thread t2 = new Thread(() =>
            {
                try
                {
                    //trying to take the remaining of the product
                    res2 = systemController.addItemToShoppingCart("b",store.storeID, item.itemID, 28);
                }
                catch (NotImplementedException e)
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
                catch (NotImplementedException e)
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
            Assert.AreEqual(1, UserRepository.Instance.findUserByUserName("b").getValue().shoppingCart.shoppingBags.Count);
            Assert.AreEqual(1, UserRepository.Instance.findUserByUserName("a").getValue().shoppingCart.shoppingBags.Count);
            Assert.AreEqual(0, UserRepository.Instance.findUserByUserName("r").getValue().shoppingCart.shoppingBags.Count);
        }
    }
}