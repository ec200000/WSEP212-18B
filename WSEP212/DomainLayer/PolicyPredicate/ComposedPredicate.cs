using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public abstract class ComposedPredicate : PolicyPredicate
    {
        public PolicyPredicate firstPredicate { get; set; }
        public PolicyPredicate secondPredicate { get; set; }

        public ComposedPredicate(PolicyPredicate firstPredicate, PolicyPredicate secondPredicate)
        {
            this.firstPredicate = firstPredicate;
            this.secondPredicate = secondPredicate;
        }

        public override PolicyPredicate addNewPredicate(PolicyPredicate newPredicate)
        {
            return new AndPredicates(newPredicate, this);
        }

        public override ResultWithValue<PolicyPredicate> removePredicate(int predicateID)
        {
            if (this.predicateID == predicateID)
            {
                // null means that after the delete, nothing is left
                return new OkWithValue<PolicyPredicate>("The Policy Predicate Successfully Removed", null);
            }
            ResultWithValue<PolicyPredicate> firstPredicateRes = firstPredicate.removePredicate(predicateID);
            // the pred need to be deleted from the first pred
            if (firstPredicateRes.getTag())
            {
                // deleted all the first pred - there is left only the second pred
                if (firstPredicateRes.getValue() == null)
                {
                    return new OkWithValue<PolicyPredicate>(firstPredicateRes.getMessage(), secondPredicate);
                }
                else
                {
                    this.firstPredicate = firstPredicateRes.getValue();
                    return new OkWithValue<PolicyPredicate>(firstPredicateRes.getMessage(), this);
                }
            }
            ResultWithValue<PolicyPredicate> secondPredicateRes = secondPredicate.removePredicate(predicateID);
            // the pred need to be deleted from the second pred
            if (secondPredicateRes.getTag())
            {
                // deleted all the second pred - there is left only the first pred
                if (secondPredicateRes.getValue() == null)
                {
                    return new OkWithValue<PolicyPredicate>(secondPredicateRes.getMessage(), firstPredicate);
                }
                else
                {
                    this.secondPredicate = secondPredicateRes.getValue();
                    return new OkWithValue<PolicyPredicate>(secondPredicateRes.getMessage(), this);
                }
            }
            return new FailureWithValue<PolicyPredicate>("This Is Not The Policy Predicate That You Want To Remove", this);
        }

        public override ResultWithValue<PolicyPredicate> editPredicate(int predicateID, PolicyPredicate editedPredicate)
        {
            if (this.predicateID == predicateID)
            {
                return new OkWithValue<PolicyPredicate>("The Policy Predicate Successfully Edited", editedPredicate);
            }
            ResultWithValue<PolicyPredicate> firstPredicateRes = firstPredicate.editPredicate(predicateID, editedPredicate);
            // the pred need to be edited from the first pred
            if (firstPredicateRes.getTag())
            {
                firstPredicate = firstPredicateRes.getValue();
                return new OkWithValue<PolicyPredicate>(firstPredicateRes.getMessage(), this);
            }
            ResultWithValue<PolicyPredicate> secondPredicateRes = secondPredicate.editPredicate(predicateID, editedPredicate);
            // the pred need to be edited from the second pred
            if (secondPredicateRes.getTag())
            {
                secondPredicate = secondPredicateRes.getValue();
                return new OkWithValue<PolicyPredicate>(secondPredicateRes.getMessage(), this);
            }
            return new FailureWithValue<PolicyPredicate>("This Is Not The Policy Predicate That You Want To Edit", this);
        }
    }
}
