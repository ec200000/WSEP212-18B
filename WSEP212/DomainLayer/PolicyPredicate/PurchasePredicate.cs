using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.PolicyPredicate
{
    public abstract class PurchasePredicate
    {
        // static counter for the predicateIDs
        [JsonIgnore]
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
