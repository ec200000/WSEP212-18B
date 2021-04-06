using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using System.Collections.Concurrent;
using WSEP212.DomainLayer;



namespace WSEP212.ServiceLayer
{
    class StoreManager : IStoreManager
    {
        public bool addItem(int storeID, int quantity, String itemName, String description, double price, String category)
        {
            return StoreManagerFacade.Instance.addItem(storeID, quantity, itemName, description, price, category);
        }

        public bool removeItem(int storeID, int itemId)
        {
            return StoreManagerFacade.Instance.removeItem(storeID, itemId);
        }
        public bool changeItemQuantity(int storeID, int itemId, int numOfItems)
        {
            return StoreManagerFacade.Instance.changeItemQuantity(storeID, itemId, numOfItems);
        }
        public bool editItem(int storeID, int itemId, String itemName, String description, double price, String category, int quantity)
        {
            return StoreManagerFacade.Instance.editItem(storeID, itemId, itemName, description, price, category, quantity);
        }
        public bool addNewStoreSeller(int storeID, String sellerName, int storeId, String grantorName, ConcurrentLinkedList<String> permissionsList)
        {
            return StoreManagerFacade.Instance.addNewStoreSeller(storeID, sellerName, storeId, grantorName, permissionsList);
        }
        public void addNewPurchase(int storeID, String userName, ConcurrentDictionary<int, int> items, double totalPrice, DateTime dateOfPurchase)
        {
            StoreManagerFacade.Instance.addNewPurchase(storeID, userName, items, totalPrice, dateOfPurchase);
        }
    }
}
