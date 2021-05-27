using System;
using System.Collections.Concurrent;
<<<<<<< HEAD:WSEP212-TESTS/WSEP212-Test-Mocks/BadPurchasePolicyMock.cs
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.DomainLayer;
=======
>>>>>>> origin/add_db:WSEP212/DomainLayer/WSEP212-Mocks/BadPurchasePolicyMock.cs
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class BadPurchasePolicyMock : PurchasePolicyInterface
    {
        public String purchasePolicyName { get; set; }
        public ConcurrentLinkedList<PurchaseType> purchaseTypes { get; set; }
        public ConcurrentDictionary<int, PurchasePredicate> purchasePredicates { get; set; }

        public BadPurchasePolicyMock() { }

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

        // not approve all the purchases
        public RegularResult approveByPurchasePolicy(PurchaseDetails purchaseDetails)
        {
            return new Failure("The Purchase Was Not Approved By The Purchase Policy");
        }

        public string purchasePolicyName { get; set; }
        public ConcurrentLinkedList<PurchaseType> purchaseTypes { get; set; }
        public ConcurrentDictionary<int, PurchasePredicate> purchasePredicates { get; set; }
        public void supportPurchaseType(PurchaseType purchaseType) { }

        public void unsupportPurchaseType(PurchaseType purchaseType) { }

        public bool hasPurchaseTypeSupport(PurchaseType purchaseType)
        {
            return true;
        }
    }
}
