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
            ItemReview review = ItemReview.getItemUserReviews(potato, user);
            review.addReview("yummy");
            potato.addUserReviews(review);
        }

        [TestMethod]
        public void addReviewTest()
        {
            ItemReview review = ItemReview.getItemUserReviews(potato, user);
            review.addReview("the potato was very tasty!");
            potato.addUserReviews(review);
            Assert.IsTrue(potato.reviews.ContainsKey("Sagiv"));
            Assert.AreEqual("the potato was very tasty!", potato.reviews["Sagiv"].reviews.First.Value);
            review.addReview("5/5!!!");
            Assert.IsTrue(potato.reviews.ContainsKey("Sagiv"));
            Assert.AreEqual("5/5!!!", potato.reviews["Sagiv"].reviews.First.Value);
        }

        [TestMethod]
        public void removeReviewTest()
        {
            Assert.IsTrue(potato.reviews.ContainsKey("Sagiv"));
            potato.removeUserReviews(user.userName);
            Assert.IsFalse(potato.reviews.ContainsKey("Sagiv"));
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