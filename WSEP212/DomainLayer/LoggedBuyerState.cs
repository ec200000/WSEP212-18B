using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class LoggedBuyerState : UserState
    {
        public LoggedBuyerState(User user) : base(user)
        {

        }

        public override bool addItemToShoppingCart(int storeID, int itemID, int quantity)
        {
            return this.user.shoppingCart.addItemToShoppingBag(storeID, itemID, quantity);
        }

        public override bool addItemToStorage(int storeID, Item item, int quantity)
        {
            foreach(SellerPermissions sellerPermissions in this.user.sellerPermissions){
                if(sellerPermissions.store.storeID == storeID)
                {
                    if (Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.AllPermissions) || Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.StorageManagment))
                        return sellerPermissions.store.addItemToStorage(item, quantity);
                }
            }
            return false;
        }

        public override bool appointStoreManager(string managerName, int storeID)
        {
            foreach (SellerPermissions sellerPermissions in this.user.sellerPermissions)
            {
                if (sellerPermissions.store.storeID == storeID)
                {
                    if (Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.AllPermissions) || Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.AppointStoreManager))
                    { //only GetOfficialsInformation
                        User seller = UserRepository.Instance.findUserByUserName(managerName);
                        User grantor = this.user;
                        Store store = StoreRepository.Instance.getStore(storeID);
                        ConcurrentBag<Permissions> pers = new ConcurrentBag<Permissions>();
                        pers.Add(Permissions.GetOfficialsInformation);
                        SellerPermissions permissions = SellerPermissions.getSellerPermissions(seller, store, grantor, pers);
                        sellerPermissions.store.addNewStoreSeller(permissions);
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool appointStoreOwner(string storeOwnerName, int storeID)
        {
            foreach (SellerPermissions sellerPermissions in this.user.sellerPermissions)
            {
                if (sellerPermissions.store.storeID == storeID)
                {
                    if (Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.AllPermissions) || Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.AppointStoreOwner))
                    {
                        User seller = UserRepository.Instance.findUserByUserName(storeOwnerName);
                        User grantor = this.user;
                        Store store = StoreRepository.Instance.getStore(storeID);
                        ConcurrentBag<Permissions> pers = new ConcurrentBag<Permissions>();
                        pers.Add(Permissions.AllPermissions);
                        SellerPermissions permissions = SellerPermissions.getSellerPermissions(seller, store, grantor, pers);
                        sellerPermissions.store.addNewStoreSeller(permissions);
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool editItemDetails(int storeID, Item item)
        {
            foreach (SellerPermissions sellerPermissions in this.user.sellerPermissions)
            {
                if (sellerPermissions.store.storeID == storeID)
                {
                    if (Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.AllPermissions) || Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.StorageManagment))
                        return sellerPermissions.store.editItem(item.itemID,item.itemName,item.description,item.price,item.category);
                }
            }
            return false;
        }

        public override bool editManagerPermissions(string managerName, ConcurrentBag<Permissions> permissions, int storeID)
        {
            foreach (SellerPermissions sellerPermissions in this.user.sellerPermissions)
            {
                if (sellerPermissions.store.storeID == storeID)
                {
                    if (Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.AllPermissions) || Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.EditManagmentPermissions))
                    {
                        foreach (SellerPermissions managerPermissions in UserRepository.Instance.findUserByUserName(managerName).sellerPermissions)
                        {
                            if (sellerPermissions.store.storeID == storeID)
                            {
                                sellerPermissions.permissionsInStore = permissions;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public override ConcurrentDictionary<User, ConcurrentBag<Permissions>> getOfficialsInformation(int storeID)
        {
            foreach (SellerPermissions sellerPermissions in this.user.sellerPermissions)
            {
                if (sellerPermissions.store.storeID == storeID)
                {
                    if (Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.AllPermissions) || Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.GetOfficialsInformation))
                        return sellerPermissions.store.getStoreOfficialsInfo();
                }
            }
            return null;
        }

        public override ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(int storeID)
        {
            foreach (SellerPermissions sellerPermissions in this.user.sellerPermissions)
            {
                if (sellerPermissions.store.storeID == storeID)
                {
                    if (Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.AllPermissions) || Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.GetOfficialsInformation))
                        return sellerPermissions.store.purchasesHistory;
                }
            }
            return null;
        }

        public override ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getStoresPurchaseHistory()
        {
            throw new NotImplementedException(); //only in system manager
        }

        public override ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>> getUsersPurchaseHistory()
        {
            throw new NotImplementedException(); //only in system manager
        }

        public override bool itemReview(string review, int itemID, int storeID)
        {
            return StoreRepository.Instance.getStore(storeID).getItemById(itemID).addReview(this.user.userName, review);
        }

        public override bool login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public override bool logout(string userName)
        {
            if (UserRepository.Instance.changeUserLoginStatus(this.user, false, null))
            {
                this.user.state = new GuestBuyerState(this.user);
                return true;
            }
            return false;
        }

        public override bool openStore(string storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            return StoreRepository.Instance.addStore(new Store(salesPolicy, purchasePolicy, this.user, storeName));
        }

        public override bool purchaseItems(string address)
        {
            throw new NotImplementedException();
        }

        public override bool register(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public override bool removeItemFromShoppingCart(int storeID, int itemID)
        {
            return this.user.shoppingCart.removeItemFromShoppingBag(storeID, itemID);

        }

        public override bool removeItemFromStorage(int storeID, Item item) //TODO: check if we can change signature
        {
            foreach (SellerPermissions sellerPermissions in this.user.sellerPermissions)
            {
                if (sellerPermissions.store.storeID == storeID)
                {
                    if (Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.AllPermissions) || Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.StorageManagment))
                        return sellerPermissions.store.removeItemFromStorage(item.itemID);
                }
            }
            return false;
        }

        public override bool removeStoreManager(string managerName, int storeID)
        {
            foreach (SellerPermissions sellerPermissions in this.user.sellerPermissions)
            {
                if (sellerPermissions.store.storeID == storeID)
                {
                    if (Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.AllPermissions) || Array.Exists(sellerPermissions.permissionsInStore.ToArray(), element => element == Permissions.EditManagmentPermissions))
                    {
                        foreach (SellerPermissions managerPermissions in UserRepository.Instance.findUserByUserName(managerName).sellerPermissions)
                        {
                            if (sellerPermissions.store.storeID == storeID)
                            {
                                sellerPermissions.permissionsInStore = null;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
