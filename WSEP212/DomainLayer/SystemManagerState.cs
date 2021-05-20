using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class SystemManagerState : LoggedBuyerState
    {
        public SystemManagerState(User user) : base(user)
        {

        }
        
        public override RegularResult loginAsSystemManager(string userName, string password)
        {
            ResultWithValue<User> findUserRes = UserRepository.Instance.findUserByUserName(userName);
            if(findUserRes.getTag())
            {
                RegularResult loginStateRes = UserRepository.Instance.changeUserLoginStatus(findUserRes.getValue(), true, password);
                if(loginStateRes.getTag())
                {
                    return new Ok("The User Has Successfully Logged In");
                }
                return loginStateRes;
            }
            return new Failure(findUserRes.getMessage());
        }

        public override ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>> getStoresPurchaseHistory()
        {
            return StoreRepository.Instance.getAllStoresPurchsesHistory();
        }

        public override ConcurrentDictionary<String, ConcurrentDictionary<int, PurchaseInvoice>> getUsersPurchaseHistory()
        {
            return UserRepository.Instance.getAllUsersPurchaseHistory();
        }
    }
}
