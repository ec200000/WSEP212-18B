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
        }

        public override bool addItemToStorage(int storeID, int quantity, String itemName, String description, double price, String category)
        {
            throw new NotImplementedException();
        }

        public override bool appointStoreManager(string managerName, int storeID)
        {
            throw new NotImplementedException();
        }

        public override bool appointStoreOwner(string storeOwnerName, int storeID)
        {
            throw new NotImplementedException();
        }

        public override bool editItemDetails(int storeID, int itemID, int quantity, String itemName, String description, double price, String category)
        {
            throw new NotImplementedException();
        }

        public override bool editManagerPermissions(string managerName, ConcurrentLinkedList<Permissions> permissions, int storeID)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionary<User, ConcurrentLinkedList<Permissions>> getOfficialsInformation(int storeID)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(int storeID)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getStoresPurchaseHistory()
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>> getUsersPurchaseHistory()
        {
            throw new NotImplementedException();
        }

        public override bool itemReview(string review, int itemID, int storeID)
        {
            throw new NotImplementedException();
        }

        public override bool login(string userName, string password)
        {
            if(UserRepository.Instance.changeUserLoginStatus(UserRepository.Instance.findUserByUserName(userName), true, password))
            {
                user.changeState(new LoggedBuyerState(user));
                return true;
            }
            return false;
        }

        public override bool logout(string userName)
        {
            throw new NotImplementedException();
        }

        public override bool openStore(string storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            throw new NotImplementedException();
        }

        public override bool purchaseItems(string address)
        {
            if (address == null)
                return false;
            return HandlePurchases.Instance.purchaseItems(this.user, address);
        }

        public override bool register(string userName, string password)
        {
            User user = new User(userName);
            return UserRepository.Instance.insertNewUser(user, password);
        }

        public override bool removeItemFromShoppingCart(int storeID, int itemID)
        {
            return this.user.shoppingCart.removeItemFromShoppingBag(storeID, itemID);

        }

        public override bool removeItemFromStorage(int storeID, int itemID)
        {
            throw new NotImplementedException();
        }

        public override bool removeStoreManager(string managerName, int storeID)
        {
            throw new NotImplementedException();
        }
    }
}
