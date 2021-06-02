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
            RegularResult loginStateRes = UserRepository.Instance.changeUserLoginStatus(userName, true, password);
            if(loginStateRes.getTag())
            {
                return new Ok("The User Has Successfully Logged In");
            }
            return loginStateRes;
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
