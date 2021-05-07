using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public abstract class PurchasePredicate
    {
        // static counter for the predicateIDs
        private static int predicateCounter = 1;
        public int predicateID { get; set; }
        public String predicateDescription { get; set; }

        public PurchasePredicate(String predicateDescription)
        {
            this.predicateID = predicateCounter;
            predicateCounter++;
            this.predicateDescription = predicateDescription;
        }

        public abstract bool applyPrediacte(PurchaseDetails purchaseDetails);
        public override String ToString()
        {
            return this.predicateDescription;
        }
    }
}
