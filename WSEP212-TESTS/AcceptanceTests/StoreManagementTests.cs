using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class StoreManagementTests
    {
        SystemController controller = new SystemController(); 
        int itemID;
        int storeID;

        public void testInit()
        {
            //controller.register("a", "123");
            controller.register("b", "123456");
            controller.register("r", "123456");
            RegularResult result = controller.login("b", "123456");
            storeID = controller.openStore("b", "store1", "somewhere", "DEFAULT", "DEFAULT").getValue();
            ItemDTO item = new ItemDTO(1, 10, "yammy", "wow", new ConcurrentDictionary<string, string>(), 2.4, "diary");
            itemID = controller.addItemToStorage("b", storeID, item).getValue();
        }
        
        public void testInit2()
        {
            controller.register("w", "123456");
            storeID = controller.openStore("b", "store20", "somewhere", "DEFAULT", "DEFAULT").getValue();
        }
        
        public void testInit3()
        {
            controller.register("abc", "123456");
            storeID = controller.openStore("b", "store21", "somewhere", "DEFAULT", "DEFAULT").getValue();
            controller.appointStoreManager("b", "abc", storeID);
        }
        
        public void testInit4()
        {
            controller.register("abcd", "123456");
            storeID = controller.openStore("b", "store22", "somewhere", "DEFAULT", "DEFAULT").getValue();
            controller.appointStoreManager("b", "abcd", storeID);
        }
        
        public void testInit5()
        {
            controller.register("abcde", "123456");
            storeID = controller.openStore("b", "store23", "somewhere", "DEFAULT", "DEFAULT").getValue();
            controller.appointStoreManager("b", "abcde", storeID);
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void appointStoreManagerTest()
        {
            testInit();

            RegularResult res = controller.appointStoreManager("b", "r", storeID);
            Assert.IsTrue(res.getTag());
            
            res = controller.appointStoreManager("b", "r", storeID);
            Assert.IsFalse(res.getTag());
            
            res = controller.appointStoreManager("b", "r", -1);
            Assert.IsFalse(res.getTag());
            
            res = controller.appointStoreManager("b", "no such user", storeID);
            Assert.IsFalse(res.getTag());
            
            controller.appointStoreManager("moon", "r", storeID); //cant perform this action
            Assert.IsFalse(true);

        }
        
        
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void appointStoreOwnerTest()
        {
            testInit2();

            RegularResult res = controller.appointStoreOwner("b", "w", -1);
            Assert.IsFalse(res.getTag());
            
            res = controller.appointStoreOwner("b", "no such user", storeID);
            Assert.IsFalse(res.getTag());
            
            res = controller.appointStoreOwner("b", "w", storeID);
            Assert.IsTrue(res.getTag());
            
            res = controller.appointStoreOwner("b", "w", storeID);
            Assert.IsFalse(res.getTag());
            
            controller.appointStoreOwner("moon", "w", storeID); //cant perform this action
            Assert.IsFalse(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void editManagerPermissionsTest()
        {
            testInit3();

            //r is store manager with only GetOfficialsInformation permission
            ConcurrentLinkedList<int> newPermissions = new ConcurrentLinkedList<int>();
            newPermissions.TryAdd(3);
            newPermissions.TryAdd(5);
            RegularResult res = controller.editManagerPermissions("b", "abc", newPermissions, storeID);
            Assert.IsTrue(res.getTag());

            res = controller.editManagerPermissions("b", "no such user", newPermissions, storeID);
            Assert.IsFalse(res.getTag());

            res = controller.editManagerPermissions("b", "abc", new ConcurrentLinkedList<int>(), -1);
            Assert.IsFalse(res.getTag());

            controller.editManagerPermissions("moon", "r", newPermissions, storeID); //no permission to do so
            Assert.IsFalse(true);
        }


        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void removeStoreManagerTest()
        {
            testInit4();
            
            RegularResult res = controller.removeStoreManager("b", "no such user", storeID);
            Assert.IsFalse(res.getTag());
            
            res = controller.removeStoreManager("b", "abcd", -1);
            Assert.IsFalse(res.getTag());
            
            res = controller.removeStoreManager("b", "abcd", storeID);
            Assert.IsTrue(res.getTag());
            
            controller.removeStoreManager("moon", "abcd", storeID); //no permission to do so
            Assert.IsFalse(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void getOfficialsInformationTest()
        {
            testInit5();
            
            Assert.IsTrue(controller.getOfficialsInformation("b", storeID).getTag());
            
            Assert.IsTrue(controller.getOfficialsInformation("abcde", storeID).getTag());
            
            controller.getOfficialsInformation("moon", storeID); //guest user - no permissions to do so
            Assert.IsFalse(true);
        }
    }
}