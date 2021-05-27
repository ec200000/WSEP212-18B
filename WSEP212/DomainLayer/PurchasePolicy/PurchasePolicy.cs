using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.PurchasePolicy
{
    public class PurchasePolicy : PurchasePolicyInterface
    {
        public String purchasePolicyName { get; set; }
        public ConcurrentLinkedList<PurchaseType> purchaseTypes { get; set; }
        public ConcurrentDictionary<int, PurchasePredicate> purchasePredicates { get; set; }

        public PurchasePolicy(String purchasePolicyName)
        {
            this.purchasePolicyName = purchasePolicyName;
            this.purchaseTypes = new ConcurrentLinkedList<PurchaseType>();
            this.purchaseTypes.TryAdd(PurchaseType.ImmediatePurchase);
            this.purchasePredicates = new ConcurrentDictionary<int, PurchasePredicate>();
        }
        
        private PurchaseType[] listToArray(ConcurrentLinkedList<PurchaseType> lst)
        {
            PurchaseType[] arr = new PurchaseType[lst.size];
            int i = 0;
            Node<PurchaseType> node = lst.First; 
            int size = lst.size;
            while(size > 0)
            {
                arr[i] = node.Value;
                node = node.Next;
                i++;
                size--;
            }
            return arr;
        }

        // add support for new purchase type
        public void supportPurchaseType(PurchaseType purchaseType)
        {
            var arr = listToArray(purchaseTypes);
            if(!arr.Contains(purchaseType))
            {
                purchaseTypes.TryAdd(purchaseType);
            }
        }

        // remove the purchase type from the store - not supporting it
        public void unsupportPurchaseType(PurchaseType purchaseType)
        {
            var arr = listToArray(purchaseTypes);
            if(arr.Contains(purchaseType))
            {
                purchaseTypes.Remove(purchaseType, out _);
            }
        }

        // return true only if the store support the purchase type
        public Boolean hasPurchaseTypeSupport(PurchaseType purchaseType)
        {
            var arr = listToArray(purchaseTypes);
            return arr.Contains(purchaseType);
        }

        // add new purchase predicate for the store purchase policy
        // add the predicate to the other predicates by composing them with AND Predicate - done by the build 
        // returns the id of the new purchase predicate
        public int addPurchasePredicate(LocalPredicate<PurchaseDetails> predicate, String predicateDescription) 
        {
            SimplePredicate simplePredicate = new SimplePredicate(predicate, predicateDescription);
            purchasePredicates.TryAdd(simplePredicate.predicateID, simplePredicate);
            return simplePredicate.predicateID;
        }

        // remove purchase predicate from the store purchase policy
        public RegularResult removePurchasePredicate(int predicateID)
        {
            if(purchasePredicates.ContainsKey(predicateID))
            {
                purchasePredicates.TryRemove(predicateID, out _);
                return new Ok("The Purchase Predicate Was Removed To The Store's Purchase Policy");
            }
            return new Failure("The Predicate Is Not Exist In This Store Purchase Policy");
        }

        // compose two predicates by the type of predicate 
        // returns the id of the new composed predicate
        public ResultWithValue<int> composePurchasePredicates(int firstPredicateID, int secondPredicateID, PurchasePredicateCompositionType typeOfComposition)
        {
            if(!purchasePredicates.ContainsKey(firstPredicateID) || !purchasePredicates.ContainsKey(secondPredicateID))
            {
                return new FailureWithValue<int>("One Or More Of The Predicates Are Not Exist In This Store Purchase Policy", -1);
            }
            purchasePredicates.TryRemove(firstPredicateID, out PurchasePredicate firstPredicate);
            purchasePredicates.TryRemove(secondPredicateID, out PurchasePredicate secondPredicate);
            // composing the two predicates togther
            PurchasePredicate composedPredicate = null;
            switch(typeOfComposition)
            {
                case PurchasePredicateCompositionType.AndComposition:
                    composedPredicate = new AndPredicates(firstPredicate, secondPredicate);
                    break;
                case PurchasePredicateCompositionType.OrComposition:
                    composedPredicate = new OrPredicates(firstPredicate, secondPredicate);
                    break;
                case PurchasePredicateCompositionType.ConditionalComposition:
                    composedPredicate = new ConditioningPredicate(firstPredicate, secondPredicate);
                    break;
            }
            purchasePredicates.TryAdd(composedPredicate.predicateID, composedPredicate);
            return new OkWithValue<int>("The Composed Purchase Predicate Was Added To The Store's Purchase Policy", composedPredicate.predicateID);
        }

        // returns the descriptions of all predicates for presenting them to the user
        public ConcurrentDictionary<int, String> getPurchasePredicatesDescriptions()
        {
            ConcurrentDictionary<int, String> predicatesDescriptions = new ConcurrentDictionary<int, string>();
            foreach (KeyValuePair<int, PurchasePredicate> predPair in purchasePredicates)
            {
                predicatesDescriptions.TryAdd(predPair.Key, predPair.Value.ToString());
            }
            return predicatesDescriptions;
        }

        // builds the purchase policy by all the predicates in the store
        // build it by composing all predicates with AND composition
        private PurchasePredicate buildPurchasePolicy()
        {
            PurchasePredicate purchasePolicyPredicates = null;
            foreach (PurchasePredicate predicate in purchasePredicates.Values)
            {
                if(purchasePolicyPredicates == null)
                {
                    purchasePolicyPredicates = predicate;
                }
                else
                {
                    purchasePolicyPredicates = new AndPredicates(predicate, purchasePolicyPredicates);
                }
            }
            return purchasePolicyPredicates;
        }

        // checks all the rules of the store policy
        public RegularResult approveByPurchasePolicy(PurchaseDetails purchaseDetails)
        {
            // checks rules
            PurchasePredicate purchasePolicyPredicates = buildPurchasePolicy();
            if (purchasePolicyPredicates == null || purchasePolicyPredicates.applyPrediacte(purchaseDetails))
            {
                return new Ok("The Purchase Was Approved By The Store's Purchase Policy");
            }
            return new Failure("The Purchase Was Not Approved By The Store's Purchase Policy");
        }
    }
}
