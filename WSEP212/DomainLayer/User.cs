using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class User
    {
        public User(String userName)
        {

        }

        public String userName { get; set; }
        public UserState state { get; set; }
        public ShoppingCart shoppingCart { get; set; }
        public LinkedList<PurchaseInfo> purchases { get; set; }
        public LinkedList<SellerPermissions> sellerPermissions { get; set; }
        public String address { get; set; }

        public void changeState(UserState state)
        {
            this.state = state;
        }

        public void register(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            String username = (String)param.parameters[0];
            String password = (String)param.parameters[1];
            bool res = state.register(username, password);
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }
        public bool login(String userName, String password)
        {
            return state.login(userName, password);
        }
        public bool logout(String userName)
        {
            return state.logout(userName);
        }

        public bool addItemToShoppingCart(int storeID, int itemID)
        {
            return state.addItemToShoppingCart(storeID, itemID);
        }
        public bool removeItemFromShoppingCart(int storeID, int itemID)
        {
            return state.removeItemFromShoppingCart(storeID, itemID);
        }
        //edit item in shopping cart is equal to -> remove + add
        public bool purchaseItems() //later
        {
            return state.purchaseItems();
        }

        public bool openStore(String storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            return state.openStore(storeName, purchasePolicy, salesPolicy);
        }
        public bool itemReview(String review, int itemID, int storeID)
        {
            return state.itemReview(review, itemID, storeID);
        }
        public bool addItemToStorage(int storeID, Item item, int quantity)
        {
            return state.addItemToStorage(storeID, item, quantity);
        }
        public bool removeItemFromStorage(int storeID, Item item)
        {
            return state.removeItemFromStorage(storeID, item);
        }
        public bool editItemDetails(int storeID, Item item)
        {
            return state.editItemDetails(storeID, item);
        }
        public bool appointStoreManager(String managerName, int storeID) //the store manager will receive default permissions(4.9)
        {
            return state.appointStoreManager(managerName, storeID);
        }
        public bool appointStoreOwner(String storeOwnerName, int storeID)
        {
            return state.appointStoreOwner(storeOwnerName, storeID);
        }
        public bool editManagerPermissions(String managerName, LinkedList<Permissions> permissions)
        {
            return state.editManagerPermissions(managerName, permissions);
        }
        public bool removeStoreManager(String managerName, int storeID)
        {
            return state.removeStoreManager(managerName, storeID);
        }
        public Dictionary<String, LinkedList<Permissions>> getOfficialsInformation(int storeID)
        {
            return state.getOfficialsInformation(storeID);
        }
        public LinkedList<PurchaseInfo> getStorePurchaseHistory(int storeID) //all the purchases of the store that I manage/own
        {
            return state.getStorePurchaseHistory(storeID);
        }
        public Dictionary<String, LinkedList<PurchaseInfo>> getUsersPurchaseHistory()
        {
            return state.getUsersPurchaseHistory();
        }
        public Dictionary<int, LinkedList<PurchaseInfo>> getStoresPurchaseHistory()
        {
            return state.getStoresPurchaseHistory();
        }

    }
}
