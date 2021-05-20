using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.ServiceObjectsDTO;
using WSEP212_TEST.UnitTests.UnitTestMocks;
using WSEP212.DomainLayer.PurchaseTypes;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class MultiThreadedTests
    {

        private int storeID;
        SystemController systemController = SystemController.Instance;
        private int itemID;
        //private Item item;
        
        [TestInitialize]
        public void testInit()
        {
            systemController.register("lol", 18, "123456");
            systemController.register("mol", 18, "1234");
            systemController.register("pol", 18, "123");
            RegularResult res = systemController.login("lol", "123456");
            Console.WriteLine(res.getMessage());
            res = systemController.login("mol", "1234");
            Console.WriteLine(res.getMessage());
            ResultWithValue<int> val = systemController.openStore("mol", "t", "bb", "DEFAULT", "DEFAULT");
            ItemDTO item = new ItemDTO(val.getValue(),30, "shoko", "taim retzah!", new ConcurrentDictionary<string, ItemUserReviews>(),12, "milk products");
            ResultWithValue<int> val2 = systemController.addItemToStorage("mol", val.getValue(), item);
            this.storeID = val.getValue();
            this.itemID = val2.getValue();
            HandlePurchases.Instance.paymentSystem = PaymentSystemMock.Instance;
            StoreRepository.Instance.stores[storeID].deliverySystem = DeliverySystemMock.Instance;
            StoreRepository.Instance.stores[storeID].purchasePolicy = new PurchasePolicyMock();
            StoreRepository.Instance.stores[storeID].salesPolicy = new SalePolicyMock();
        }

        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            StoreRepository.Instance.stores.Clear();
        }

        [TestMethod]
        public void registerTest()
        {
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok"), res3 = new Ok("ok");
            for (int i = 0; i < 3; i++)
            {
                Thread t1 = new Thread(() =>
                {
                    try
                    {
                        res1 = systemController.register("iris"+i, 18, "12345");
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
                        res2 = systemController.register("iris"+i, 18, "12345");
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
                        res3 = systemController.register("itay"+i, 18, "12345");
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
            
        }

        [TestMethod]
        public void loginTest()
        {
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok");
            for (int i = 0; i < 3; i++)
            {
                Thread t1 = new Thread(() =>
                {
                    try
                    {
                        res1 = systemController.login("pol", "123");
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
                        res2 = systemController.login("pol", "123");
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
                systemController.logout("pol");
            }
            systemController.login("pol", "123");
        }

        [TestMethod]
        public void logout()
        {
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok"), res3 = new Ok("ok");
            for (int i = 0; i < 3; i++)
            {
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
                        res2 = systemController.logout("mol");
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
                        res3 = systemController.logout("mol");
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
                systemController.login("mol", "1234");
            }
        }

        [TestMethod]
        public void addItemToShoppingCartTest()
        {
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok"), res3 = new Ok("ok");

            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.addItemToShoppingCart("a",storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 12);
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
                    res2 = systemController.addItemToShoppingCart("mol",storeID, itemID, 28, (int)PurchaseType.ImmediatePurchase, 12);
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
                    res3 = systemController.addItemToShoppingCart("lol",storeID, itemID, 58, (int)PurchaseType.ImmediatePurchase, 12);
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
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok"), res3 = new Ok("ok");
            
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.addItemToShoppingCart("a",storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 12);
                    if (res1.getTag())
                        res1 = systemController.removeItemFromShoppingCart("a", storeID, itemID);
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
                    res2 = systemController.addItemToShoppingCart("mol",storeID, itemID, 28, (int)PurchaseType.ImmediatePurchase, 12);
                    if (res2.getTag())
                        res2 = systemController.removeItemFromShoppingCart("mol", storeID, -1);
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
                    res3 = systemController.addItemToShoppingCart("lol",storeID, itemID, 12, (int)PurchaseType.ImmediatePurchase, 12);
                    if(res3.getTag())
                        res3 = systemController.removeItemFromShoppingCart("lol", -1, itemID);
                        
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
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok"), res3 = new Ok("ok");
            ResultWithValue<NotificationDTO> res4 =
                    new OkWithValue<NotificationDTO>("ok", null),
                res5 = new OkWithValue<NotificationDTO>("ok", null),
                res6 = new OkWithValue<NotificationDTO>("ok", null);
            
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.addItemToShoppingCart("a",storeID, itemID, 2, (int)PurchaseType.ImmediatePurchase, 12);
                    if (res1.getTag())
                        res4 = systemController.purchaseItems("a", "ashdod");
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
                    res2 = systemController.addItemToShoppingCart("mol",storeID, itemID, 28, (int)PurchaseType.ImmediatePurchase, 12);
                    if (res2.getTag())
                        res5 = systemController.purchaseItems("mol", "ness ziona");
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
                    res3 = systemController.addItemToShoppingCart("lol",storeID, itemID, 28, (int)PurchaseType.ImmediatePurchase, 12);
                    if(res3.getTag())
                        res6 = systemController.purchaseItems("lol", "holon");

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
            Assert.IsTrue((!res5.getTag() && res6.getTag()) || (res5.getTag() && !res6.getTag())); //only one can be successful
            Assert.IsTrue(res4.getTag());
        }

        [TestMethod]
        public void openStoreTest()
        {
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
                    res2 = systemController.openStore("mol", "HAMAMA", "ashdod", "DEFAULT", "DEFAULT");
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
                    res3 = systemController.openStore("lol", "HAMAMA", "ashdod", "DEFAULT", "DEFAULT");
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
            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok");
            ResultWithValue<NotificationDTO> res3 =
                new OkWithValue<NotificationDTO>("ok", null);
            
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.removeItemFromStorage("mol", storeID, itemID);
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
                    res2 = systemController.addItemToShoppingCart("lol",storeID, itemID, 28, (int)PurchaseType.ImmediatePurchase, 12);
                    if (res2.getTag())
                        res3 = systemController.purchaseItems("lol", "ness ziona");
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
            Assert.IsTrue(res2.getTag());
            Assert.IsTrue((res1.getTag() && res3.getTag()) || (res1.getTag() && !res3.getTag()));
        }

        [TestMethod]
        public void selectTheSameUserToBeAManager()
        {
            systemController.register("moshe", 18, "123");
            systemController.login("moshe", "123");

            systemController.appointStoreOwner("mol", "moshe", storeID);

            RegularResult res1 = new Ok("ok"), res2 = new Ok("ok");
            
            Thread t1 = new Thread(() =>
            {
                try
                {
                    res1 = systemController.appointStoreManager("mol", "lol", storeID);
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
                    res2 = systemController.appointStoreManager("moshe", "lol", storeID);
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