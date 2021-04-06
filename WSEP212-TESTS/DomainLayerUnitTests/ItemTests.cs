using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.DomainLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212_TESTS
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
            bool review = potato.addReview("admin", "the potato was very tasty!");
            Assert.IsTrue(review);
            bool anotherReview = potato.addReview("admin", "5/5!!!");
            Assert.IsTrue(anotherReview);
        }
    }
}