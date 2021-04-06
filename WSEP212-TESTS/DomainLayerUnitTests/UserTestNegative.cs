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
            
            User user2 = new User(user.userName); 
            ThreadParameters parameters2 = new ThreadParameters();
            object[] list2 = new object[2];
            list2[0] = user.userName;
            list2[1] = "5678";
            parameters2.parameters = list2;
            user.register(parameters2);
            Assert.IsFalse((bool)parameters2.result);
            Assert.AreEqual(UserRepository.Instance.users.Count, 1);
        }
    }
}