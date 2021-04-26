using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class ConditioningPredicate : ComposedPredicate
    {
        public ConditioningPredicate(PolicyPredicate onlyIf, PolicyPredicate then) : base(onlyIf, then) { }

        // the function will return true on condition that:
        // 1. then predicate is not met for this purchase
        // 2. then predicate is met, but the onlyIf is met as well
        // For Example, "You can buy 5 kg onion or more only if you buy apples"
        public override bool applyPrediacte(PurchaseDetails purchaseDetails)
        {
            // second pred = then
            // the purchase does not contain the predicate
            if (!secondPredicate.applyPrediacte(purchaseDetails))
            {
                return true;
            }
            // first pred = only if
            // check that the only If is met
            return firstPredicate.applyPrediacte(purchaseDetails);
        }
    }
}
