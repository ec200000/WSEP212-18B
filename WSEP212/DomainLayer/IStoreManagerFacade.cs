using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public interface IStoreManagerFacade
    {
        public bool addItem(int storeID, Item item, int quantity);
        public bool removeItem(int storeID, Item item);
        public bool changeItemQuantity(int storeID, Item item, int numOfItems);
        public bool editItem(int storeID, Item item);
        public bool addNewStoreSeller(int storeID, SellerPermissions sellerPermissions);
        public bool addNewPurchase(int storeID, PurchaseInfo purchase);
    }
}
