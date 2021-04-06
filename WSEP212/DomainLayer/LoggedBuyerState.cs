using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class LoggedBuyerState : UserState
    {
        public LoggedBuyerState(User user) : base(user)
        {

        }

        // return the state of the user
        // checks if the user is store owner, store manager or none of them
        public override UserType getUserType()
        {
            // check store owner or store manager
            return UserType.LoggedBuyer;
        }

        public override bool addItemToShoppingCart(int storeID, int itemID, int quantity)
        {
            return this.user.shoppingCart.addItemToShoppingBag(storeID, itemID, quantity);
        }

        public override bool addItemToStorage(int storeID, Item item)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First;
            while(sellerPermissions.Value != null)
            {
                if(sellerPermissions.Value.store.storeID == storeID)
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.StorageManagment))
                            return sellerPermissions.Value.store.addItemToStorage(item);
                }
                sellerPermissions = sellerPermissions.Next;
            }
            return false;
        }

        public override bool appointStoreManager(string managerName, int storeID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First;
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID)
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.AppointStoreManager))
                    {
                        User seller = UserRepository.Instance.findUserByUserName(managerName);
                        User grantor = this.user;
                        Store store = StoreRepository.Instance.getStore(storeID);
                        ConcurrentLinkedList<Permissions> pers = new ConcurrentLinkedList<Permissions>();
                        if (pers.TryAdd(Permissions.GetOfficialsInformation))
                        {
                            SellerPermissions permissions =
                                SellerPermissions.getSellerPermissions(seller, store, grantor, pers);
                            if (sellerPermissions.Value.store.addNewStoreSeller(permissions))
                            {
                                return seller.sellerPermissions.TryAdd(permissions);
                            }
                        }
                    }
                }
                sellerPermissions = sellerPermissions.Next;
            }
            return false;
        }

        public override bool appointStoreOwner(string storeOwnerName, int storeID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First;
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID)
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.AppointStoreOwner))
                    {
                        User seller = UserRepository.Instance.findUserByUserName(storeOwnerName);
                        User grantor = this.user;
                        Store store = StoreRepository.Instance.getStore(storeID);
                        ConcurrentLinkedList<Permissions> pers = new ConcurrentLinkedList<Permissions>();
                        if (pers.TryAdd(Permissions.AllPermissions))
                        {
                            SellerPermissions permissions =
                                SellerPermissions.getSellerPermissions(seller, store, grantor, pers);
                            if (sellerPermissions.Value.store.addNewStoreSeller(permissions))
                            {
                                return seller.sellerPermissions.TryAdd(permissions);
                            }
                        }
                    }
                }
                sellerPermissions = sellerPermissions.Next;
            }
            return false;
        }

        

        public override bool editItemDetails(int storeID, Item item)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First;
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID)
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.StorageManagment))
                        return sellerPermissions.Value.store.editItem(item.itemID,item.itemName,item.description,item.price,item.category, item.quantity); 
                }
                sellerPermissions = sellerPermissions.Next;
            }
            return false;
        }

        public override bool editManagerPermissions(string managerName, ConcurrentLinkedList<Permissions> permissions, int storeID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First;
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID)
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.EditManagmentPermissions))
                    {
                        Node<SellerPermissions> managerPermissions = UserRepository.Instance.findUserByUserName(managerName).sellerPermissions.First;
                        while(managerPermissions.Value != null)
                        {
                            if (managerPermissions.Value.store.storeID == storeID)
                            {
                                managerPermissions.Value.permissionsInStore = permissions;
                                return true;
                            }
                            managerPermissions = managerPermissions.Next;
                        }
                    }
                }
                sellerPermissions = sellerPermissions.Next;
            }
            return false;
        }

        public override ConcurrentDictionary<User, ConcurrentLinkedList<Permissions>> getOfficialsInformation(int storeID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First;
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID)
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.GetOfficialsInformation))
                        return sellerPermissions.Value.store.getStoreOfficialsInfo();
                }
                sellerPermissions = sellerPermissions.Next;
            }
            return null;
        }

        public override ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(int storeID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First;
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID)
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.GetOfficialsInformation))
                        return sellerPermissions.Value.store.purchasesHistory;
                }
                sellerPermissions = sellerPermissions.Next;
            }
            return null;
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
            throw new NotImplementedException();
        }

        public override bool logout(string userName)
        {
            if (UserRepository.Instance.changeUserLoginStatus(UserRepository.Instance.findUserByUserName(userName), false, null))
            {
                this.user.changeState(new GuestBuyerState(this.user));
                return true;
            }
            return false;
        }

        public override bool openStore(string storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            Store store = new Store(storeName, salesPolicy, purchasePolicy, this.user);
            int storeID = store.storeID;
            if (StoreRepository.Instance.addStore(store))
            {
                return this.user.sellerPermissions.TryAdd(StoreRepository.Instance.stores[storeID]
                    .storeSellersPermissions[this.user.userName]);
            }

            return false;
        }

        public override bool purchaseItems(string address)
        {
            throw new NotImplementedException(); //TODO: COMPLETE
        }

        public override bool register(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public override bool removeItemFromShoppingCart(int storeID, int itemID)
        {
            return this.user.shoppingCart.removeItemFromShoppingBag(storeID, itemID);

        }

        public override bool removeItemFromStorage(int storeID, int itemID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First;
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID)
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.StorageManagment))
                        return sellerPermissions.Value.store.removeItemFromStorage(itemID);
                }
                sellerPermissions = sellerPermissions.Next;
            }
            return false;
        }

        public override bool removeStoreManager(string managerName, int storeID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First;
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID)
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.EditManagmentPermissions))
                    {
                        Node<SellerPermissions> managerPermissions = UserRepository.Instance.findUserByUserName(managerName).sellerPermissions.First;
                        while(managerPermissions.Value != null)
                        {
                            if (managerPermissions.Value.store.storeID == storeID)
                            {
                                SellerPermissions result;
                                if (UserRepository.Instance.findUserByUserName(managerName).sellerPermissions
                                    .Remove(managerPermissions.Value, out result))
                                {
                                    SellerPermissions result2;
                                    return StoreRepository.Instance.stores[storeID].storeSellersPermissions
                                        .TryRemove(managerName, out result2);
                                }
                            }
                            managerPermissions = managerPermissions.Next;
                        }
                    }
                }
                sellerPermissions = sellerPermissions.Next;
            }
            return false;
        }
    }
}
