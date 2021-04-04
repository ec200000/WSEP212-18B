using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.DomainLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer.Tests
{
    [TestClass()]
    public class StoreRepositoryTests
    {
        private Store store;

        [TestInitialize]
        public void Initialize()
        {
            this.store = new Store(new SalesPolicy(), new PurchasePolicy(), new User("admin"));
        }

        [TestMethod]
        public void addStoreTest()
        {
            bool addStoreBool1 = StoreRepository.getInstance().addStore(store);
            Assert.IsTrue(addStoreBool1);
            bool addStoreBool2 = StoreRepository.getInstance().addStore(store);
            Assert.IsFalse(addStoreBool2);
        }

        [TestMethod]
        public void removeStoreTest()
        {
            int storeID = this.store.storeID;
            bool removeStoreBool1 = StoreRepository.getInstance().removeStore(storeID);
            Assert.IsTrue(removeStoreBool1);
            bool removeStoreBool2 = StoreRepository.getInstance().removeStore(storeID);
            Assert.IsFalse(removeStoreBool2);
        }
    }
}