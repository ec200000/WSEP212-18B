using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class SimplePredicate : PolicyPredicate
    {
        public Predicate<PurchaseDetails> predicate { get; set; }

        public SimplePredicate(Predicate<PurchaseDetails> predicate) : base()
        {
            this.predicate = predicate;
        }

        public override bool applyPrediacte(PurchaseDetails purchaseDetails)
        {
            return this.predicate(purchaseDetails);
        }
    }
}
