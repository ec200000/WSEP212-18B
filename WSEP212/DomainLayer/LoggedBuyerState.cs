using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class LoggedBuyerState : UserState
    {
        public LoggedBuyerState(User user) : base(user)
        {

        }

        public override ResultWithValue<int> addItemToStorage(int storeID, int quantity, String itemName, String description, double price, ItemCategory category)
        {
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            {
                if(sellerPermissions.Value.StoreID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.StorageManagment))
                    {
                        return StoreRepository.Instance.getStore(sellerPermissions.Value.StoreID).getValue().addItemToStorage(quantity, itemName, description, price, category);
                    }
                    return new FailureWithValue<int>("The User Has No Permission To Add Item To This Store", -1);
                }
                sellerPermissions = sellerPermissions.Next; // checking the next store
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

        private RegularResult appointStoreSeller(string sellerName, int storeID, Permissions appointeepermission)
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

            if (sellerRes.getValue() == null)// check the manager is logged buyer - if he is subscriber in the system
                return new Failure("A Guest Cannot Be Appointed To Be A Store Manager");

            // checks if the user has the permissions for appointing store manager
            RegularResult hasPermissionRes;
            if(appointeepermission.Equals(Permissions.AllPermissions))
                hasPermissionRes = hasPermissionInStore(storeID, Permissions.AppointStoreOwner);
           else
                hasPermissionRes = hasPermissionInStore(storeID, Permissions.AppointStoreManager);
            if(hasPermissionRes.getTag())
            {
                User grantor = this.user;
                ConcurrentLinkedList<Permissions> pers = new ConcurrentLinkedList<Permissions>();
                pers.TryAdd(appointeepermission);  // new seller permissions
                SellerPermissions permissions = SellerPermissions.getSellerPermissions(sellerRes.getValue().userName, storeRes.getValue().storeID, grantor.userName, pers);
                RegularResult addSellerRes = storeRes.getValue().addNewStoreSeller(permissions);
                if (addSellerRes.getTag())
                {
                    if(sellerRes.getValue().addSellerPermissions(permissions))
                        return new Ok("The Appointment Of The New Seller To The Store Made Successfully");
                    return new Failure("Could not add seller permission");
                }
                return addSellerRes;
            }
            return hasPermissionRes;
        }
        
        public override RegularResult continueAsGuest(String userName)
        {
            throw new NotImplementedException();
        }

        private RegularResult hasPermissionInStore(int storeID, Permissions permission)
        {
            // checks if the user has the permissions for permission
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First;
            while (sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.StoreID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(permission))
                    {
                        return new Ok("The User Has Permission To Do The Action");
                    }
                    return new Failure("The User Has No Permission To Edit Store Manager Permissions");
                }
                sellerPermissions = sellerPermissions.Next; // checking the next store
            }
            return new Failure("The User Is Not Store Seller Of This Store");
        }

        public override RegularResult editItemDetails(int storeID, int itemID, int quantity, String itemName, String description, double price, ItemCategory category)
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
                    if (storeSellerRes.getValue().permissionsInStore.Contains(Permissions.AllPermissions))
                        return new Failure("Can't edit store's owner permissions!");
                    storeSellerRes.getValue().setPermissions(permissions);
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
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.StoreID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.size!=0)
                        return StoreRepository.Instance.getStore(sellerPermissions.Value.StoreID).getValue().getStoreOfficialsInfo(); // getting all users permissions
                }
                sellerPermissions = sellerPermissions.Next; // checking the next store
            }
            return null;
        }

        public override ConcurrentDictionary<int, PurchaseInvoice> getStorePurchaseHistory(int storeID)
        {
            if (!StoreRepository.Instance.getStore(storeID).getTag()) return null;
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            {
                if (sellerPermissions.Value.StoreID == storeID) // if the user works at the store
                {
                    if (sellerPermissions.Value.permissionsInStore.Contains(Permissions.AllPermissions) || sellerPermissions.Value.permissionsInStore.Contains(Permissions.GetOfficialsInformation))
                        return StoreRepository.Instance.getStore(sellerPermissions.Value.StoreID).getValue().purchasesHistory; // getting the store's purchase history
                }
                sellerPermissions = sellerPermissions.Next; // checking the next store
            }
            return null;
        }

        public override ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>> getStoresPurchaseHistory()
        {
            throw new NotImplementedException();
            // only system managers can do that
        }

        public override ConcurrentDictionary<String, ConcurrentDictionary<int, PurchaseInvoice>> getUsersPurchaseHistory()
        {
            throw new NotImplementedException();
            // only system managers can do that
        }

        public override ResultWithValue<ConcurrentLinkedList<string>> itemReview(String review, int itemID, int storeID)
        {
            if(review==null) return new FailureWithValue<ConcurrentLinkedList<string>>("Review Is Null",null);
            ResultWithValue<Store> getStoreRes = StoreRepository.Instance.getStore(storeID);
            if (getStoreRes.getTag())
            {
                ResultWithValue<Item> getItemRes = getStoreRes.getValue().getItemById(itemID);
                if (getItemRes.getTag())
                {
                    if(isPurchasedItem(itemID))
                    {
                        getItemRes.getValue().addReview(this.user.userName, review);
                        
                        return new OkWithValue<ConcurrentLinkedList<string>>("Item Review Have Been Successfully Added",
                            StoreRepository.Instance.getStoreOwners(storeID));
                    }
                    return new FailureWithValue<ConcurrentLinkedList<string>>("Unpurchased Product Cannot Be Reviewed",null);
                }
                return new FailureWithValue<ConcurrentLinkedList<string>>(getItemRes.getMessage(),null);
            }
            return new FailureWithValue<ConcurrentLinkedList<string>>(getStoreRes.getMessage(),null);
        }

        private bool isPurchasedItem(int itemID)
        {
            foreach (PurchaseInvoice purchaseInfo in this.user.purchases.Values)
            {
                if(purchaseInfo.wasItemPurchased(itemID))
                {
                    return true;
                }
            }
            return false;
        }

        public override RegularResult login(String userName, String password)
        {
            throw new NotImplementedException();
            // already logged in
        }

        public override RegularResult loginAsSystemManager(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public override RegularResult logout(String userName)
        {
            ResultWithValue<User> findUserRes = UserRepository.Instance.findUserByUserName(userName);
            if (findUserRes.getTag())
            {
                RegularResult loginStateRes = UserRepository.Instance.changeUserLoginStatus(findUserRes.getValue(), false, null);
                if (loginStateRes.getTag())
                {
                    this.user.changeState(new GuestBuyerState(this.user));
                    return new Ok("The User Has Successfully Logged Out");
                }
                return loginStateRes;
            }
            return new Failure(findUserRes.getMessage());
        }

        public override ResultWithValue<int> openStore(String storeName, String storeAddress, PurchasePolicyInterface purchasePolicy, SalePolicyInterface salesPolicy)
        {
            ResultWithValue<int> addStoreRes = StoreRepository.Instance.addStore(storeName, storeAddress, salesPolicy, purchasePolicy, this.user);
            if (addStoreRes.getTag())
            {
                int storeID = addStoreRes.getValue();
                ResultWithValue<SellerPermissions> sellerPermissionsRes = StoreRepository.Instance.getStore(storeID).getValue().getStoreSellerPermissions(this.user.userName);
                this.user.addSellerPermissions(sellerPermissionsRes.getValue());
            }
            return addStoreRes;
        }

        public override ResultWithValue<string> confirmPriceStatus(String userName, int storeID, int itemID, PriceStatus priceStatus)
        {
            // checks user exists
            ResultWithValue<User> findUserRes = UserRepository.Instance.findUserByUserName(userName);
            if (!findUserRes.getTag())
            {
                return new FailureWithValue<string>(findUserRes.getMessage(), userName);
            }
            // checks if the user has the permissions for confirm price status
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.ConfirmPurchasePrice);
            if (hasPermissionRes.getTag())
            {
                RegularResult res = findUserRes.getValue().shoppingCart.itemPriceStatusDecision(storeID, itemID, priceStatus);
                if(res.getTag())
                {
                    return new OkWithValue<string>(res.getMessage(), userName);
                }
                return new FailureWithValue<string>(res.getMessage(), userName);
            }
            return new FailureWithValue<string>(hasPermissionRes.getMessage(), userName);
        }

        public override ResultWithValue<string> itemCounterOffer(String userName, int storeID, int itemID, double counterOffer)
        {
            // checks user exists
            ResultWithValue<User> findUserRes = UserRepository.Instance.findUserByUserName(userName);
            if (!findUserRes.getTag())
            {
                return new FailureWithValue<string>(findUserRes.getMessage(), userName);
            }
            // checks if the user has the permissions for confirm price status
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.ConfirmPurchasePrice);
            if (hasPermissionRes.getTag())
            {
                RegularResult res = findUserRes.getValue().shoppingCart.itemCounterOffer(storeID, itemID, counterOffer);
                if (res.getTag())
                {
                    return new OkWithValue<string>(res.getMessage(), userName);
                }
                return new FailureWithValue<string>(res.getMessage(), userName);
            }
            return new FailureWithValue<string>(hasPermissionRes.getMessage(), userName);
        }

        public override RegularResult supportPurchaseType(int storeID, PurchaseType purchaseType)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new Failure(storeRes.getMessage());
            }
            // checks if the user has the permissions for support purchase type
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.StorePoliciesManagement);
            if (hasPermissionRes.getTag())
            {
                storeRes.getValue().supportPurchaseType(purchaseType);
                return new Ok("The Purchase Type Added And Will Be Supported In The Store");
            }
            return hasPermissionRes;
        }

        public override RegularResult unsupportPurchaseType(int storeID, PurchaseType purchaseType)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new Failure(storeRes.getMessage());
            }
            // checks if the user has the permissions for support purchase type
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.StorePoliciesManagement);
            if (hasPermissionRes.getTag())
            {
                storeRes.getValue().unsupportPurchaseType(purchaseType);
                return new Ok("The Purchase Type Removed And Will Not Be Supported In The Store");
            }
            return hasPermissionRes;
        }

        public override RegularResult register(User newUSer, string password)
        {
            throw new NotImplementedException();
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
                    if (storeSellerRes.getValue().permissionsInStore.Contains(Permissions.AllPermissions))
                        return new Failure("You can't remove a store owner!");
                    if (!storeSellerRes.getValue().GrantorName.Equals(this.user.userName))
                        return new Failure("Only who appointed you, can remove you!");
                    // remove him from the store
                    RegularResult removeFromStoreRes = storeRes.getValue().removeStoreSeller(storeSellerRes.getValue().SellerName);
                    if(removeFromStoreRes.getTag())
                    {
                        if(userRes.getValue().sellerPermissions.Contains(storeSellerRes.getValue()))
                        {
                            userRes.getValue().sellerPermissions.Remove(storeSellerRes.getValue(), out _);
                            return new Ok("Remove Store Manager Successfully");
                        }
                        return new Failure("The User Is Not Store Manager In This Store");
                    }
                    return new Failure(removeFromStoreRes.getMessage());
                }
                return new Failure(storeSellerRes.getMessage());
            }
            return new Failure(hasPermissionRes.getMessage());
        }
        
        public override RegularResult removeStoreOwner(string ownerName, int storeID)
        {
            // checks manager exists
            ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(ownerName);
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
                ResultWithValue<SellerPermissions> storeSellerRes = getStoreSellerPermissions(storeID, ownerName);
                if (storeSellerRes.getTag())
                {
                    if (!storeSellerRes.getValue().permissionsInStore.Contains(Permissions.AllPermissions))
                        return new Failure("You can't remove a store manager!");
                    if (!storeSellerRes.getValue().GrantorName.Equals(this.user.userName))
                        return new Failure("Only who appointed you, can remove you!");
                    // remove him from the store
                    RegularResult removeFromStoreRes = storeRes.getValue().removeStoreSeller(storeSellerRes.getValue().SellerName);
                    if(removeFromStoreRes.getTag())
                    {
                        if(userRes.getValue().sellerPermissions.Contains(storeSellerRes.getValue()))
                        {
                            userRes.getValue().sellerPermissions.Remove(storeSellerRes.getValue(), out _);
                            return new Ok("Remove Store Owner Successfully");
                        }
                        return new Failure("The User Is Not Store Owner In This Store");
                    }
                    return removeFromStoreRes;
                }
                return new Failure(storeSellerRes.getMessage());
            }
            return hasPermissionRes;
        }

        public override ResultWithValue<int> addPurchasePredicate(int storeID, Predicate<PurchaseDetails> newPredicate, String predDescription)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new FailureWithValue<int>(storeRes.getMessage(), -1);
            }
            // checks if the user has the permissions for adding purchase predicate
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.StorePoliciesManagement);
            if (hasPermissionRes.getTag())
            {
                int predicateID = storeRes.getValue().addPurchasePredicate(newPredicate, predDescription);
                return new OkWithValue<int>("The Purchase Predicate Added To The Store's Purchase Policy", predicateID);
            }
            return new FailureWithValue<int>(hasPermissionRes.getMessage(), -1);
        }

        public override RegularResult removePurchasePredicate(int storeID, int predicateID)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new Failure(storeRes.getMessage());
            }
            // checks if the user has the permissions for removing purchase predicate
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.StorePoliciesManagement);
            if (hasPermissionRes.getTag())
            {
                return storeRes.getValue().removePurchasePredicate(predicateID);
            }
            return hasPermissionRes;
        }

        public override ResultWithValue<int> composePurchasePredicates(int storeID, int firstPredicateID, int secondPredicateID, PurchasePredicateCompositionType typeOfComposition)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new FailureWithValue<int>(storeRes.getMessage(), -1);
            }
            // checks if the user has the permissions for adding purchase predicate
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.StorePoliciesManagement);
            if (hasPermissionRes.getTag())
            {
                return storeRes.getValue().composePurchasePredicates(firstPredicateID, secondPredicateID, typeOfComposition);
            }
            return new FailureWithValue<int>(hasPermissionRes.getMessage(), -1);
        }

        public override ResultWithValue<int> addSale(int storeID, int salePercentage, ApplySaleOn saleOn, String saleDescription)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new FailureWithValue<int>(storeRes.getMessage(), -1);
            }
            // checks if the user has the permissions for adding purchase predicate
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.StorePoliciesManagement);
            if (hasPermissionRes.getTag())
            {
                int saleID = storeRes.getValue().addSale(salePercentage, saleOn, saleDescription);
                return new OkWithValue<int>("The Sale Added To The Store's Sale Policy", saleID);
            }
            return new FailureWithValue<int>(hasPermissionRes.getMessage(), -1);
        }

        public override RegularResult removeSale(int storeID, int saleID)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new Failure(storeRes.getMessage());
            }
            // checks if the user has the permissions for removing purchase predicate
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.StorePoliciesManagement);
            if (hasPermissionRes.getTag())
            {
                return storeRes.getValue().removeSale(saleID);
            }
            return hasPermissionRes;
        }

        public override ResultWithValue<int> addSaleCondition(int storeID, int saleID, SimplePredicate condition, SalePredicateCompositionType compositionType)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new FailureWithValue<int>(storeRes.getMessage(), -1);
            }
            // checks if the user has the permissions for adding purchase predicate
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.StorePoliciesManagement);
            if (hasPermissionRes.getTag())
            {
                return storeRes.getValue().addSaleCondition(saleID, condition, compositionType);
            }
            return new FailureWithValue<int>(hasPermissionRes.getMessage(), -1);
        }

        public override ResultWithValue<int> composeSales(int storeID, int firstSaleID, int secondSaleID, SaleCompositionType typeOfComposition, SimplePredicate selectionRule)
        {
            // checks store exists
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new FailureWithValue<int>(storeRes.getMessage(), -1);
            }
            // checks if the user has the permissions for adding purchase predicate
            RegularResult hasPermissionRes = hasPermissionInStore(storeID, Permissions.StorePoliciesManagement);
            if (hasPermissionRes.getTag())
            {
                return storeRes.getValue().composeSales(firstSaleID, secondSaleID, typeOfComposition, selectionRule);
            }
            return new FailureWithValue<int>(hasPermissionRes.getMessage(), -1);
        }

        public override ConcurrentLinkedList<int> getUsersStores()
        {
            ConcurrentLinkedList<int> list = new ConcurrentLinkedList<int>();
            Node<SellerPermissions> sellerPermissions = this.user.sellerPermissions.First; // going over the user's permissions to check if he is a store manager or owner
            while(sellerPermissions.Value != null)
            {
                list.TryAdd(sellerPermissions.Value.StoreID);
                sellerPermissions = sellerPermissions.Next;
            }
            return list;
        }
    }
}
