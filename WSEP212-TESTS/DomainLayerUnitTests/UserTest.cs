using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS
{
    [TestClass]
    public class UserTest
    {
        User user;
        [TestInitialize]
        public void testInit()
        {
            this.user = new User("check name");
        }

        public bool registerAndLogin()
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
                return (bool)parameters2.result;
            }
            return false;
        }

        public bool logout()
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[1];
            list[0] = user.userName;
            parameters.parameters = list;
            user.logout(parameters);
            return (bool)parameters.result;
        }

        public void switchToSystemManager()
        {
            user.changeState(new SystemManagerState(this.user));
        }

        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            UserRepository.Instance.usersInfo.Clear();
        }

        [TestMethod]
        public void TestConstructor()
        {
            Assert.AreEqual("check name", user.userName);
            Assert.IsTrue(typeof(GuestBuyerState).IsInstanceOfType(user.state));
        }

        [TestMethod]
        public void TestChangeState()
        {
            user.changeState(new LoggedBuyerState(user));
            Assert.IsTrue(typeof(LoggedBuyerState).IsInstanceOfType(user.state));
        }

        [TestMethod]
        public void TestRegister()
        {
            ThreadParameters parameters = new ThreadParameters();
            object[] list = new object[2];
            list[0] = user.userName;
            list[1] = "1234";
            parameters.parameters = list;
            user.register(parameters);
            Assert.IsTrue((bool)parameters.result);
            Assert.AreEqual(UserRepository.Instance.users.Count, 1);

            User user2 = new User(user.userName); //negative test!
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
        //[ExpectedException (typeof(NotImplementedException))]
        public void TestLogin()
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
                Assert.IsTrue(typeof(LoggedBuyerState).IsInstanceOfType(user.state));
            }

            ThreadParameters parameters3 = new ThreadParameters(); //the user is already logged in
            object[] list3 = new object[2];
            list3[0] = user.userName;
            list3[1] = "1234";
            parameters3.parameters = list3;
            user.login(parameters3);
            Assert.IsTrue(typeof(NotImplementedException).IsInstanceOfType(parameters3.result));

        }

        [TestMethod]
        public void TestLoginWrongPassword() // wrong password
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
                list2[1] = "3456";
                parameters2.parameters = list2;
                user.login(parameters2);
                Assert.IsFalse((bool)parameters2.result);
                Assert.IsTrue(typeof(GuestBuyerState).IsInstanceOfType(user.state));

            }
        }

        [TestMethod]
        public void TestLogout()
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
            Assert.IsTrue(typeof(NotImplementedException).IsInstanceOfType(parameters4.result));
        }

        [TestMethod]
        public void TestAddItemToShoppingCart() // wrong password
        {
            if (registerAndLogin())
            {
                Item item = new Item(3, "shoko", "taim retzah!", 12, "milk products");
                Store store = new Store("store", new SalesPolicy("default", null), new PurchasePolicy("default", null, null), this.user);
                StoreRepository.Instance.addStore(store);
                store.addItemToStorage(item);
                int storeID = store.storeID;
                int itemID = item.itemID;
                int quantity = 2;
                ThreadParameters parameters = new ThreadParameters();
                object[] list = new object[3];
                list[0] = storeID;
                list[1] = itemID;
                list[2] = quantity;
                parameters.parameters = list;
                user.addItemToShoppingCart(parameters);
                Assert.IsTrue((bool)parameters.result);
                Assert.AreEqual(1, user.shoppingCart.shoppingBags[storeID].items.Count);
            }
        }

        [TestMethod]
        public void TestRemoveItemFromShoppingCart() // wrong password
        {
            if (registerAndLogin())
            {
                Item item = new Item(3, "shoko", "taim retzah!", 12, "milk products");
                Store store = new Store("store", new SalesPolicy("default", null), new PurchasePolicy("default", null, null), this.user);
                StoreRepository.Instance.addStore(store);
                store.addItemToStorage(item);
                int storeID = store.storeID;
                int itemID = item.itemID;
                int quantity = 2;
                ThreadParameters parameters = new ThreadParameters();
                object[] list = new object[3];
                list[0] = storeID;
                list[1] = itemID;
                list[2] = quantity;
                parameters.parameters = list;
                user.addItemToShoppingCart(parameters);
                if ((bool) parameters.result)
                {
                    ThreadParameters parameters2 = new ThreadParameters();
                    object[] list2 = new object[2];
                    list2[0] = storeID;
                    list2[1] = itemID;
                    parameters2.parameters = list2;
                    user.removeItemFromShoppingCart(parameters2);
                    Assert.IsTrue((bool)parameters2.result);
                    Assert.AreEqual(0, user.shoppingCart.shoppingBags.Count);
                }
            }
        }
    }
}
