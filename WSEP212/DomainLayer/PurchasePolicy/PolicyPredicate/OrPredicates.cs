using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class OrPredicates : SalePredicate
    {
        public PurchasePredicate firstPredicate { get; set; }
        public PurchasePredicate secondPredicate { get; set; }

        public OrPredicates(PurchasePredicate firstPredicate, PurchasePredicate secondPredicate)
        {
            this.firstPredicate = firstPredicate;
            this.secondPredicate = secondPredicate;
        }

        // return true if one or more of the predicates are met, else false
        public override bool applyPrediacte(PurchaseDetails purchaseDetails)
        {
            if(!firstPredicate.applyPrediacte(purchaseDetails))
            {
                return secondPredicate.applyPrediacte(purchaseDetails);
            }
            return true;
        }
    }
}
