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
        public ConcurrentDictionary<int, PolicyPredicate> purchasePredicates { get; set; }

        public PurchasePolicy(String purchasePolicyName, ConcurrentDictionary<int, PolicyPredicate> purchasePredicates)
        {
            this.purchasePolicyName = purchasePolicyName;
            this.purchasePredicates = purchasePredicates;
        }

        // add new purchase predicate for the store purchase policy
        public RegularResult addPurchasePolicyPredicate(PolicyPredicate predicate)
        {
            if(!purchasePredicates.ContainsKey(predicate.predicateID))
            {
                purchasePredicates.TryAdd(predicate.predicateID, predicate);
                return new Ok("The Purchase Policy Predicate Added Successfully");
            }
            return new Failure("The Purchase Policy Predicate Already Exist In This Store");
        }

        // remove purchase predicate from the store purchase policy
        public RegularResult removePurchasePolicyPredicate(int predicateID)
        {
            if (purchasePredicates.ContainsKey(predicateID))
            {
                purchasePredicates.TryRemove(predicateID, out _);
                return new Ok("The Purchase Policy Predicate Removed Successfully");
            }
            return new Failure("The Purchase Policy Predicate Not Exist In This Store");
        }

        // edit purchase predicate details
        // removes the previous predicate and insert a new one
        public RegularResult editPurchasePolicyPredicate(int predicateIDToEdit, PolicyPredicate predicateEdited)
        {
            RegularResult removePredicateRes = removePurchasePolicyPredicate(predicateIDToEdit);
            if(removePredicateRes.getTag())
            {
                return addPurchasePolicyPredicate(predicateEdited);
            }
            return removePredicateRes;
        }

        // checks all the rules of the store policy
        public RegularResult approveByPurchasePolicy(PurchaseDetails purchaseDetails)
        {
            // checks rules
            foreach (KeyValuePair<int, PolicyPredicate> predicate in purchasePredicates)
            {
                if (!predicate.Value.applyPrediacte(purchaseDetails))
                {
                    return new Failure("The Purchase Was Not Approved By The Store's Purchase Policy");
                }
            }
            return new Ok("The Purchase Was Approved By The Store's Purchase Policy");
        }
    }
}
