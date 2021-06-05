using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using WSEP212;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.ServiceLayer.Result;

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
            UserRepository.Instance.users.TryGetValue(newUser, out var res);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void changeUserLoginStatusTest()
        {
            UserRepository.Instance.changeUserLoginStatus(user1.userName, true, "123456"); //register -> login
            User user = UserRepository.Instance.findUserByUserName(user1.userName).getValue();
            Assert.IsTrue(UserRepository.Instance.users[user]);
            
            UserRepository.Instance.changeUserLoginStatus(user1.userName, false, "123456"); //login -> register
        }

        [TestMethod]
        public void changeUserLoginStatusTestFails()
        {
            RegularResult res = UserRepository.Instance.changeUserLoginStatus(user1.userName, true, "123");
            User u1 = UserRepository.Instance.findUserByUserName(user1.userName).getValue();
            Console.WriteLine(res.getMessage());
            Assert.IsFalse(UserRepository.Instance.users[u1]);
            
            UserRepository.Instance.changeUserLoginStatus(user2.userName, false, null); //login -> logout
            User u2 = UserRepository.Instance.findUserByUserName(user2.userName).getValue();
            Assert.IsFalse(UserRepository.Instance.users[u2]);
        }

        [TestMethod]
        public void removeExistingUserTest()
        {
            UserRepository.Instance.removeUser(user1); //removing existing user
            Assert.IsFalse(UserRepository.Instance.users.ContainsKey(user1));
            
            UserRepository.Instance.insertNewUser(user1, "123456");
        }

        [TestMethod]
        public void removeNotExistingUserTest()
        {
            User u = new User("c");
            Assert.IsFalse(UserRepository.Instance.removeUser(u)); //removing user that do not exists
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
            User user3 = new User("d");
            user3.purchases.TryAdd(purchaseInfo.purchaseInvoiceID, purchaseInfo);
            UserRepository.Instance.users.TryAdd(user3, false);
            var res = UserRepository.Instance.getAllUsersPurchaseHistory();
            Assert.IsNotNull(res);
            Assert.AreEqual(5, res.Count); // 5 users
            foreach (var p in res)
            {
                Assert.AreEqual(p.Key.Equals("d") ? 1 : 0, p.Value.Count);
            }
        }
    }
}
