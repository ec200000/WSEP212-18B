using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class SystemManagerState : LoggedBuyerState
    {
        public SystemManagerState(User user) : base(user)
        {

        }

        public override ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>> getStoresPurchaseHistory()
        {
            return StoreRepository.Instance.getAllStoresPurchsesHistory();
        }

        public override ConcurrentDictionary<String, ConcurrentBag<PurchaseInvoice>> getUsersPurchaseHistory()
        {
            return UserRepository.Instance.getAllUsersPurchaseHistory();
        }
    }
}
