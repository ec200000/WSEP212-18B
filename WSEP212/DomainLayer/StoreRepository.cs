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
        private readonly object storeExistsLock = new object();
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

        // returns all items that contains that string in their name
        public ConcurrentLinkedList<Item> searchItemByName(String itemName)
        {
            ConcurrentLinkedList<Item> itemsByName = new ConcurrentLinkedList<Item>();
            foreach (Store store in stores.Values)
            {
                foreach (Item item in store.storage.Values)
                {
                    if(item.itemName.Contains(itemName))
                    {
                        itemsByName.TryAdd(item);
                    }
                }
            }
            return itemsByName;
        }

        // returns all items that belong to the category
        public ConcurrentLinkedList<Item> searchItemByCategory(String itemCategory)
        {
            ConcurrentLinkedList<Item> itemsByCategory = new ConcurrentLinkedList<Item>();
            foreach (Store store in stores.Values)
            {
                foreach (Item item in store.storage.Values)
                {
                    if (item.category.Equals(itemCategory))
                    {
                        itemsByCategory.TryAdd(item);
                    }
                }
            }
            return itemsByCategory;
        }

        // returns all items that belong to the category, name or description
        public ConcurrentLinkedList<Item> searchItemByKeyWords(String keyWords)
        {
            ConcurrentLinkedList<Item> itemsByKeyWords = new ConcurrentLinkedList<Item>();
            String[] words = keyWords.Split(' ');   // split key words by space

            foreach (Store store in stores.Values)
            {
                foreach (Item item in store.storage.Values)
                {
                    foreach (String word in words)
                    {
                        if (item.category.Equals(word) || item.description.Contains(word) || item.itemName.Contains(word) || item.category.Contains(word))
                        {
                            itemsByKeyWords.TryAdd(item);
                            break;
                        }
                    }
                }
            }
            return itemsByKeyWords;
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
    }
}
