using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class GuestBuyerState : UserState
    {
        public GuestBuyerState(User user) : base(user)
        {
        }

        public override ResultWithValue<int> addItemToStorage(int storeID, int quantity, String itemName, String description, double price, String category)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override ResultWithValue<int> addPurchasePredicate(int storeID, Predicate<PurchaseDetails> newPredicate)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override ResultWithValue<int> addSale(int storeID, int salePercentage, ApplySaleOn saleOn)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override ResultWithValue<int> addSaleCondition(int storeID, int saleID, Predicate<PurchaseDetails> condition, SalePredicateCompositionType compositionType)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override RegularResult appointStoreManager(string managerName, int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override RegularResult appointStoreOwner(string storeOwnerName, int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override ResultWithValue<int> composePurchasePredicates(int storeID, int firstPredicateID, int secondPredicateID, PurchasePredicateCompositionType typeOfComposition)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override ResultWithValue<int> composeSales(int storeID, int firstSaleID, int secondSaleID, SaleCompositionType typeOfComposition, Predicate<PurchaseDetails> selectionRule)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override RegularResult editItemDetails(int storeID, int itemID, int quantity, String itemName, String description, double price, String category)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override RegularResult editManagerPermissions(string managerName, ConcurrentLinkedList<Permissions> permissions, int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>> getOfficialsInformation(int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override ConcurrentBag<PurchaseInvoice> getStorePurchaseHistory(int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>> getStoresPurchaseHistory()
        {
            throw new NotImplementedException();
            // only system managers can do that
        }

        public override ConcurrentDictionary<String, ConcurrentBag<PurchaseInvoice>> getUsersPurchaseHistory()
        {
            throw new NotImplementedException();
            // only system managers can do that
        }

        public override RegularResult itemReview(string review, int itemID, int storeID)
        {
            throw new NotImplementedException();
            // only logged buyers can do that
        }

        public override RegularResult login(string userName, string password)
        {
            ResultWithValue<User> findUserRes = UserRepository.Instance.findUserByUserName(userName);
            if(findUserRes.getTag())
            {
                RegularResult loginStateRes = UserRepository.Instance.changeUserLoginStatus(findUserRes.getValue(), true, password);
                if(loginStateRes.getTag())
                {
                    user.changeState(new LoggedBuyerState(user));
                    return new Ok("The User Has Successfully Logged In");
                }
                return loginStateRes;
            }
            return new Failure("username and password don't match");
        }

        public override RegularResult loginAsSystemManager(string userName, string password)
        {
            ResultWithValue<User> findUserRes = UserRepository.Instance.findUserByUserName(userName);
            if(findUserRes.getTag())
            {
                RegularResult loginStateRes = UserRepository.Instance.changeUserLoginStatus(findUserRes.getValue(), true, password);
                if(loginStateRes.getTag())
                {
                    user.changeState(new SystemManagerState(user));
                    return new Ok("The User Has Successfully Logged In");
                }
                return loginStateRes;
            }
            return new Failure(findUserRes.getMessage());
        }

        public override RegularResult logout(String userName)
        {
            throw new NotImplementedException(); // can't log out because he ain't logged in
        }

        public override ResultWithValue<int> openStore(String storeName, String storeAddress, PurchasePolicy purchasePolicy, SalePolicy salesPolicy)
        {
            throw new NotImplementedException();
            // only logged buyers can do that
        }

        public override RegularResult register(String userName, int userAge, String password)
        {
            User user = new User(userName, userAge);
            return UserRepository.Instance.insertNewUser(user, password);
        }

        public override RegularResult removeItemFromStorage(int storeID, int itemID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override RegularResult removePurchasePredicate(int storeID, int predicateID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override RegularResult removeSale(int storeID, int saleID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }

        public override RegularResult removeStoreManager(string managerName, int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }
        
        public override RegularResult removeStoreOwner(string ownerName, int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
        }
    }
}
