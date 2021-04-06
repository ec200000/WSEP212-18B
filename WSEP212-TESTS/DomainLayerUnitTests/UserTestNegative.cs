using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS
{
    [TestClass]
    public class UserTestNegative
    {
        private User user1;
        private User user2;
        [TestInitialize]
        public void testInit()
        {
            user1 = new User("a"); //guest
            user2 = new User("b"); //logged
            user2.changeState(new LoggedBuyerState(user2));
            UserRepository.Instance.users.TryAdd(user1, false);
            UserRepository.Instance.usersInfo.TryAdd("a", Authentication.Instance.encryptPassword("123"));
            UserRepository.Instance.users.TryAdd(user2, true);
            UserRepository.Instance.usersInfo.TryAdd("b", Authentication.Instance.encryptPassword("123456"));
            
            Store store2 = new Store("t", new SalesPolicy("default", null), new PurchasePolicy("default", null,null), user2);
            
            StoreRepository.Instance.stores.TryAdd(1,store2);
        }
        
        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            UserRepository.Instance.usersInfo.Clear();
            StoreRepository.Instance.stores.Clear();
        }

        [TestMethod]
        public void registerTest()
        {
            User user = new User("c");
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = user.userName;
            list[1] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            Assert.IsTrue((bool)parameters.result);
            Assert.AreEqual(UserRepository.Instance.users.Count, 1);
            
            User user2 = new User(user.userName); //the user can't register again - already in the system
            ThreadParameters parameters2 = new ThreadParameters();
            object[] list2 = new object[2];
            list2[0] = user.userName;
            list2[1] = "5678";
            parameters2.parameters = list2;
            user.register(parameters2);
            Assert.IsFalse((bool)parameters2.result);
            Assert.AreEqual(UserRepository.Instance.users.Count, 1);
        }

        [TestMethod]
        public void loginTest()
        {
            ThreadParameters parameters3 = new ThreadParameters(); //the user is already logged in
            object[] list3 = new object[2];
            list3[0] = user2.userName;
            list3[1] = "123456";
            parameters3.parameters = list3;
            user2.login(parameters3);
            Assert.IsTrue(parameters3.result is NotImplementedException);
        }

        [TestMethod]
        public void logoutTest()
        {
            ThreadParameters parameters3 = new ThreadParameters();
            object[] list3 = new object[1];
            list3[0] = user2.userName;
            parameters3.parameters = list3;
            user2.logout(parameters3);
            Assert.IsTrue((bool)parameters3.result);

            ThreadParameters parameters4 = new ThreadParameters(); //the user is already logged out
            object[] list4 = new object[1];
            list4[0] = user2.userName;
            parameters4.parameters = list4;
            user2.logout(parameters4);
            Assert.IsTrue(parameters4.result is NotImplementedException);
        }

        [TestMethod]
        public void openStoreTest()
        {
            User u = new User("k"); //the user is not registered to the system
            String name = "store";
            SalesPolicy salesPolicy = new SalesPolicy("default", null);
            PurchasePolicy purchasePolicy = new PurchasePolicy("default", null, null);
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[3];
            list[0] = name;
            list[1] = purchasePolicy;
            list[2] = salesPolicy;
            parameters.parameters = list;
            u.openStore(parameters);
            Assert.IsTrue(parameters.result is NotImplementedException);
            Assert.AreEqual(0, StoreRepository.Instance.stores.Count);
        }
    }
}