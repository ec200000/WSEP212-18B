using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.Result;

namespace WSEP212.DomainLayer
{
    public class SystemManagerState : LoggedBuyerState
    {
        public SystemManagerState(User user) : base(user)
        {

        }

        public override RegularResult addItemToShoppingCart(int storeID, int itemID, int quantity)
        {
            throw new NotImplementedException();
        }

        public override ResultWithValue<int> addItemToStorage(int storeID, int quantity, String itemName, String description, double price, String category)
        {
            throw new NotImplementedException();
        }

        public override RegularResult appointStoreManager(string managerName, int storeID)
        {
            throw new NotImplementedException();
        }

        public override RegularResult appointStoreOwner(string storeOwnerName, int storeID)
        {
            throw new NotImplementedException();
        }

        public override RegularResult editItemDetails(int storeID, int itemID, int quantity, String itemName, String description, double price, String category)
        {
            throw new NotImplementedException();
        }

        public override RegularResult editManagerPermissions(string managerName, ConcurrentLinkedList<Permissions> permissions, int storeID)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>> getOfficialsInformation(int storeID)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(int storeID)
        {
            throw new NotImplementedException();
        }

        public override ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getStoresPurchaseHistory()
        {
            return StoreRepository.Instance.getAllStoresPurchsesHistory();
        }

        public override ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>> getUsersPurchaseHistory()
        {
            return UserRepository.Instance.getAllUsersPurchaseHistory();
        }

        public override RegularResult itemReview(string review, int itemID, int storeID)
        {
            throw new NotImplementedException();
        }

        public override ResultWithValue<int> openStore(String storeName, String storeAddress, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            throw new NotImplementedException();
        }

        public override RegularResult purchaseItems(string address)
        {
            throw new NotImplementedException();
        }

        public override RegularResult removeItemFromShoppingCart(int storeID, int itemID)
        {
            throw new NotImplementedException();
        }

        public override RegularResult removeItemFromStorage(int storeID, int itemID)
        {
            throw new NotImplementedException();
        }

        public override RegularResult removeStoreManager(string managerName, int storeID)
        {
            throw new NotImplementedException();
        }
    }
}
