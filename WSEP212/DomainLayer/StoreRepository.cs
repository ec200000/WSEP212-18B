using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class StoreRepository
    {
        private readonly object storeExistsLock = new object();
        //singelton
        // A data structure associated with a store ID and its store
        public ConcurrentDictionary<int, Store> stores { get; set; }
        
        StoreRepository()
        {
            stores = new ConcurrentDictionary<int, Store>();
            var storeList = SystemDBAccess.Instance.Stores.ToList();
            storeList.ForEach(s => stores.TryAdd(s.storeID, s));
        }  
        
        private static readonly Lazy<StoreRepository> lazy
            = new Lazy<StoreRepository>(() => new StoreRepository());

        public static StoreRepository Instance
            => lazy.Value;

        
        public ResultWithValue<int> addStore(String storeName, String storeAddress, SalePolicy salesPolicy, PurchasePolicy purchasePolicy, User storeFounder)
        {
            if (storeName == null || storeAddress == null || salesPolicy == null || purchasePolicy == null ||
                storeFounder == null)
            {
                return new FailureWithValue<int>("The Store Has Null Element(s)", -1);
            }

            lock (storeExistsLock)
            {
                if(isExistingStore(storeName, storeAddress))
                {
                    return new FailureWithValue<int>("The Store Already Exist In The Store Repository", -1);
                }
                else
                {
                    Store store = new Store(storeName, storeAddress, salesPolicy, purchasePolicy, storeFounder);
                    store.addToDB();
                    int storeID = store.storeID;
                    stores.TryAdd(storeID, store);
                    return new OkWithValue<int>("The Store Was Added To The Store Repository Successfully", storeID);
                }
            }
        }

        private bool isExistingStore(String storeName, String storeAddress)
        {
            foreach (KeyValuePair<int, Store> storePair in stores)
            {
                Store store = storePair.Value;
                if (store != null && store.storeName!=null && store.storeAddress!=null)
                {
                    if (store.storeName.Equals(storeName) && store.storeAddress.Equals(storeAddress))
                    {
                        return true;
                    }
                }
            }

            //if (SystemDBAccess.Instance.Stores.Find(storeName) != null)
            //    return true;
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

        // returns all items that match the user's search settings
        public ConcurrentDictionary<Item, int> searchItem(SearchItems searchItems)
        {
            ConcurrentDictionary<Item, int> items = new ConcurrentDictionary<Item, int>();
            foreach (Store store in stores.Values)
            {
                foreach (Item item in store.storage.Values)
                {
                    if(searchItems.matchSearchSettings(item))
                    {
                        items.TryAdd(item, store.storeID);
                    }
                }
            }
            return items;
        }
        
        public KeyValuePair<Item,int> getItemByID(int itemID)
        {
            foreach (Store store in stores.Values)
            {
                foreach (Item item in store.storage.Values)
                {
                    if(item.itemID==itemID)
                    {
                        return new KeyValuePair<Item, int>(item, store.storeID);
                    }
                }
            }
            return new KeyValuePair<Item, int>();
        }

        public ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>> getAllStoresPurchsesHistory()
        {
            ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>> storesPurchasesHistory = new ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>>();
            foreach(KeyValuePair<int, Store> storePair in stores)
            {
                storesPurchasesHistory.TryAdd(storePair.Key, storePair.Value.purchasesHistory);
            }
            return storesPurchasesHistory;
        }

        // returns all the stores in the system with their items
        public ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> getStoresAndItemsInfo()
        {
            ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> storesItemsInfo = new ConcurrentDictionary<Store, ConcurrentLinkedList<Item>>();
            foreach (Store store in stores.Values)
            {
                ConcurrentLinkedList<Item> itemsInfo = new ConcurrentLinkedList<Item>();
                foreach (Item item in store.storage.Values)
                {
                    itemsInfo.TryAdd(item);
                }
                storesItemsInfo.TryAdd(store, itemsInfo);
            }
            return storesItemsInfo;
        }
        
        public ConcurrentLinkedList<string> getStoreOwners(int storeID)
        {
            ConcurrentLinkedList<string> officials = new ConcurrentLinkedList<string>();
            Store store = stores[storeID];
            foreach (var sellerPer in store.storeSellersPermissions.Values)
            {
                if(sellerPer.isStoreOwner())
                    officials.TryAdd(sellerPer.SellerName);
            }
            return officials;
        }
    }
}
