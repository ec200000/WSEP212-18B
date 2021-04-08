using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.Result;
using WSEP212.ServiceLayer;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class SingleThreadedTests
    {
        private User user1;
        private User user2;
        private Store store;
        private Item item;
        
        [TestInitialize]
        public void testInit()
        {
            user1 = new User("a"); //guest
            user2 = new User("b"); //logged
            user2.changeState(new LoggedBuyerState(user2));
            UserRepository.Instance.users.TryAdd(user1, false);
            UserRepository.Instance.usersInfo.TryAdd("a", Authentication.Instance.encryptPassword("123"));
            UserRepository.Instance.users.TryAdd(user2, true);
            UserRepository.Instance.usersInfo.TryAdd("b", Authentication.Instance.encryptPassword("123456"));
            
            store = new Store("t", "bb", new SalesPolicy("default", null), new PurchasePolicy("default", null,null), user2);
            item = new Item(30, "shoko", "taim retzah!", 12, "milk products");
            store.storage.TryAdd(1, item);
            StoreRepository.Instance.stores.TryAdd(1,store);
        }
        
        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            UserRepository.Instance.usersInfo.Clear();
            StoreRepository.Instance.stores.Clear();
        }

        [TestMethod]
        public void registerTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.register("abcd", "1234");
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(UserRepository.Instance.users.Count, 3);
            
            RegularResult result2 = controller.register("a", "123");
            Assert.IsFalse(result2.getTag());
            Assert.AreEqual(UserRepository.Instance.users.Count, 3);
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException), "user that is logged in, can perform this action again")]
        public void loginTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.login("a", "123");
            Assert.IsTrue(result.getTag());
            UserRepository.Instance.users.TryGetValue(user1, out var res); //is saved as logged in
            Assert.IsTrue(res);
            Assert.AreEqual(UserRepository.Instance.users.Count, 2);
            
            RegularResult result2 = controller.login("b", "123456"); //already logged
            //should  throw exception
            Assert.IsFalse(true); //if it gets here - no exception was thrown
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void logoutTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.logout("b");
            Assert.IsTrue(result.getTag());
            UserRepository.Instance.users.TryGetValue(user2, out var res); //is saved as logged out
            Assert.IsFalse(res);
            Assert.AreEqual(UserRepository.Instance.users.Count, 2);
            
            RegularResult result2 = controller.logout("a"); //not logged in
            //should  throw exception
            Assert.IsFalse(true); //if it gets here - no exception was thrown
        }
        
        [TestMethod]
        public void addItemToShoppingCartTest()
        {
            SystemController controller = new SystemController();
            RegularResult result = controller.addItemToShoppingCart("b",store.storeID, item.itemID, 2); //logged user
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 1);
            user2.shoppingCart.shoppingBags.Clear();
            
            result = controller.addItemToShoppingCart("a",store.storeID, item.itemID, 8); //guest user
            Assert.IsTrue(result.getTag());
            Assert.AreEqual(user1.shoppingCart.shoppingBags.Count, 1);
            
            result = controller.addItemToShoppingCart("b",store.storeID, item.itemID, 100); //over quantity
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 0);
            
            result = controller.addItemToShoppingCart("b",store.storeID, -1, 1); //item does not exists
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.shoppingCart.shoppingBags.Count, 0);
            
            result = controller.addItemToShoppingCart("b",-1, item.itemID, 1); //store doest not exists
            Assert.IsFalse(result.getTag());
            Assert.AreEqual(user2.purchases.Count, 0);
        }
    }
}