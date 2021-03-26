using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class Store
    {
        public ConcurrentDictionary<Item, int> storage { get; set; }
        public int storeID { get; set; }
        public SalesPolicy salesPolicy { get; set; }
        public PurchasePolicy purchasePolicy { get; set; }
        //DeliveryInterface will be singelton -> the store will use it
        public ConcurrentBag<PurchaseInfo> purchases { get; set; }
        public ConcurrentBag<SellerPermissions> sellersPermissions { get; set; }

        public bool addItem(Item item, int quantity);
        public bool removeItem(Item item);
        public bool changeItemQuantity(Item item, int numOfItems);
        public bool editItem(Item item);
        public bool addNewStoreSeller(SellerPermissions sellerPermissions); //add to list
        public bool addNewPurchase(PurchaseInfo purchase); //add to list
    }
}
