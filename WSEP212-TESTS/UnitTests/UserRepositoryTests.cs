using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WebApplication;
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
            Startup.readConfigurationFile();
            SystemDBAccess.mock = true;
            
            SystemDBAccess.Instance.Bids.RemoveRange(SystemDBAccess.Instance.Bids);
            SystemDBAccess.Instance.Carts.RemoveRange(SystemDBAccess.Instance.Carts);
            SystemDBAccess.Instance.Invoices.RemoveRange(SystemDBAccess.Instance.Invoices);
            SystemDBAccess.Instance.Items.RemoveRange(SystemDBAccess.Instance.Items);
            SystemDBAccess.Instance.Permissions.RemoveRange(SystemDBAccess.Instance.Permissions);
            SystemDBAccess.Instance.Stores.RemoveRange(SystemDBAccess.Instance.Stores);
            SystemDBAccess.Instance.Users.RemoveRange(SystemDBAccess.Instance.Users);
            SystemDBAccess.Instance.DelayedNotifications.RemoveRange(SystemDBAccess.Instance.DelayedNotifications);
            SystemDBAccess.Instance.ItemReviewes.RemoveRange(SystemDBAccess.Instance.ItemReviewes);
            SystemDBAccess.Instance.UsersInfo.RemoveRange(SystemDBAccess.Instance.UsersInfo);
            SystemDBAccess.Instance.SaveChanges();

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
            Assert.IsFalse(res.Key);
        }

        [TestMethod]
        public void changeUserLoginStatusTest()
        {
            UserRepository.Instance.changeUserLoginStatus(user1.userName, true, "123456"); //register -> login
            User user = UserRepository.Instance.findUserByUserName(user1.userName).getValue();
            Assert.IsTrue(UserRepository.Instance.users[user].Key);
            
            UserRepository.Instance.changeUserLoginStatus(user1.userName, false, "123456"); //login -> register
        }

        [TestMethod]
        public void changeUserLoginStatusTestFails()
        {
            RegularResult res = UserRepository.Instance.changeUserLoginStatus(user1.userName, true, "123");
            User u1 = UserRepository.Instance.findUserByUserName(user1.userName).getValue();
            Console.WriteLine(res.getMessage());
            Assert.IsFalse(UserRepository.Instance.users[u1].Key);
            
            UserRepository.Instance.changeUserLoginStatus(user2.userName, false, null); //login -> logout
            User u2 = UserRepository.Instance.findUserByUserName(user2.userName).getValue();
            Assert.IsFalse(UserRepository.Instance.users[u2].Key);
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
            UserRepository.Instance.users.TryAdd(user3, new KeyValuePair<bool, DateTime>(false,DateTime.MinValue));
            var res = UserRepository.Instance.getAllUsersPurchaseHistory();
            Assert.IsNotNull(res);
            Assert.AreEqual(4, res.Count); // 4 users
            foreach (var p in res)
            {
                Assert.AreEqual(p.Key.Equals("d") ? 1 : 0, p.Value.Count);
            }
        }
    }
}
