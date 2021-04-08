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
            // adding a quantity of the item to the shopping bag that belongs to the store id
        }

        public override bool addItemToStorage(int storeID, int quantity, String itemName, String description, double price, String category)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            {
                if(sellerPermissions.Value.store.storeID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) ||
                        sellerPermissions.Value.permissionsInStore.Contains(Permissions.StorageManagment))
                    {
                        // checking if the user have the needed permissions
                        return sellerPermissions.Value.store.addItemToStorage(quantity, itemName, description, price,
                            category) > 0;
                        // adding the item to the storage
                    }
                }
                sellerPermissions = sellerPermissions.Next; // going to the next store if it's not the one we want
            }
            return false;
        }

        public override bool appointStoreManager(string managerName, int storeID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            { 
                if (sellerPermissions.Value.store.storeID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || 
                        sellerPermissions.Value.permissionsInStore.Contains(Permissions.AppointStoreManager))
                    {
                        // checking if the user have the needed permissions
                        User seller = UserRepository.Instance.findUserByUserName(managerName);
                        if (user.state is LoggedBuyerState)
                        {
                            User grantor = this.user; // appoints the user
                            Store store = StoreRepository.Instance.getStore(storeID); 
                            ConcurrentLinkedList<Permissions> pers = new ConcurrentLinkedList<Permissions>();
                            if (pers.TryAdd(Permissions.GetOfficialsInformation)) // basic manager permission
                            {
                                SellerPermissions permissions =
                                    SellerPermissions.getSellerPermissions(seller, store, grantor, pers);
                                if (sellerPermissions.Value.store.addNewStoreSeller(permissions))
                                {
                                    return seller.sellerPermissions.TryAdd(permissions);
                                }
                                return false;
                            }
                            return false;
                        }
                        return false;
                    }
                }
                sellerPermissions = sellerPermissions.Next; // checking the next store
            }
            return false;
        }

        public override bool appointStoreOwner(string storeOwnerName, int storeID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || 
                        sellerPermissions.Value.permissionsInStore.Contains(Permissions.AppointStoreOwner))
                    {
                        // checking if the user have the needed permissions
                        User seller = UserRepository.Instance.findUserByUserName(storeOwnerName);
                        if (user.state is LoggedBuyerState)
                        {
                            User grantor = this.user; // appoints the user
                            Store store = StoreRepository.Instance.getStore(storeID);
                            ConcurrentLinkedList<Permissions> pers = new ConcurrentLinkedList<Permissions>();
                            if (pers.TryAdd(Permissions.AllPermissions)) // owner permissions
                            {
                                SellerPermissions permissions =
                                    SellerPermissions.getSellerPermissions(seller, store, grantor, pers);
                                if (sellerPermissions.Value.store.addNewStoreSeller(permissions))
                                {
                                    return seller.sellerPermissions.TryAdd(permissions);
                                }
                            }
                        }
                        return false;
                    }
                }
                sellerPermissions = sellerPermissions.Next; // checking the next store
            }
            return false;
        }

        

        public override bool editItemDetails(int storeID, int itemID, int quantity, String itemName, String description, double price, String category)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) ||
                        sellerPermissions.Value.permissionsInStore.Contains(Permissions.StorageManagment))
                    {
                        // checking if the user have the needed permissions
                        return sellerPermissions.Value.store.editItem(itemID, itemName, description, price, category,
                            quantity);
                    }
                }
                sellerPermissions = sellerPermissions.Next; // checking the next store
            }
            return false;
        }

        public override bool editManagerPermissions(string managerName, ConcurrentLinkedList<Permissions> permissions, int storeID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.EditManagmentPermissions))
                    { 
                        // checking if the user have the needed permissions
                        Node<SellerPermissions> managerPermissions = UserRepository.Instance.findUserByUserName(managerName).sellerPermissions.First;
                        while(managerPermissions.Value != null)
                        {
                            if (managerPermissions.Value.store.storeID == storeID) // if the user works at the store
                            {
                                managerPermissions.Value.permissionsInStore = permissions; // giving the user his new permissions
                                return true;
                            }
                            managerPermissions = managerPermissions.Next; // checking the next store 
                        }
                    }
                }
                sellerPermissions = sellerPermissions.Next; // checking the next store
            }
            return false;
        }

        public override ConcurrentDictionary<User, ConcurrentLinkedList<Permissions>> getOfficialsInformation(int storeID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.GetOfficialsInformation))
                        return sellerPermissions.Value.store.getStoreOfficialsInfo(); // getting all users permissions
                }
                sellerPermissions = sellerPermissions.Next; // checking the next store
            }
            return null;
        }

        public override ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(int storeID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.GetOfficialsInformation))
                        return sellerPermissions.Value.store.purchasesHistory; // getting the store's purchase history
                }
                sellerPermissions = sellerPermissions.Next; // checking the next store
            }
            return null;
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
            if (StoreRepository.Instance.getStore(storeID) != null)
            {
                if (StoreRepository.Instance.getStore(storeID).getItemById(itemID)!=null)
                {
                    return StoreRepository.Instance.getStore(storeID).getItemById(itemID).addReview(this.user.userName, review); // adding the review to the item
                }
                return false;
            }
            return false;
        }

        public override bool login(string userName, string password)
        {
            throw new NotImplementedException();
            // already logged in
        }

        public override bool logout(string userName)
        {
            if (UserRepository.Instance.changeUserLoginStatus(UserRepository.Instance.findUserByUserName(userName), false, null))
            {
                this.user.changeState(new GuestBuyerState(this.user)); // changing the user's state to guest
                return true;
            }
            return false;
        }

        public override bool openStore(string storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            Store store = new Store(storeName, salesPolicy, purchasePolicy, this.user); // creates the store
            int storeID = store.storeID;
            if (StoreRepository.Instance.addStore(store))
            {
                return this.user.sellerPermissions.TryAdd(StoreRepository.Instance.stores[storeID]
                    .storeSellersPermissions[this.user.userName]); // adding the store
            }
            return false;
        }

        public override bool purchaseItems(string address)
        {
            if (address == null)
                return false;
            return HandlePurchases.Instance.purchaseItems(this.user, address); // handles the purchase procedure
        }

        public override bool register(string userName, string password)
        {
            throw new NotImplementedException();
            // can't register, already logged in.
        }

        public override bool removeItemFromShoppingCart(int storeID, int itemID)
        {
            return this.user.shoppingCart.removeItemFromShoppingBag(storeID, itemID); 
            // removing the item from the shopping bag of the store
        }

        public override bool removeItemFromStorage(int storeID, int itemID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.StorageManagment))
                        return sellerPermissions.Value.store.removeItemFromStorage(itemID); // removes the item from the store's storage
                }
                sellerPermissions = sellerPermissions.Next; // checking the next store
            }
            return false;
        }

        public override bool removeStoreManager(string managerName, int storeID)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.EditManagmentPermissions))
                    {
                        Node<SellerPermissions> managerPermissions = UserRepository.Instance.findUserByUserName(managerName).sellerPermissions.First;
                        while(managerPermissions.Value != null)
                        {
                            if (managerPermissions.Value.store.storeID == storeID) // if the user works at the store
                            {
                                SellerPermissions result;
                                if (UserRepository.Instance.findUserByUserName(managerName).sellerPermissions
                                    .Remove(managerPermissions.Value, out result)) // removing his store permission
                                {
                                    SellerPermissions result2;
                                    return StoreRepository.Instance.stores[storeID].storeSellersPermissions
                                        .TryRemove(managerName, out result2); // removing the user from the people with permissions in the store
                                }
                            }
                            managerPermissions = managerPermissions.Next; // checking the next store
                        }
                    }
                    return false;
                }
                sellerPermissions = sellerPermissions.Next; // checking the next store
            }
            return false;
        }
    }
}
