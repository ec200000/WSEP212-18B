using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class GuestBuyerState : UserState
    {
        public GuestBuyerState(User user) : base(user)
        {
        }

        public override UserType getUserType()
        {
            return UserType.GuestBuyer;
        }

        public override bool addItemToShoppingCart(int storeID, int itemID, int quantity)
        {
            return this.user.shoppingCart.addItemToShoppingBag(storeID, itemID, quantity);
            // adding a quantity of the item to the shopping bag that belongs to the store id
        }

        public override bool addItemToStorage(int storeID, int quantity, String itemName, String description, double price, String category)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override bool appointStoreManager(string managerName, int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override bool appointStoreOwner(string storeOwnerName, int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override bool editItemDetails(int storeID, int itemID, int quantity, String itemName, String description, double price, String category)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override bool editManagerPermissions(string managerName, ConcurrentLinkedList<Permissions> permissions, int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override ConcurrentDictionary<User, ConcurrentLinkedList<Permissions>> getOfficialsInformation(int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getStoresPurchaseHistory()
        {
            throw new NotImplementedException();
            // only system managers can do that
        }

        public override ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>> getUsersPurchaseHistory()
        {
            throw new NotImplementedException();
            // only system managers can do that
        }

        public override bool itemReview(string review, int itemID, int storeID)
        {
            throw new NotImplementedException();
            // only logged buyers can do that
        }

        public override bool login(string userName, string password)
        {
            if(UserRepository.Instance.changeUserLoginStatus(UserRepository.Instance.findUserByUserName(userName), true, password))
            {
                user.changeState(new LoggedBuyerState(user)); // changing the user's state to logged buyer
                return true;
            }
            return false;
        }

        public override bool logout(string userName)
        {
            throw new NotImplementedException(); // can't log out because he ain't logged in
        }

        public override bool openStore(string storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            throw new NotImplementedException();
            // only logged buyers can do that
        }

        public override bool purchaseItems(string address)
        {
            if (address == null) // checking the user entered an addresss
                return false;
            return HandlePurchases.Instance.purchaseItems(this.user, address); // handling the purchase procedure
        }

        public override bool register(string userName, string password)
        {
            User user = new User(userName); // creating the user
            return UserRepository.Instance.insertNewUser(user, password); // adding the user to the repository (DB)
        }

        public override bool removeItemFromShoppingCart(int storeID, int itemID)
        {
            return this.user.shoppingCart.removeItemFromShoppingBag(storeID, itemID);
            // removing the item from the shopping bag of the store
        }

        public override bool removeItemFromStorage(int storeID, int itemID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override bool removeStoreManager(string managerName, int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }
    }
}
