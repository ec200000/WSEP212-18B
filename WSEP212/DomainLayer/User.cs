using System;
using System.Collections.Concurrent;
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
        public ConcurrentBag<PurchaseInfo> purchases { get; set; }
        public ConcurrentBag<SellerPermissions> sellerPermissions { get; set; }
        public String address { get; set; }

        public void changeState(UserState state)
        {
            this.state = state;
        }

        // params: string username, string password
        // returns: bool
        public void register(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            String username = (String)param.parameters[0];
            String password = (String)param.parameters[1];
            bool res = state.register(username, password);
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: string username, string password
        // returns: bool
        public void login(Object list)
        {
            return state.login(userName, password);
        }

        // params: string username
        // returns: bool
        public void logout(Object list)
        {
            return state.logout(userName);
        }

        // params: int storeID, int itemID
        // returns: bool
        public void addItemToShoppingCart(Object list)
        {
            return state.addItemToShoppingCart(storeID, itemID);
        }

        // params: int storeID, int itemID
        // returns: bool
        public void removeItemFromShoppingCart(Object list)
        {
            return state.removeItemFromShoppingCart(storeID, itemID);
        }
        //edit item in shopping cart is equal to -> remove + add

        // params: ?
        // returns: bool
        public void purchaseItems(Object list) //later
        {
            return state.purchaseItems();
        }

        // params: String storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy
        // returns: bool
        public void openStore(Object list)
        {
            return state.openStore(storeName, purchasePolicy, salesPolicy);
        }

        // params: String review, int itemID, int storeID
        // returns: bool
        public void itemReview(Object list)
        {
            return state.itemReview(review, itemID, storeID);
        }

        // params: int storeID, Item item, int quantity
        // returns: bool
        public void addItemToStorage(Object list)
        {
            return state.addItemToStorage(storeID, item, quantity);
        }

        // params: int storeID, Item item
        // returns: bool
        public void removeItemFromStorage(Object list)
        {
            return state.removeItemFromStorage(storeID, item);
        }

        // params: int storeID, Item item
        // returns:  bool
        public void editItemDetails(Object list)
        {
            return state.editItemDetails(storeID, item);
        }

        // params: String managerName, int storeID
        // returns: bool
        public void appointStoreManager(Object list) //the store manager will receive default permissions(4.9)
        {
            return state.appointStoreManager(managerName, storeID);
        }

        // params: String storeOwnerName, int storeID
        // returns: 
        public void appointStoreOwner(Object list)
        {
            return state.appointStoreOwner(storeOwnerName, storeID);
        }

        // params: String managerName, ConcurrentBag<Permissions> permissions
        // returns: bool
        public void editManagerPermissions(Object list)
        {
            return state.editManagerPermissions(managerName, permissions);
        }

        // params: String managerName, int storeID
        // returns: bool
        public void removeStoreManager(Object list)
        {
            return state.removeStoreManager(managerName, storeID);
        }

        // params: int storeID
        // returns: ConcurrentDictionary<String, ConcurrentBag<Permissions>>
        public void getOfficialsInformation(Object list)
        {
            return state.getOfficialsInformation(storeID);
        }

        // params: int storeID
        // returns: ConcurrentBag<PurchaseInfo>
        public void getStorePurchaseHistory(Object list) //all the purchases of the store that I manage/own
        {
            return state.getStorePurchaseHistory(storeID);
        }

        // params: NONE
        // returns: ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>>
        public void getUsersPurchaseHistory(Object list)
        {
            return state.getUsersPurchaseHistory();
        }

        // params: NONE
        // returns: ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>>
        public void getStoresPurchaseHistory(Object list)
        {
            return state.getStoresPurchaseHistory();
        }

    }
}
