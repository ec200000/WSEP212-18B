using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer;

namespace WSEP212.ServiceLayer
{
    interface IStoreManager
    {
        public bool addItem(int storeID, Item item, int quantity);
        public bool removeItem(int storeID, Item item);
        public bool changeItemQuantity(int storeID, Item item, int numOfItems);
        public bool editItem(int storeID, Item item);
        public bool addNewStoreSeller(int storeID, SellerPermissions sellerPermissions);
        public bool addNewPurchase(int storeID, PurchaseInfo purchase);
    }
}
