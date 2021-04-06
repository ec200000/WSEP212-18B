using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS
{
    [TestClass]
    public class UserTestNegative
    {
        User user;
        [TestInitialize]
        public void testInit()
        {
            this.user = new User("a");
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
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = user.userName;
            list[1] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            if ((bool)parameters.result)
            {
                ThreadParameters parameters2 = new ThreadParameters();
                object[] list2 = new object[2];
                list2[0] = user.userName;
                list2[1] = "1234";
                parameters2.parameters = list2;
                user.login(parameters2);
                Assert.IsTrue((bool)parameters2.result);
                Assert.IsTrue(user.state is LoggedBuyerState);
            }

            ThreadParameters parameters3 = new ThreadParameters(); //the user is already logged in
            object[] list3 = new object[2];
            list3[0] = user.userName;
            list3[1] = "1234";
            parameters3.parameters = list3;
            user.login(parameters3);
            Assert.IsTrue(parameters3.result is NotImplementedException);
        }

        [TestMethod]
        public void logoutTest()
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = user.userName;
            list[1] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            if ((bool)parameters.result)
            {
                ThreadParameters parameters2 = new ThreadParameters();
                object[] list2 = new object[2];
                list2[0] = user.userName;
                list2[1] = "1234";
                parameters2.parameters = list2;
                user.login(parameters2);
                if ((bool)parameters2.result)
                {
                    ThreadParameters parameters3 = new ThreadParameters();
                    object[] list3 = new object[1];
                    list3[0] = user.userName;
                    parameters3.parameters = list3;
                    user.logout(parameters3);
                    Assert.IsTrue((bool)parameters3.result);
                }
            }

            ThreadParameters parameters4 = new ThreadParameters(); //the user is already logged out
            object[] list4 = new object[1];
            list4[0] = user.userName;
            parameters4.parameters = list4;
            user.logout(parameters4);
            Assert.IsTrue(parameters4.result is NotImplementedException);
        }
    }
}