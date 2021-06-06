using System;
using System.Collections.Concurrent;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class PurchasePolicyMock : PurchasePolicyInterface
    {
        //singelton
        private static readonly Lazy<PurchasePolicyMock> lazy
            = new Lazy<PurchasePolicyMock>(() => new PurchasePolicyMock());

        public static PurchasePolicyMock Instance
            => lazy.Value;
        
        public String purchasePolicyName { get; set; }
        public ConcurrentLinkedList<PurchaseType> purchaseTypes { get; set; }
        public ConcurrentDictionary<int, PurchasePredicate> purchasePredicates { get; set; }

        // there is no predicates in this policy
        public int addPurchasePredicate(LocalPredicate<PurchaseDetails> newPredicate, string predDescription)
        {
            return -1;
        }

        // there is no predicates in this policy - cannot compose them
        public ResultWithValue<int> composePurchasePredicates(int firstPredicateID, int secondPredicateID, PurchasePredicateCompositionType typeOfComposition)
        {
            return null;
        }

        // there is no predicates in this policy
        public ConcurrentDictionary<int, string> getPurchasePredicatesDescriptions()
        {
            return null;
        }

        // there is no predicates in this policy
        public RegularResult removePurchasePredicate(int predicateID)
        {
            return null;
        }

        // approve all the purchases
        public RegularResult approveByPurchasePolicy(PurchaseDetails purchaseDetails)
        {
            if (PurchasePolicyInterface.mock)
                return BadPurchasePolicyMock.Instance.approveByPurchasePolicy(purchaseDetails);
            return new Ok("Purchase Approved By The Purchase Policy");
        }

        public void supportPurchaseType(PurchaseType purchaseType) { }

        public void unsupportPurchaseType(PurchaseType purchaseType) { }

        public bool hasPurchaseTypeSupport(PurchaseType purchaseType)
        {
            return true;
        }
    }
}
