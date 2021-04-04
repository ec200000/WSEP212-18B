using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using NuGet.Frameworks;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS
{
    [TestClass]
    class AuthenticationTest
    {
        Authentication authentication;
        [TestInitialize]
        public void testInit()
        {
            this.authentication = Authentication.Instance;
        }

        [TestCleanup]
        public void testClean()
        {
            
        }

        [TestMethod]
        public void TestEncryptTwice()
        {
            String password = "abcd";
            String encPass1 = this.authentication.encryptPassword(password);
            String encPass2 = this.authentication.encryptPassword(password);
            Assert.AreEqual(encPass1, encPass2);
            Assert.IsTrue(true);
        }

    }
}
