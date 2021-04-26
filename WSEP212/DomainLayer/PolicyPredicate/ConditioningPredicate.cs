using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class ConditioningPredicate : PolicyPredicate
    {
        public PolicyPredicate onlyIf { get; set; }
        public Predicate<PurchaseDetails> then { get; set; }

        public ConditioningPredicate(PolicyPredicate onlyIf, Predicate<PurchaseDetails> then) : base()
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
            if (!this.then(purchaseDetails))
            {
                return true;
            }
            // check that the only If is met
            return onlyIf.applyPrediacte(purchaseDetails);
        }
    }
}
