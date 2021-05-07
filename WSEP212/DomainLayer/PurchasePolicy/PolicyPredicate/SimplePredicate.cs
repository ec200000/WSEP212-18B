using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class SimplePredicate : SalePredicate
    {
        public Predicate<PurchaseDetails> predicate { get; set; }

        public SimplePredicate(Predicate<PurchaseDetails> predicate, String predicateDescription) : base(predicateDescription)
        {
            this.predicate = predicate;
        }

        public override bool applyPrediacte(PurchaseDetails purchaseDetails)
        {
            return this.predicate(purchaseDetails);
        }
    }
}
