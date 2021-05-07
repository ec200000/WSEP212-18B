using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class ConditioningPredicate : PurchasePredicate
    {
        public PurchasePredicate onlyIf { get; set; }
        public PurchasePredicate then { get; set; }

        public ConditioningPredicate(PurchasePredicate onlyIf, PurchasePredicate then) : 
            base("(Only If: " + onlyIf.ToString() + " Then You Can:" + then.ToString() + ")")
        {
            this.onlyIf = onlyIf;
            this.then = then;
        }

        // the function will return true on condition that:
        // 1. then predicate is not met for this purchase
        // 2. then predicate is met, but the onlyIf is met as well
        // For Example, "You can buy 5 kg onion or more only if you buy apples"
        public override bool applyPrediacte(PurchaseDetails purchaseDetails)
        {
            // the purchase does not contain the predicate
            if (!then.applyPrediacte(purchaseDetails))
            {
                return true;
            }
            // check that the only If is met
            return onlyIf.applyPrediacte(purchaseDetails);
        }
    }
}
