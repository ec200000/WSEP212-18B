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
        User user;
        [TestInitialize]
        public void testInit()
        {
            this.user = new User("check name");
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
    }
}
