using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

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
        private static StoreRepository instance = null;  
        public static StoreRepository Instance  
        {  
            get  
            {  
                lock (padlock)  
                {  
                    if (instance == null)  
                    {  
                        instance = new StoreRepository();  
                    }  
                    return instance;  
                }  
            }  
        }
        
        public bool addStore(Store store)
        {
            int storeID = store.storeID;
            if(!stores.ContainsKey(storeID))
            {
                return stores.TryAdd(storeID, store);
            }
            return false;
        }

        public bool removeStore(int storeID)
        {
            if (stores.ContainsKey(storeID))
            {
                return stores.TryRemove(storeID, out _);
            }
            return false;
        }

        public Store getStore(int storeID)
        {
            if(stores.ContainsKey(storeID))
            {
                return stores[storeID];
            }
            return null;
        }
        public ConcurrentBag<Store> getAllStores()
        {
            ConcurrentBag<Store> allStores = new ConcurrentBag<Store>();
            foreach(KeyValuePair<int,Store> store in stores)
            {
                allStores.Add(store.Value);
            }
            return allStores;
        }

        public ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getAllStoresPurchsesHistory()
        {
            ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> storesPurchasesHistory = new ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>>();
            ConcurrentBag<Store> allStores = getAllStores();
            foreach(Store store in allStores)
            {
                storesPurchasesHistory.TryAdd(store.storeID, store.purchasesHistory);
            }
            return storesPurchasesHistory;

        }
    }
}
