using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TEST.UnitTests.UnitTestMocks
{
    public class PurchasePolicyMock : PurchasePolicyInterface
    {
        public PurchasePolicyMock() { }

        // there is no predicates in this policy
        public int addPurchasePredicate(Predicate<PurchaseDetails> newPredicate, string predDescription)
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
            return new Ok("Purchase Approved By The Purchase Policy");
        }

        public void supportPurchaseType(PurchaseType purchaseType) { }

        public void unsupportPurchaseType(PurchaseType purchaseType) { }

        public bool hasPurchaseTypeSupport(PurchaseType purchaseType)
        {
            return false;
        }
    }
}
