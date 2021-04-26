using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class OrPredicates : ComposedPredicate
    {
        public OrPredicates(PolicyPredicate firstPredicate, PolicyPredicate secondPredicate) : base(firstPredicate, secondPredicate) { }

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
