using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class SimplePredicate : PurchasePredicate, SalePredicate
    {
        public Predicate<PurchaseDetails> predicate { get; set; }

        public SimplePredicate(Predicate<PurchaseDetails> predicate)
        {
            this.predicate = predicate;
        }

        public bool applyPrediacte(PurchaseDetails purchaseDetails)
        {
            return this.predicate(purchaseDetails);
        }
    }
}
