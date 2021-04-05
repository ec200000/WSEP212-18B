using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS
{
    [TestClass]
    class AuthenticationTest
    {
        [TestMethod]
        public void encryptPasswordTest()
        {
            String password = "abcd";
            String encPass1 = Authentication.Instance.encryptPassword(password);
            Assert.AreNotEqual(password,encPass1); //the password is indeed encrypted
            String encPass2 = Authentication.Instance.encryptPassword(password);
            Assert.AreEqual(encPass1, encPass2); //encrypting the same password should act the same
        }

        [TestMethod]
        public void validatePassword()
        {
            String passwordToValidate = "12345";
            String userPass1 = Authentication.Instance.encryptPassword(passwordToValidate);
            String userPass2 = Authentication.Instance.encryptPassword("abcd");
            
            Assert.IsTrue(Authentication.Instance.validatePassword(passwordToValidate, userPass1));
            Assert.IsFalse(Authentication.Instance.validatePassword(passwordToValidate,userPass2));
        }

    }
}
