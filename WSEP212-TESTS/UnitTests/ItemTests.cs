using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.DomainLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class ItemTests
    {
        private Item potato;

        [TestInitialize]
        public void Initialize()
        {
            this.potato = new Item(5, "potato", "vegetable", 1.5, "vegetables");
        }

        [TestMethod]
        public void addReviewTest()
        {
            potato.addReview("admin", "the potato was very tasty!");
            Assert.IsTrue(potato.reviews.ContainsKey("admin"));
            Assert.AreEqual("the potato was very tasty!", potato.reviews["admin"].reviews.First.Value);
            potato.addReview("admin", "5/5!!!");
            Assert.IsTrue(potato.reviews.ContainsKey("admin"));
            Assert.AreEqual("5/5!!!", potato.reviews["admin"].reviews.First.Value);
        }
    }
}