using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer;
using System.Collections.Concurrent;
using WSEP212.ConcurrentLinkedList;



namespace WSEP212.ServiceLayer
{
    interface IStoreManager
    {
        public bool addItem(int storeID, int quantity, String itemName, String description, double price, String category);
        public bool removeItem(int storeID, int itemId);
        public bool changeItemQuantity(int storeID, int itemId, int numOfItems);
        public bool editItem(int storeID, int itemId, String itemName, String description, double price, String category, int quantity);
        public bool addNewStoreSeller(int storeID, String sellerName, int storeId, String grantorName, ConcurrentLinkedList<String> permissionsList);
        public void addNewPurchase(int storeID, String userName, ConcurrentDictionary<int, int> items, double totalPrice, DateTime dateOfPurchase);
    }
}
