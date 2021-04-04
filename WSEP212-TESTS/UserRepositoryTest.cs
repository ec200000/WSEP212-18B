using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS
{
    [TestClass]
    class UserRepositoryTest
    {
        private User user1;
        private User user2;
        [TestInitialize]
        public void testInit()
        {
            user1 = new User("a");
            user2 = new User("b");
            UserRepository.Instance.users.TryAdd(user1, false);
            UserRepository.Instance.usersInfo.TryAdd("a", Authentication.Instance.encryptPassword("123"));
            UserRepository.Instance.users.TryAdd(user2, true);
            UserRepository.Instance.usersInfo.TryAdd("b", Authentication.Instance.encryptPassword("123456"));
            
        }

        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            UserRepository.Instance.usersInfo.Clear();
        }

        [TestMethod]
        public void insertNewUserTest()
        {
            User newUser = new User("iris");
            UserRepository.Instance.insertNewUser(newUser, "12345");
            Assert.AreEqual(3, UserRepository.Instance.users.Count);
            Assert.AreEqual(3, UserRepository.Instance.usersInfo.Count);
            UserRepository.Instance.users.TryGetValue(newUser, out var res);
            Assert.IsFalse(res);
            UserRepository.Instance.usersInfo.TryGetValue("iris", out var pass);
            Assert.AreNotEqual("12345", pass); //should be encrypted
        }

        [TestMethod]
        public void changeUserLoginStatusTest()
        {
            UserRepository.Instance.changeUserLoginStatus(user1, false, "123");
            UserRepository.Instance.users.TryGetValue(user1, out var res1);
            Assert.IsFalse(res1);
            
            UserRepository.Instance.changeUserLoginStatus(user1, true, "723"); //wrong password
            UserRepository.Instance.users.TryGetValue(user1, out var res2); 
            Assert.IsFalse(res2);
            
            UserRepository.Instance.changeUserLoginStatus(user1, true, "123"); //register -> login
            UserRepository.Instance.users.TryGetValue(user1, out var res3); 
            Assert.IsTrue(res3);
            
            UserRepository.Instance.changeUserLoginStatus(user2, false, null); //login -> logout
            UserRepository.Instance.users.TryGetValue(user2, out var res4); 
            Assert.IsFalse(res4);
        }

        [TestMethod]
        public void removeUserTest()
        {
            UserRepository.Instance.removeUser(user1); //removing existing user
            Assert.AreEqual(1, UserRepository.Instance.users.Count);
            Assert.AreEqual(1, UserRepository.Instance.usersInfo.Count);

            User u = new User("c");
            UserRepository.Instance.removeUser(u); //removing user that do not exists
            Assert.AreEqual(1, UserRepository.Instance.users.Count);
            Assert.AreEqual(1, UserRepository.Instance.usersInfo.Count);
        }

        [TestMethod]
        public void updateUserTest()
        {
            
        }
    }
}
