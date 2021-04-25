using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class AuthenticationActionsTests
    {
        SystemController controller = SystemController.Instance;
        
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