using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.AuthenticationSystem;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class SystemDBTests
    {
        private static User user;
        private static Store store;
        private static Item item;
        private static ItemReview review;
        private static ShoppingCart cart;
        private static PurchaseInvoice invoice;
        private static SellerPermissions permissions;

        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
            SystemDBAccess.mock = true;

            user = new User("check", 12, false);
            store = new Store("Biga", "Holon", new SalePolicyMock(), new PurchasePolicyMock(), user);
            item = new Item(100, "bamba", "taim!", 5.0, ItemCategory.Snacks);
            review = new ItemReview(user, item.itemID);
            cart = new ShoppingCart(user.userName);
            
            ConcurrentDictionary<int, int> items = new ConcurrentDictionary<int, int>();
            items.TryAdd(item.itemID, 10);
            ConcurrentDictionary<int, double> itemsPrices = new ConcurrentDictionary<int, double>();
            itemsPrices.TryAdd(item.itemID, item.price);
            invoice = new PurchaseInvoice(store.storeID, user.userName, items, itemsPrices, DateTime.Now);

            ConcurrentLinkedList<Permissions> perm = new ConcurrentLinkedList<Permissions>();
            perm.TryAdd(Permissions.AllPermissions);
            permissions = SellerPermissions.getSellerPermissions(user.userName, store.storeID, "admin", perm);
        }

        [TestMethod]
        public void registerTest()
        {
            SystemDBAccess.Instance.Users.Add(user);
            Assert.Equals(1, SystemDBAccess.Instance.Users.Count());
        }
        
        [TestMethod]
        public void unregisterTest()
        {
            SystemDBAccess.Instance.Users.Remove(user);
            Assert.Equals(0, SystemDBAccess.Instance.Users.Count());
        }
        
        [TestMethod]
        public void addStoreTest()
        {
            SystemDBAccess.Instance.Stores.Add(store);
            Assert.Equals(1, SystemDBAccess.Instance.Stores.Count());
        }
        
        [TestMethod]
        public void removeStoreTest()
        {
            SystemDBAccess.Instance.Stores.Remove(store);
            Assert.Equals(0, SystemDBAccess.Instance.Stores.Count());
        }
        
        [TestMethod]
        public void addItemToStorageTest()
        {
            SystemDBAccess.Instance.Items.Add(item);
            Assert.Equals(1, SystemDBAccess.Instance.Items.Count());
        }
        
        [TestMethod]
        public void removeItemFromStorageTest()
        {
            SystemDBAccess.Instance.Items.Remove(item);
            Assert.Equals(0, SystemDBAccess.Instance.Items.Count());
        }

        [TestMethod]
        public void addReviewTest()
        {
            SystemDBAccess.Instance.ItemReviewes.Add(review);
            Assert.Equals(1, SystemDBAccess.Instance.ItemReviewes.Count());
        }
        
        [TestMethod]
        public void removeReviewTest()
        {
            SystemDBAccess.Instance.ItemReviewes.Remove(review);
            Assert.Equals(0, SystemDBAccess.Instance.ItemReviewes.Count());
        }
        
        [TestMethod]
        public void addCartTest()
        {
            SystemDBAccess.Instance.Carts.Add(cart);
            Assert.Equals(1, SystemDBAccess.Instance.Carts.Count());
        }
        
        [TestMethod]
        public void removeCartTest()
        {
            SystemDBAccess.Instance.Carts.Remove(cart);
            Assert.Equals(0, SystemDBAccess.Instance.Carts.Count());
        }
        
        [TestMethod]
        public void addInvoiceTest()
        {
            SystemDBAccess.Instance.Invoices.Add(invoice);
            Assert.Equals(1, SystemDBAccess.Instance.Invoices.Count());
        }
        
        [TestMethod]
        public void removeInvoiceTest()
        {
            SystemDBAccess.Instance.Invoices.Remove(invoice);
            Assert.Equals(0, SystemDBAccess.Instance.Invoices.Count());
        }
        
        [TestMethod]
        public void addSellerPermissionTest()
        {
            SystemDBAccess.Instance.Permissions.Add(permissions);
            Assert.Equals(1, SystemDBAccess.Instance.Permissions.Count());
        }
        
        [TestMethod]
        public void removeSellerPermissionTest()
        {
            SystemDBAccess.Instance.Permissions.Remove(permissions);
            Assert.Equals(0, SystemDBAccess.Instance.Permissions.Count());
        }
    }
}