using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public abstract class PolicyPredicate
    {
        // static counter for the predicateIDs
        private static int predicateCounter = 1;

        public int predicateID { get; set; }

        public PolicyPredicate()
        {
            this.predicateID = predicateCounter;
            predicateCounter++;
        }

        public abstract bool applyPrediacte(PurchaseDetails purchaseDetails);
    }
}
