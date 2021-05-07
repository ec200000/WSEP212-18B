using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class AndPredicates : SalePredicate
    {
        public PurchasePredicate firstPredicate { get; set; }
        public PurchasePredicate secondPredicate { get; set; }

        public AndPredicates(PurchasePredicate firstPredicate, PurchasePredicate secondPredicate) :
            base("(Both Predicates Must Be Met: " + firstPredicate.ToString() + " AND:" + secondPredicate.ToString() + ")")
        {
            this.firstPredicate = firstPredicate;
            this.secondPredicate = secondPredicate;
        }

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
