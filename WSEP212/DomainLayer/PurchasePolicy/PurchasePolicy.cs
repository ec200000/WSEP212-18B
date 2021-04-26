using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class PurchasePolicy
    {
        public String purchasePolicyName { get; set; }
        //public ConcurrentLinkedList<PurchaseType> purchaseRoutes { get; set; }
        public PolicyPredicate purchasePredicates { get; set; }

        public PurchasePolicy(String purchasePolicyName)
        {
            this.purchasePolicyName = purchasePolicyName;
            this.purchasePredicates = null;
        }

        // add new purchase predicate for the store purchase policy
        // add the predicate to the other predicates by composing then with AND Predicate
        public void addPurchasePolicyPredicate(PolicyPredicate predicate)
        {
            if(purchasePredicates == null)
                purchasePredicates = predicate;
            else
                purchasePredicates = purchasePredicates.addNewPredicate(predicate);
        }

        // remove purchase predicate from the store purchase policy
        public RegularResult removePurchasePolicyPredicate(int predicateID)
        {
            ResultWithValue<PolicyPredicate> removeRes = purchasePredicates.removePredicate(predicateID);
            if(removeRes.getTag())
            {
                purchasePredicates = removeRes.getValue();
                return new Ok(removeRes.getMessage());
            }
            return new Failure(removeRes.getMessage());
        }

        // edit purchase predicate details
        // removes the previous predicate and insert a new one
        public RegularResult editPurchasePolicyPredicate(int predicateIDToEdit, PolicyPredicate predicateEdited)
        {
            ResultWithValue<PolicyPredicate> editRes = purchasePredicates.editPredicate(predicateIDToEdit, predicateEdited);
            if (editRes.getTag())
            {
                purchasePredicates = editRes.getValue();
                return new Ok(editRes.getMessage());
            }
            return new Failure(editRes.getMessage());
        }

        // checks all the rules of the store policy
        public RegularResult approveByPurchasePolicy(PurchaseDetails purchaseDetails)
        {
            // checks rules
            if (purchasePredicates == null || purchasePredicates.applyPrediacte(purchaseDetails))
            {
                return new Ok("The Purchase Was Approved By The Store's Purchase Policy");
            }
            return new Failure("The Purchase Was Not Approved By The Store's Purchase Policy");
        }
    }
}
