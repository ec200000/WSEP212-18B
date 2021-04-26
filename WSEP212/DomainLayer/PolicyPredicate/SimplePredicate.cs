using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class SimplePredicate : PolicyPredicate
    {
        public Predicate<PurchaseDetails> predicate { get; set; }

        public SimplePredicate(Predicate<PurchaseDetails> predicate) : base()
        {
            this.predicate = predicate;
        }

        public override PolicyPredicate addNewPredicate(PolicyPredicate newPredicate)
        {
            return new AndPredicates(newPredicate, this);
        }

        public override ResultWithValue<PolicyPredicate> removePredicate(int predicateID)
        {
            if(this.predicateID == predicateID)
            {
                // null means that after the delete, nothing is left
                return new OkWithValue<PolicyPredicate>("The Policy Predicate Successfully Removed", null);
            }
            return new FailureWithValue<PolicyPredicate>("This Is Not The Policy Predicate That You Want To Remove", this);
        }

        public override ResultWithValue<PolicyPredicate> editPredicate(int predicateID, PolicyPredicate editedPredicate)
        {
            if (this.predicateID == predicateID)
            {
                return new OkWithValue<PolicyPredicate>("The Policy Predicate Successfully Edited", editedPredicate);
            }
            return new FailureWithValue<PolicyPredicate>("This Is Not The Policy Predicate That You Want To Edit", this);
        }

        public override bool applyPrediacte(PurchaseDetails purchaseDetails)
        {
            return this.predicate(purchaseDetails);
        }
    }
}
