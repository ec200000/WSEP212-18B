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
            
            Store store2 = new Store("t", "bb", new SalesPolicy("default", null), new PurchasePolicy("default", null,null), user2);
            Item item = new Item(3, "shoko", "taim retzah!", 12, "milk products");
            store2.storage.TryAdd(1, item);
            StoreRepository.Instance.stores.TryAdd(1,store2);
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
    }
}