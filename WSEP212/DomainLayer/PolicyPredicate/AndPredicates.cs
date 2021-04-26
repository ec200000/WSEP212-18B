using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class AndPredicates : ComposedPredicate
    {
        public AndPredicates(PolicyPredicate firstPredicate, PolicyPredicate secondPredicate) : base(firstPredicate, secondPredicate) { }

        // return true if all predicates are met, else false
        public override bool applyPrediacte(PurchaseDetails purchaseDetails)
        {
            if(firstPredicate.applyPrediacte(purchaseDetails))
            {
                return secondPredicate.applyPrediacte(purchaseDetails);
            }
            return false;
        }
    }
}
