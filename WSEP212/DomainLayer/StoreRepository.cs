using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.Result;

namespace WSEP212.DomainLayer
{
    public class StoreRepository
    {
        //singelton
        // A data structure associated with a store ID and its store
        public ConcurrentDictionary<int, Store> stores { get; set; }
        
        StoreRepository()
        {
            stores = new ConcurrentDictionary<int, Store>();
        }  
        private static readonly object padlock = new object();  
        private static readonly Lazy<StoreRepository> lazy
            = new Lazy<StoreRepository>(() => new StoreRepository());

        public static StoreRepository Instance
            => lazy.Value;

        
        public ResultWithValue<int> addStore(String storeName, String storeAddress, SalesPolicy salesPolicy, PurchasePolicy purchasePolicy, User storeFounder)
        {
            if(isExistingStore(storeName, storeAddress))
            {
                return new FailureWithValue<int>("The Store Already Exist In The Store Repository", -1);
            }
            else
            {
                Store store = new Store(storeName, storeAddress, salesPolicy, purchasePolicy, storeFounder);
                int storeID = store.storeID;
                stores.TryAdd(storeID, store);
                return new OkWithValue<int>("The Store Was Added To The Store Repository Successfully", storeID);
            }
        }

        private bool isExistingStore(String storeName, String storeAddress)
        {
            foreach (KeyValuePair<int, Store> storePair in stores)
            {
                Store store = storePair.Value;
                if (store.storeName.Equals(storeName) && store.storeAddress.Equals(storeAddress))
                {
                    return true;
                }
            }
            return false;
        }

        public RegularResult removeStore(int storeID)
        {
            if (stores.ContainsKey(storeID))
            {
                stores.TryRemove(storeID, out _);
                return new Ok("The Store Was Removed From The Store Repository Successfully");
            }
            return new Failure("The Store Is Not Exist In The Store Repository");
        }

        public ResultWithValue<Store> getStore(int storeID)
        {
            if(stores.ContainsKey(storeID))
            {
                return new OkWithValue<Store>("The Store Was Found In The Store Repository Successfully", stores[storeID]);
            }
            return new FailureWithValue<Store>("The Store Is Not Exist In The Store Repository", null);
        }

        public ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getAllStoresPurchsesHistory()
        {
            ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> storesPurchasesHistory = new ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>>();
            foreach(KeyValuePair<int, Store> storePair in stores)
            {
                storesPurchasesHistory.TryAdd(storePair.Key, storePair.Value.purchasesHistory);
            }
            return storesPurchasesHistory;
        }
    }
}
