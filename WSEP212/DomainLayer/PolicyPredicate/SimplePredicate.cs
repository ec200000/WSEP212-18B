using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.PolicyPredicate
{
    public class SimplePredicate : SalePredicate
    {
        public LocalPredicate<PurchaseDetails> predicate { get; set; }

        public SimplePredicate(LocalPredicate<PurchaseDetails> predicate, String predicateDescription) : base(predicateDescription)
        {
            this.predicate = predicate;
        }

        public override bool applyPrediacte(PurchaseDetails purchaseDetails)
        {
            return this.predicate.applyPredicate(purchaseDetails)(purchaseDetails);
        }
    }
}
