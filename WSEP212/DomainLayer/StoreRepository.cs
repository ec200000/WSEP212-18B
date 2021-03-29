using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class StoreRepository
    {
        //singelton
        private static StoreRepository storeRepositoryInstance;
        // A data structure associated with a store ID and its store
        public ConcurrentDictionary<int, Store> stores { get; set; }

        private StoreRepository()
        {
            this.stores = new ConcurrentDictionary<int, Store>();
        }

        public static StoreRepository getInstance()
        {
            if (storeRepositoryInstance == null)
            {
                storeRepositoryInstance = new StoreRepository();
            }
            return storeRepositoryInstance;
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

    }
}
