using System;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class PurchasePolicy
    {
        public String purchasePolicyName { get; set; }
        //public ConcurrentLinkedList<PurchaseType> purchaseRoutes { get; set; }
        public ConcurrentLinkedList<PurchasePredicate> purchasePredicates { get; set; }

        public PurchasePolicy(String purchasePolicyName, ConcurrentLinkedList<PurchasePredicate> purchasePredicates)
        {
            this.purchasePolicyName = purchasePolicyName;
            this.purchasePredicates = purchasePredicates;
        }

        // checks all the rules of the store policy
        public RegularResult approveByPurchasePolicy(PurchaseDetails purchaseDetails)
        {
            // checks rules
            Node<PurchasePredicate> predicateNode = purchasePredicates.First;
            while (predicateNode.Value != null)
            {
                if(!predicateNode.Value.applyPrediacte(purchaseDetails))
                {
                    return new Failure("The Purchase Was Not Approved By The Store's Purchase Policy");
                }
                predicateNode = predicateNode.Next;
            }
            return new Ok("The Purchase Was Approved By The Store's Purchase Policy");
        }
    }
}
