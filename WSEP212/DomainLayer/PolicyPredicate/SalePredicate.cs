using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public interface SalePredicate
    {
        public bool applyPrediacte(PurchaseDetails purchaseDetails);
    }
}
