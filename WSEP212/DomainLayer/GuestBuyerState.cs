using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class GuestBuyerState : UserState
    {
        public GuestBuyerState(User user) : base(user)
        {
        }

        public override bool addItemToShoppingCart(int storeID, int itemID, int quantity)
        {
            return this.user.shoppingCart.addItemToShoppingBag(storeID, itemID, quantity);
        }

        public override bool addItemToStorage(int storeID, Item item, int quantity)
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

        public override bool editItemDetails(int storeID, Item item)
        {
            throw new NotImplementedException();
        }

        public override bool editManagerPermissions(string managerName, ConcurrentBag<Permissions> permissions, int storeID)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionary<User, ConcurrentBag<Permissions>> getOfficialsInformation(int storeID)
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
            return StoreRepository.Instance.getStore(storeID).getItemById(itemID).addReview(this.user.userName, review);
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
            throw new NotImplementedException();
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

        public override bool removeItemFromStorage(int storeID, Item item)
        {
            throw new NotImplementedException();
        }

        public override bool removeStoreManager(string managerName, int storeID)
        {
            throw new NotImplementedException();
        }
    }
}
