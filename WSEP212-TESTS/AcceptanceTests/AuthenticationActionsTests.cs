﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class AuthenticationActionsTests
    {
        SystemController controller = SystemController.Instance;

        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
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
        }
        
        public void testInit()
        {
            controller.register("a", 18, "123");
            controller.register("b", 18, "123456");
        }

        [TestMethod]
        public void registerTest()
        {
            testInit();
            RegularResult result = controller.register("abcd", 18, "1234");
            Assert.IsTrue(result.getTag());
        }

        [TestMethod]
        public void failRegisterTest()
        {
            testInit();

            RegularResult result2 = controller.register("a", 18, "123");
            Assert.IsFalse(result2.getTag());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException), "user that is logged in, can perform this action again")]
        public void loginTest()
        {
            testInit();
            RegularResult result = controller.login("a", "123");
            Assert.IsTrue(result.getTag());
            RegularResult result2 = controller.login("a", "123"); //already logged
            //should  throw exception
            Assert.IsFalse(true); //if it gets here - no exception was thrown
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void logoutTest()
        {
            testInit();
            RegularResult result = controller.logout("b");
            Assert.IsTrue(result.getTag());
            RegularResult result2 = controller.logout("a"); //not logged in
            //should throw exception
            Assert.IsFalse(true); //if it gets here - no exception was thrown
        }
    }
}