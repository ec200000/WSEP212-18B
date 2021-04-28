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

        public PurchasePredicate()
        {
            this.predicateID = predicateCounter;
            predicateCounter++;
        }

        public abstract bool applyPrediacte(PurchaseDetails purchaseDetails);

    }
}
