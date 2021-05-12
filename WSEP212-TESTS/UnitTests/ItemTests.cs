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
        private User user;
        private Item potato;

        [ClassInitialize]
        public void Initialize()
        {
            this.potato = new Item(5, "potato", "vegetable", 1.5, "vegetables");
            this.user = new User("Sagiv");
            ItemUserReviews userReviews = ItemUserReviews.getItemUserReviews(potato, user);
            userReviews.addReview("yummy");
            potato.addUserReviews(userReviews);
        }

        [TestMethod]
        public void addReviewTest()
        {
            ItemUserReviews userReviews = ItemUserReviews.getItemUserReviews(potato, user);
            userReviews.addReview("the potato was very tasty!");
            potato.addUserReviews(userReviews);
            Assert.IsTrue(potato.reviews.ContainsKey("admin"));
            Assert.AreEqual("the potato was very tasty!", potato.reviews["admin"].reviews.First.Value);
            userReviews.addReview("5/5!!!");
            Assert.IsTrue(potato.reviews.ContainsKey("admin"));
            Assert.AreEqual("5/5!!!", potato.reviews["admin"].reviews.First.Value);
        }

        [TestMethod]
        public void removeReviewTest()
        {
            Assert.IsTrue(potato.reviews.ContainsKey("admin"));
            potato.removeUserReviews(user.userName);
            Assert.IsFalse(potato.reviews.ContainsKey("admin"));
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