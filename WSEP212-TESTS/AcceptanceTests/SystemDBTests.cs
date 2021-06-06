using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.AuthenticationSystem;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;
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
            Startup.readConfigurationFile();
            SystemDBAccess.mock = true;
            
            SystemDBAccess.Instance.Bids.RemoveRange(SystemDBAccess.Instance.Bids);
            SystemDBAccess.Instance.Carts.RemoveRange(SystemDBAccess.Instance.Carts);
            SystemDBAccess.Instance.Invoices.RemoveRange(SystemDBAccess.Instance.Invoices);
            SystemDBAccess.Instance.Items.RemoveRange(SystemDBAccess.Instance.Items);
            SystemDBAccess.Instance.Permissions.RemoveRange(SystemDBAccess.Instance.Permissions);
            SystemDBAccess.Instance.Stores.RemoveRange(SystemDBAccess.Instance.Stores);
            SystemDBAccess.Instance.Users.RemoveRange(SystemDBAccess.Instance.Users);
            SystemDBAccess.Instance.DelayedNotifications.RemoveRange(SystemDBAccess.Instance.DelayedNotifications);
            SystemDBAccess.Instance.ItemReviewes.RemoveRange(SystemDBAccess.Instance.ItemReviewes);
            SystemDBAccess.Instance.UsersInfo.RemoveRange(SystemDBAccess.Instance.UsersInfo);
            SystemDBAccess.Instance.SaveChanges();
            
            UserRepository.Instance.initRepo();
            user = new User("check", 12, false);
            UserRepository.Instance.insertNewUser(user, "123456");
            user.changeState(new LoggedBuyerState(user));
            UserRepository.Instance.changeUserLoginStatus(user.userName, true, "123456");
            
            ResultWithValue<int> addStoreRes = StoreRepository.Instance.addStore("Biga", "Holon", new SalePolicyMock(), new PurchasePolicyMock(), user);
            store = StoreRepository.Instance.getStore(addStoreRes.getValue()).getValue();

            ResultWithValue<int> addItemRes = store.addItemToStorage(100, "bamba", "taim!", 5.0, ItemCategory.Snacks);
            item = store.getItemById(addItemRes.getValue()).getValue();
            review = new ItemReview(user, item.itemID);
            cart = user.shoppingCart;
            
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
        public void unregisterTest()
        {
            SystemDBAccess.Instance.Users.Remove(user);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(0, SystemDBAccess.Instance.Users.Count());
        }
        
        [TestMethod]
        public void registerTest()
        {
            SystemDBAccess.Instance.Users.Add(user);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(1, SystemDBAccess.Instance.Users.Count());
            
        }
        
        [TestMethod]
        public void removeStoreTest()
        {
            SystemDBAccess.Instance.Stores.Remove(store);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(0, SystemDBAccess.Instance.Stores.Count());
        }
        
        [TestMethod]
        public void addStoreTest()
        {
            SystemDBAccess.Instance.Stores.Add(store);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(1, SystemDBAccess.Instance.Stores.Count());
        }
        
        [TestMethod]
        public void removeItemFromStorageTest()
        {
            SystemDBAccess.Instance.Items.Remove(item);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(0, SystemDBAccess.Instance.Items.Count());
        }
        
        [TestMethod]
        public void addItemToStorageTest()
        {
            SystemDBAccess.Instance.Items.Add(item);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(1, SystemDBAccess.Instance.Items.Count());
        }
        
        [TestMethod]
        public void addReviewTest()
        {
            SystemDBAccess.Instance.ItemReviewes.Add(review);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(1, SystemDBAccess.Instance.ItemReviewes.Count());
        }
        
        [TestMethod]
        public void removeReviewTest()
        {
            SystemDBAccess.Instance.ItemReviewes.Remove(review);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(0, SystemDBAccess.Instance.ItemReviewes.Count());
        }

        [TestMethod]
        public void removeCartTest()
        {
            SystemDBAccess.Instance.Carts.Remove(cart);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(0, SystemDBAccess.Instance.Carts.Count());
        }
        
        [TestMethod]
        public void addCartTest()
        {
            SystemDBAccess.Instance.Carts.Add(cart);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(1, SystemDBAccess.Instance.Carts.Count());
        }

        [TestMethod]
        public void addInvoiceTest()
        {
            SystemDBAccess.Instance.Invoices.Add(invoice);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(1, SystemDBAccess.Instance.Invoices.Count());
        }
        
        [TestMethod]
        public void removeSellerPermissionTest()
        {
            SystemDBAccess.Instance.Permissions.Remove(permissions);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(1, SystemDBAccess.Instance.Permissions.Count());
        }
        
        [TestMethod]
        public void addSellerPermissionTest()
        {
            SystemDBAccess.Instance.Permissions.Add(permissions);
            SystemDBAccess.Instance.SaveChanges();
            Assert.AreEqual(2, SystemDBAccess.Instance.Permissions.Count());
        }
    }
}