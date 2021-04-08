using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.Result;

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
            ResultWithValue<int> storeIdRes = StoreRepository.Instance.addStore("Mega", "Ashdod", new SalesPolicy("DEFAULT", new ConcurrentLinkedList<PolicyRule>()), new PurchasePolicy("DEFAULT", purchaseRoutes, new ConcurrentLinkedList<PolicyRule>()), new User("admin"));
            this.store = StoreRepository.Instance.getStore(storeIdRes.getValue()).getValue();
        }

        [TestCleanup]
        public void afterTests()
        {
            StoreRepository.Instance.removeStore(store.storeID);
        }

        [TestMethod]
        public void addStoreTest()
        {
            ConcurrentLinkedList<PurchaseType> purchaseRoutes = new ConcurrentLinkedList<PurchaseType>();
            purchaseRoutes.TryAdd(PurchaseType.ImmediatePurchase);
            ResultWithValue<int> addStoreBool = StoreRepository.Instance.addStore("Mega", "Holon", new SalesPolicy("DEFAULT", new ConcurrentLinkedList<PolicyRule>()), new PurchasePolicy("DEFAULT", purchaseRoutes, new ConcurrentLinkedList<PolicyRule>()), new User("admin"));
            Assert.IsTrue(addStoreBool.getTag());
            addStoreBool = StoreRepository.Instance.addStore("Mega", "Holon", new SalesPolicy("DEFAULT", new ConcurrentLinkedList<PolicyRule>()), new PurchasePolicy("DEFAULT", purchaseRoutes, new ConcurrentLinkedList<PolicyRule>()), new User("admin"));
            Assert.IsFalse(addStoreBool.getTag());
        }

        [TestMethod]
        public void removeStoreTest()
        {
            int storeID = this.store.storeID;
            RegularResult removeStoreBool = StoreRepository.Instance.removeStore(storeID);
            Assert.IsTrue(removeStoreBool.getTag());
            removeStoreBool = StoreRepository.Instance.removeStore(storeID);
            Assert.IsFalse(removeStoreBool.getTag());
        }
    }
}