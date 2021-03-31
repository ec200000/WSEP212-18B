using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class SystemManagerState : LoggedBuyerState
    {
        public SystemManagerState(User user) : base(user)
        {

        }

        public override bool addItemToShoppingCart(int storeID, int itemID)
        {
            throw new NotImplementedException();
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

        public override bool editManagerPermissions(string managerName, LinkedList<Permissions> permissions)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, LinkedList<Permissions>> getOfficialsInformation(int storeID)
        {
            throw new NotImplementedException();
        }

        public override LinkedList<PurchaseInfo> getStorePurchaseHistory(int storeID)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<int, LinkedList<PurchaseInfo>> getStoresPurchaseHistory()
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, LinkedList<PurchaseInfo>> getUsersPurchaseHistory()
        {
            throw new NotImplementedException();
        }

        public override bool itemReview(string review, int itemID, int storeID)
        {
            throw new NotImplementedException();
        }

        public override bool login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public override bool logout(string userName)
        {
            throw new NotImplementedException();
        }

        public override bool openStore(string storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            throw new NotImplementedException();
        }

        public override bool purchaseItems()
        {
            throw new NotImplementedException();
        }

        public override bool register(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public override bool removeItemFromShoppingCart(int storeID, int itemID)
        {
            throw new NotImplementedException();
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
