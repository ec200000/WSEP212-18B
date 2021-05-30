using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using WSEP212;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class UserRepositoryTest
    {
        private static User user1;
        private static User user2;
        
        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
            SystemDBAccess.mock = true;
            
            SystemDBMock.Instance.Bids.RemoveRange(SystemDBMock.Instance.Bids);
            SystemDBMock.Instance.Carts.RemoveRange(SystemDBMock.Instance.Carts);
            SystemDBMock.Instance.Invoices.RemoveRange(SystemDBMock.Instance.Invoices);
            SystemDBMock.Instance.Items.RemoveRange(SystemDBMock.Instance.Items);
            SystemDBMock.Instance.Permissions.RemoveRange(SystemDBMock.Instance.Permissions);
            SystemDBMock.Instance.Stores.RemoveRange(SystemDBMock.Instance.Stores);
            SystemDBMock.Instance.Users.RemoveRange(SystemDBMock.Instance.Users);
            SystemDBMock.Instance.DelayedNotifications.RemoveRange(SystemDBMock.Instance.DelayedNotifications);
            SystemDBMock.Instance.ItemReviewes.RemoveRange(SystemDBMock.Instance.ItemReviewes);
            SystemDBMock.Instance.UsersInfo.RemoveRange(SystemDBMock.Instance.UsersInfo);
            
            UserRepository.Instance.initRepo();
            user1 = new User("a");
            UserRepository.Instance.insertNewUser(user1, "123456");
            user2 = new User("b");
            UserRepository.Instance.insertNewUser(user2, "123456");
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
        public void removeExistingUserTest()
        {
            UserRepository.Instance.removeUser(user1); //removing existing user
            Assert.AreEqual(1, UserRepository.Instance.users.Count);
        }

        [TestMethod]
        public void removeNotExistingUserTest()
        {
            User u = new User("c");
            Assert.IsFalse(UserRepository.Instance.removeUser(u)); //removing user that do not exists
        }

        [TestMethod]
        public void updateUserTest()
        {
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(10, 1);
            ConcurrentDictionary<int, double> itemsPrices = new ConcurrentDictionary<int, double>();
            itemsPrices.TryAdd(10, 1.2);
            PurchaseInvoice purchaseInfo = new PurchaseInvoice(1, "a", items, itemsPrices, DateTime.Now);
            user1.purchases.TryAdd(purchaseInfo.purchaseInvoiceID, purchaseInfo); //updating the user
            UserRepository.Instance.updateUser(user1);
            Assert.AreEqual(2, UserRepository.Instance.users.Count);
            bool found = false;
            foreach (var u in UserRepository.Instance.users.Keys)
            {
                if (u.userName.Equals(user1.userName))
                {
                    Assert.AreEqual(1, u.purchases.Count);
                    u.purchases.TryGetValue(purchaseInfo.purchaseInvoiceID, out var p);
                    Assert.AreEqual(1.2, p.getPurchaseTotalPrice());
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
        public void findExistUserByNameTest()
        {
            Assert.IsTrue(UserRepository.Instance.findUserByUserName("b").getTag());
        }

        [TestMethod]
        public void findNotExistUserByNameTest()
        {
            Assert.IsFalse(UserRepository.Instance.findUserByUserName("k").getTag());
        }

        [TestMethod]
        public void checkIfUserExistsSuccessfulTest()
        {
            Assert.IsTrue(UserRepository.Instance.checkIfUserExists("b"));
        }

        [TestMethod]
        public void checkIfUserExistsUnsuccessfulTest()
        {
            Assert.IsFalse(UserRepository.Instance.checkIfUserExists("k"));
        }

        [TestMethod]
        public void getAllUsersPurchaseHistoryTest()
        {
            ConcurrentDictionary<int, double> itemsPrices = new ConcurrentDictionary<int, double>();
            itemsPrices.TryAdd(10, 1.2);
            PurchaseInvoice purchaseInfo = new PurchaseInvoice(1, "a", null, itemsPrices, DateTime.Now);
            User user3 = new User("c");
            user3.purchases.TryAdd(purchaseInfo.purchaseInvoiceID, purchaseInfo);
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
