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

        public override UserType getUserType()
        {
            return UserType.GuestBuyer;
        }

        public override ResultWithValue<int> addItemToStorage(int storeID, int quantity, String itemName, String description, double price, String category)
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

        public override ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(int storeID)
        {
            throw new NotImplementedException();
            // only store managers and store owners can do that (logged buyers)
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

        public override ResultWithValue<int> openStore(String storeName, String storeAddress, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            throw new NotImplementedException();
            // only logged buyers can do that
        }

        public override RegularResult register(String userName, String password)
        {
            User user = new User(userName);
            return UserRepository.Instance.insertNewUser(user, password);
        }

        public override RegularResult removeItemFromStorage(int storeID, int itemID)
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
