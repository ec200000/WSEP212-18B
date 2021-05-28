using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class StoreManagementTests
    {
        SystemController controller =  SystemController.Instance; 
        int itemID;
        int storeID;

        [ClassInitialize]
        public void init()
        {
            SystemDBAccess.mock = true;
        }
        
        public void testInit()
        {
            controller.register("b", 18, "123456");
            controller.register("r", 18, "123456");
            RegularResult result = controller.login("b", "123456");
            storeID = controller.openStore("b", "store1", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, ItemReview>(), 2.4, (int)ItemCategory.Dairy);
            itemID = controller.addItemToStorage("b", storeID, item).getValue();
        }
        
        public void testInitRegStore()
        {
            controller.register("b", 18, "123456");
            controller.login("b", "123456");
            controller.register("w", 18, "123456");
            storeID = controller.openStore("b", "store20", "somewhere", "DEFAULT", "DEFAULT").getValue();
        }
        
        public void testInitStoreWithManager()
        {
            controller.register("b", 18, "123456");
            controller.login("b", "123456");
            controller.register("abc", 18, "123456");
            storeID = controller.openStore("b", "store21", "somewhere", "DEFAULT", "DEFAULT").getValue();
            controller.appointStoreManager("b", "abc", storeID);
        }
        
        public void testInitStoreWithManager2()
        {
            controller.register("b", 18, "123456");
            controller.login("b", "123456");
            controller.register("abcd", 18, "123456");
            storeID = controller.openStore("b", "store22", "somewhere", "DEFAULT", "DEFAULT").getValue();
            controller.appointStoreManager("b", "abcd", storeID);
        }
        
        public void testInitStoreWithManager3()
        {
            controller.register("b", 18, "123456");
            controller.login("b", "123456");
            controller.register("abcde", 18, "123456");
            storeID = controller.openStore("b", "store23", "somewhere", "DEFAULT", "DEFAULT").getValue();
            controller.appointStoreManager("b", "abcde", storeID);
        }

        [TestCleanup]
        public void testClean()
        {
            UserRepository.Instance.users.Clear();
            foreach (Store store in StoreRepository.Instance.stores.Values)
            {
                store.storage.Clear();
            }
            StoreRepository.Instance.stores.Clear();
        }

        [TestMethod]
        public void appointStoreManagerTest()
        {
            testInit();

            RegularResult res = controller.appointStoreManager("b", "r", storeID);
            Assert.IsTrue(res.getTag());   
            res = controller.appointStoreManager("b", "r", storeID);
            Assert.IsFalse(res.getTag());
        }

        [TestMethod]
        public void appointStoreManagerNoSuchStoreTest()
        {
            testInit();

            RegularResult res = controller.appointStoreManager("b", "r", -1);
            Assert.IsFalse(res.getTag());
        }

        [TestMethod]
        public void appointStoreManagerNoSuchUserTest()
        {
            testInit();

            RegularResult res = controller.appointStoreManager("b", "no such user", storeID);
            Assert.IsFalse(res.getTag());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void appointStoreManagerNoPermissionTest()
        {
            testInit();

            controller.appointStoreManager("moon", "r", storeID); //cant perform this action
            Assert.IsFalse(true);
        }


        [TestMethod]
        public void appointStoreOwnerTest()
        {
            testInitRegStore();

            RegularResult res = controller.appointStoreOwner("b", "w", storeID);
            Assert.IsTrue(res.getTag());
            res = controller.appointStoreOwner("b", "w", storeID);
            Assert.IsFalse(res.getTag());
        }

        [TestMethod]
        public void appointStoreOwnerNoSuchStoreTest()
        {
            testInit();

            RegularResult res = controller.appointStoreOwner("b", "w", -1);
            Assert.IsFalse(res.getTag());
        }

        [TestMethod]
        public void appointStoreOwnerNoSuchUserTest()
        {
            testInit();

            RegularResult res = controller.appointStoreOwner("b", "no such user", storeID);
            Assert.IsFalse(res.getTag());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void appointStoreOwnerNoPermissionTest()
        {
            testInit();

            controller.appointStoreOwner("moon", "w", storeID); //cant perform this action
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void editManagerPermissionsTest()
        {
            testInitStoreWithManager();

            //r is store manager with only GetOfficialsInformation permission
            ConcurrentLinkedList<int> newPermissions = new ConcurrentLinkedList<int>();
            newPermissions.TryAdd(3);
            newPermissions.TryAdd(5);
            RegularResult res = controller.editManagerPermissions("b", "abc", newPermissions, storeID);
            Assert.IsTrue(res.getTag());
        }

        [TestMethod]
        public void editManagerPermissionsNoSuchUserTest()
        {
            testInitStoreWithManager();
            ConcurrentLinkedList<int> newPermissions = new ConcurrentLinkedList<int>();
            newPermissions.TryAdd(3);
            newPermissions.TryAdd(5);

            RegularResult res = controller.editManagerPermissions("b", "no such user", newPermissions, storeID);
            Assert.IsFalse(res.getTag());
        }

        [TestMethod]
        public void editManagerPermissionsNoSuchStoreTest()
        {
            testInitStoreWithManager();
            ConcurrentLinkedList<int> newPermissions = new ConcurrentLinkedList<int>();
            newPermissions.TryAdd(3);
            newPermissions.TryAdd(5);

            RegularResult res = controller.editManagerPermissions("b", "abc", new ConcurrentLinkedList<int>(), -1);
            Assert.IsFalse(res.getTag());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void editManagerPermissionsNoPermissionTest()
        {
            testInitStoreWithManager();
            ConcurrentLinkedList<int> newPermissions = new ConcurrentLinkedList<int>();
            newPermissions.TryAdd(3);
            newPermissions.TryAdd(5);

            controller.editManagerPermissions("moon", "r", newPermissions, storeID); //no permission to do so
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void removeStoreManagerTest()
        {
            testInitStoreWithManager2();

            ResultWithValue<NotificationDTO> res = controller.removeStoreManager("b", "abcd", storeID);
            Assert.IsTrue(res.getTag());
        }

        [TestMethod]
        public void removeStoreManagerNoSuchUserTest()
        {
            testInitStoreWithManager2();

            ResultWithValue<NotificationDTO> res = controller.removeStoreManager("b", "no such user", storeID);
            Assert.IsFalse(res.getTag());
        }

        [TestMethod]
        public void removeStoreManagerNoSuchStoreTest()
        {
            testInitStoreWithManager2();

            ResultWithValue<NotificationDTO> res = controller.removeStoreManager("b", "abcd", -1);
            Assert.IsFalse(res.getTag());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void removeStoreManagerNoPermissionTest()
        {
            testInitStoreWithManager2();

            controller.removeStoreManager("moon", "abcd", storeID); //no permission to do so
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void getOfficialsInformationTest()
        {
            testInitStoreWithManager3();
            
            Assert.IsTrue(controller.getOfficialsInformation("b", storeID).getTag());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void getOfficialsInformationNoPermissionTest()
        {
            testInitStoreWithManager3();

            controller.getOfficialsInformation("moon", storeID); //guest user - no permissions to do so
            Assert.IsFalse(true);
        }
    }
}