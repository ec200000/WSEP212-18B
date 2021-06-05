using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication;
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

        [TestInitialize]
        public void SetupAuth()
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
        public void loginTest()
        {
            testInit();
            RegularResult result = controller.login("a", "123");
            Assert.IsTrue(result.getTag());
        }
        
        [TestMethod]
        public void logoutTest()
        {
            testInit();
            controller.login("b", "123456");
            RegularResult result = controller.logout("b");
            Assert.IsTrue(result.getTag());
        }
    }
}