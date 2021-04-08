using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.Result;

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

        public override RegularResult addItemToShoppingCart(int storeID, int itemID, int quantity)
        {
            return this.user.shoppingCart.addItemToShoppingBag(storeID, itemID, quantity);
        }

        public override ResultWithValue<int> addItemToStorage(int storeID, int quantity, String itemName, String description, double price, String category)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First;
            while(sellerPermissions.Value != null)
            {
                if(sellerPermissions.Value.store.storeID == storeID)
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.StorageManagment))
                    {
                        return sellerPermissions.Value.store.addItemToStorage(quantity, itemName, description, price, category);
                    }
                    return new FailureWithValue<int>("The User Has No Permission To Add Item To This Store", -1);
                }
                sellerPermissions = sellerPermissions.Next;
            }
            return new FailureWithValue<int>("The User Is Not Store Seller Of This Store", -1);
        }

        public override RegularResult appointStoreManager(String managerName, int storeID)
        {
            return appointStoreSeller(managerName, storeID, Permissions.GetOfficialsInformation);
        }

        public override RegularResult appointStoreOwner(string storeOwnerName, int storeID)
        {
            return appointStoreSeller(storeOwnerName, storeID, Permissions.AllPermissions);
        }

        private RegularResult appointStoreSeller(string sellerName, int storeID, Permissions permission)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new Failure(storeRes.getMessage());
            }
            // checks new manager exists
            ResultWithValue<User> sellerRes = UserRepository.Instance.findUserByUserName(sellerName);
            if (!sellerRes.getTag())
            {
                return new Failure(sellerRes.getMessage());
            }
            // check the manager is logged buyer - if he is subscriber in the system
            // TOTO HERE
            // return new Failure("A Guest Cannot Be Appointed To Be A Store Manager");

            // checks if the user has the permissions for appointing store manager
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, permission);
            if(hasPermissionRes.getTag())
            {
                User grantor = this.user;
                ConcurrentLinkedList<Permissions> pers = new ConcurrentLinkedList<Permissions>();
                pers.TryAdd(permission);  // new seller permissions
                SellerPermissions permissions = SellerPermissions.getSellerPermissions(sellerRes.getValue(), storeRes.getValue(), grantor, pers);
                RegularResult addSellerRes = storeRes.getValue().addNewStoreSeller(permissions);
                if (addSellerRes.getTag())
                {
                    sellerRes.getValue().sellerPermissions.TryAdd(permissions);
                    return new Ok("The Appointment Of The New Seller To The Store Made Successfully");
                }
                return addSellerRes;
            }
            return hasPermissionRes;
        }

        private RegularResult hasPermissionInStore(int storeID, Permissions permission)
        {
            // checks if the user has the permissions for permission
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First;
            while (sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.store.storeID == storeID)
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(permission))
                    {
                        return new Ok("The User Has Permission To Do The Action");
                    }
                    return new Failure("The User Has No Permission To Edit Store Manager Permissions");
                }
                sellerPermissions = sellerPermissions.Next;
            }
            return new Failure("The User Is Not Store Seller Of This Store");
        }

        public override RegularResult editItemDetails(int storeID, int itemID, int quantity, String itemName, String description, double price, String category)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new Failure(storeRes.getMessage());
            }
            // checks if the user has the permissions for editing item details
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.StorageManagment);
            if (hasPermissionRes.getTag())
            {
                return storeRes.getValue().editItem(itemID, itemName, description, price, category, quantity);
            }
            return hasPermissionRes;
        }

        public override RegularResult editManagerPermissions(string managerName, ConcurrentLinkedList<Permissions> permissions, int storeID)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new Failure(storeRes.getMessage());
            }
            // checks if the user has the permissions for editing store manager permissions
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.EditManagmentPermissions);
            if (hasPermissionRes.getTag())
            {
                // checks the manager is indeed the manager in that store
                ResultWithValue<SellerPermissions> storeSellerRes = getStoreSellerPermissions(storeID, managerName);
                if(storeSellerRes.getTag())
                {
                    storeSellerRes.getValue().permissionsInStore = permissions;
                    return new Ok("Edit Manager Permissions Successfully");
                }
                return new Failure(storeSellerRes.getMessage());
            }
            return hasPermissionRes;
        }

        private ResultWithValue<SellerPermissions> getStoreSellerPermissions(int storeID, String userName)
        {
            ResultWithValue<Store> getStoreRes = StoreRepository.Instance.getStore(storeID);
            if(getStoreRes.getTag())
            {
                return getStoreRes.getValue().getStoreSellerPermissions(userName);
            }
            return new FailureWithValue<SellerPermissions>(getStoreRes.getMessage(), null);
        }

        public override ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>> getOfficialsInformation(int storeID)
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

        public override RegularResult itemReview(String review, int itemID, int storeID)
        {
            ResultWithValue<Store> getStoreRes = StoreRepository.Instance.getStore(storeID);
            if (getStoreRes.getTag())
            {
                ResultWithValue<Item> getItemRes = getStoreRes.getValue().getItemById(itemID);
                if (getItemRes.getTag())
                {
                    if(isPurchasedItem(itemID))
                    {
                        getItemRes.getValue().addReview(this.user.userName, review);
                        return new Ok("Item Review Have Been Successfully Added");
                    }
                    return new Failure("Unpurchased Product Cannot Be Reviewed");
                }
                return new Failure(getItemRes.getMessage());
            }
            return new Failure(getStoreRes.getMessage());
        }

        private bool isPurchasedItem(int itemID)
        {
            foreach (PurchaseInfo purchaseInfo in this.user.purchases)
            {
                if(purchaseInfo.items.ContainsKey(itemID))
                {
                    return true;
                }
            }
            return false;
        }

        public override RegularResult login(String userName, String password)
        {
            throw new NotImplementedException();
        }

        public override RegularResult logout(String userName)
        {
            ResultWithValue<User> findUserRes = UserRepository.Instance.findUserByUserName(userName);
            if (findUserRes.getTag())
            {
                RegularResult loginStateRes = UserRepository.Instance.changeUserLoginStatus(findUserRes.getValue(), true, null);
                if (loginStateRes.getTag())
                {
                    this.user.changeState(new GuestBuyerState(this.user));
                    return new Ok("The User Has Successfully Logged Out");
                }
                return loginStateRes;
            }
            return new Failure(findUserRes.getMessage());
        }

        public override ResultWithValue<int> openStore(String storeName, String storeAddress, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            ResultWithValue<int> addStoreRes = StoreRepository.Instance.addStore(storeName, storeAddress, salesPolicy, purchasePolicy, this.user);
            if (addStoreRes.getTag())
            {
                int storeID = addStoreRes.getValue();
                ResultWithValue<SellerPermissions> sellerPermissionsRes = StoreRepository.Instance.getStore(storeID).getValue().getStoreSellerPermissions(this.user.userName);
                this.user.sellerPermissions.TryAdd(sellerPermissionsRes.getValue());
            }
            return addStoreRes;
        }

        public override RegularResult purchaseItems(string address)
        {
            return HandlePurchases.Instance.purchaseItems(this.user, address);
        }

        public override RegularResult register(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public override RegularResult removeItemFromShoppingCart(int storeID, int itemID)
        {
            return this.user.shoppingCart.removeItemFromShoppingBag(storeID, itemID);
        }

        public override RegularResult removeItemFromStorage(int storeID, int itemID)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new Failure(storeRes.getMessage());
            }
            // checks if the user has the permissions for removing item from storage
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.StorageManagment);
            if (hasPermissionRes.getTag())
            {
                return storeRes.getValue().removeItemFromStorage(itemID);
            }
            return hasPermissionRes;
        }

        public override RegularResult removeStoreManager(string managerName, int storeID)
        {
            // checks manager exists
            ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(managerName);
            if (!userRes.getTag())
            {
                return new Failure(userRes.getMessage());
            }
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new Failure(storeRes.getMessage());
            }
            // checks if the user has the permissions for removing store manager
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.RemoveStoreManager);
            if (hasPermissionRes.getTag())
            {
                // checks the manager is indeed the manager in that store
                ResultWithValue<SellerPermissions> storeSellerRes = getStoreSellerPermissions(storeID, managerName);
                if (storeSellerRes.getTag())
                {
                    // remove him from the store
                    RegularResult removeFromStoreRes = storeRes.getValue().removeStoreSeller(storeSellerRes.getValue().seller.userName);
                    if(removeFromStoreRes.getTag())
                    {
                        if(userRes.getValue().sellerPermissions.Contains(storeSellerRes.getValue()))
                        {
                            userRes.getValue().sellerPermissions.Remove(storeSellerRes.getValue(), out _);
                            return new Ok("Remove Store Manager Successfully");
                        }
                        return new Failure("The User Is Not Store Manager In This Store");
                    }
                    return removeFromStoreRes;
                }
                return new Failure(storeSellerRes.getMessage());
            }
            return hasPermissionRes;
        }
    }
}
