using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public interface PolicyPredicate
    {
        public bool applyPrediacte(PurchaseDetails purchaseDetails);
    }
}
