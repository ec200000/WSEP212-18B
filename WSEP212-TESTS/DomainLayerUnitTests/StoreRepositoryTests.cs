﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS
{
    [TestClass]
    public class StoreRepositoryTests
    {
        private Store store;

        [TestInitialize]
        public void Initialize()
        {
            ConcurrentLinkedList<PurchaseType> purchaseRoutes = new ConcurrentLinkedList<PurchaseType>();
            purchaseRoutes.TryAdd(PurchaseType.ImmediatePurchase);
            this.store = new Store("Mega", new SalesPolicy("DEFAULT", new ConcurrentLinkedList<PolicyRule>()), new PurchasePolicy("DEFAULT", purchaseRoutes, new ConcurrentLinkedList<PolicyRule>()), new User("admin"));
        }

        [TestMethod]
        public void addStoreTest()
        {
            bool addStoreBool1 = StoreRepository.Instance.addStore(store);
            Assert.IsTrue(addStoreBool1);
            bool addStoreBool2 = StoreRepository.Instance.addStore(store);
            Assert.IsFalse(addStoreBool2);
        }

        [TestMethod]
        public void removeStoreTest()
        {
            int storeID = this.store.storeID;
            StoreRepository.Instance.addStore(store);
            bool removeStoreBool1 = StoreRepository.Instance.removeStore(storeID);
            Assert.IsTrue(removeStoreBool1);
            bool removeStoreBool2 = StoreRepository.Instance.removeStore(storeID);
            Assert.IsFalse(removeStoreBool2);
        }
    }
}