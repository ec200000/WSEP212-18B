using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.DomainLayer;
using System;
using System.Collections.Generic;
using System.Text;
using WSEP212;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class ItemTests
    {
        private static User user;
        private static Item potato;

        [ClassInitialize]
        public void init()
        {
            SystemDBAccess.mock = true;
        }
        
        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
            potato = new Item(5, "potato", "vegetable", 1.5, ItemCategory.Vegetables);
            user = new User("Sagiv");
        }

        [TestMethod]
        public void addReviewTest()
        {
            potato.addReview(user.userName, "the potato was very tasty!");
            Assert.IsTrue(potato.reviews.ContainsKey("Sagiv"));
            Assert.AreEqual("the potato was very tasty!", potato.reviews["Sagiv"].reviews.First.Value);
        }

        [TestMethod]
        public void setQuantityTest()
        {
            Assert.AreEqual(5, potato.quantity);
            potato.setQuantity(10);
            Assert.AreEqual(10, potato.quantity);
            potato.setQuantity(5);
        }

        [TestMethod]
        public void changeQuantitySuccessfullyTest()
        {
            Assert.AreEqual(5, potato.quantity);
            potato.changeQuantity(-3);
            Assert.AreEqual(2, potato.quantity);
            potato.changeQuantity(3);
            Assert.AreEqual(5, potato.quantity);
        }

        [TestMethod]
        public void changeQuantityUnsuccessfullyTest()
        {
            Assert.AreEqual(5, potato.quantity);
            bool res = potato.changeQuantity(-10);
            Assert.IsFalse(res);
            Assert.AreEqual(5, potato.quantity);
        }
    }
}