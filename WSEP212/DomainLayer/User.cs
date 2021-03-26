using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class User
    {
        public User(String userName);
        
        public String userName { get; set; }
        public UserState state { get; }
        public ShoppingCart shoppingCart { get; set; }
        public LinkedList<PurchaseInfo> purchases { get; set; }
        public LinkedList<SellerPermissions> sellerPermissions { get; set; }

        public void changeState(UserState state);

        public bool register(String userName, String password);
        public bool login(String userName, String password);
        public bool logout(String userName);

        public bool addItemToShoppingCart(int storeID, int itemID);
        public bool removeItemFromShoppingCart(int storeID, int itemID);
        //edit item in shopping cart is equal to -> remove + add
        public bool purchaseItems(); //later
        public bool openStore(String storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy);
        public bool itemReview(String review, int itemID, int storeID);
        public bool addItemToStorage(int storeID, Item item, int quantity);
        public bool removeItemFromStorage(int storeID, Item item);
        public bool editItemDetails(int storeID, Item item);
        public bool appointStoreManager(String managerName, int storeID); //the store manager will receive default permissions(4.9)
        public bool appointStoreOwner(String storeOwnerName, int storeID);
        public bool editManagerPermissions(String managerName, LinkedList<Permissions> permissions);
        public bool removeStoreManager(String managerName, int storeID);
        public Dictionary<String, LinkedList<Permissions>> getOfficialsInformation(int storeID);
        public LinkedList<PurchaseInfo> getStorePurchaseHistory(int storeID); //all the purchases of the store that I manage/own
        public Dictionary<String, LinkedList<PurchaseInfo>> getUsersPurchaseHistory();
        public Dictionary<int, LinkedList<PurchaseInfo>> getStoresPurchaseHistory();

    }
}
