using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class StoreRepository
    {
        //singelton
        private static StoreRepository storeRepositoryInstance;
        private ConcurrentBag<Store> stores;

        private StoreRepository()
        {
            this.stores = new ConcurrentBag<Store>();
        }
        public static StoreRepository getInstance()
        {
            if (storeRepositoryInstance == null)
            {
                storeRepositoryInstance = new StoreRepository();
            }
            return storeRepositoryInstance;
        }

        public Store getStore(int storeID)
        {
            foreach (Store store in stores)
            {
                if(storeID == store.storeID)
                {
                    return store;
                }
            }
            return null;
        }
    }
}
