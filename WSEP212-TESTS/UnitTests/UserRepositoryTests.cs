using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class UserRepositoryTest
    {
        private User user1;
        private User user2;
        [TestInitialize]
        public void testInit()
        {
            user1 = new User("a");
            user2 = new User("b");
            UserRepository.Instance.users.TryAdd(user1, false);
            UserRepository.Instance.users.TryAdd(user2, true);
        }

        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
        }

        [TestMethod]
        public void insertNewUserTest()
        {
            User newUser = new User("iris");
            UserRepository.Instance.insertNewUser(newUser, "12345");
            Assert.AreEqual(3, UserRepository.Instance.users.Count);
            UserRepository.Instance.users.TryGetValue(newUser, out var res);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void changeUserLoginStatusTest()
        {
            UserRepository.Instance.changeUserLoginStatus(user1, true, "123"); //register -> login
            UserRepository.Instance.users.TryGetValue(user1, out var res3); 
            Assert.IsTrue(res3);
        }

        [TestMethod]
        public void changeUserLoginStatusTestFails()
        {
            UserRepository.Instance.changeUserLoginStatus(user1, false, "123");
            UserRepository.Instance.users.TryGetValue(user1, out var res1);
            Assert.IsFalse(res1);
            
            UserRepository.Instance.changeUserLoginStatus(user2, false, null); //login -> logout
            UserRepository.Instance.users.TryGetValue(user2, out var res4); 
            Assert.IsFalse(res4);
        }

        [TestMethod]
        public void removeUserTest()
        {
            UserRepository.Instance.removeUser(user1); //removing existing user
            Assert.AreEqual(1, UserRepository.Instance.users.Count);

            User u = new User("c");
            UserRepository.Instance.removeUser(u); //removing user that do not exists
            Assert.AreEqual(1, UserRepository.Instance.users.Count);
        }

        [TestMethod]
        public void updateUserTest()
        {
            PurchaseInvoice purchaseInfo = new PurchaseInvoice(1, "a", null, 1.2, DateTime.Now);
            user1.purchases.Add(purchaseInfo); //updating the user
            UserRepository.Instance.updateUser(user1);
            Assert.AreEqual(2, UserRepository.Instance.users.Count);
            bool found = false;
            foreach (var u in UserRepository.Instance.users.Keys)
            {
                if (u.userName.Equals(user1.userName))
                {
                    Assert.AreEqual(1, u.purchases.Count);
                    u.purchases.TryPeek(out var p);
                    Assert.AreEqual(1.2, p.totalPrice);
                    found = true;
                }
            }
            if(!found)
                Assert.IsTrue(false); //didn't find the user to update

            User user3 = new User("c");
            UserRepository.Instance.updateUser(user3);
            Assert.AreEqual(2, UserRepository.Instance.users.Count); //shouldn't add the user3 because it's not in the DB
            var keys = UserRepository.Instance.users.Keys;
            var res = keys.Contains(user1) && keys.Contains(user2); //no other users were added
            
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void findUserByUserNameTest()
        {
            Assert.IsTrue(UserRepository.Instance.findUserByUserName("b").getTag());
            Assert.IsFalse(UserRepository.Instance.findUserByUserName("k").getTag());
        }

        [TestMethod]
        public void checkIfUserExistsTest()
        {
            Assert.IsTrue(UserRepository.Instance.checkIfUserExists("b"));
            Assert.IsFalse(UserRepository.Instance.checkIfUserExists("k"));
        }

        [TestMethod]
        public void getAllUsersPurchaseHistoryTest()
        {
            PurchaseInvoice purchaseInfo = new PurchaseInvoice(1, "a", null, 1.2, DateTime.Now);
            User user3 = new User("c");
            user3.purchases.Add(purchaseInfo);
            UserRepository.Instance.users.TryAdd(user3, false);
            var res = UserRepository.Instance.getAllUsersPurchaseHistory();
            Assert.IsNotNull(res);
            Assert.AreEqual(3, res.Count); //3 users
            foreach (var p in res)
            {
                Assert.AreEqual(p.Key.Equals("c") ? 1 : 0, p.Value.Count);
            }
        }
    }
}
