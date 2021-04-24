using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class AndPredicates : ComposedPredicates
    {
        private readonly object applyPredicatesLock = new object();

        public AndPredicates(ConcurrentLinkedList<PolicyPredicate> predicates) : base(predicates) { }

        // return true if all predicates are met, else false
        public override bool applyPrediacte(PurchaseDetails purchaseDetails)
        {
            lock (applyPredicatesLock)
            {
                Node<PolicyPredicate> predicateNode = predicates.First;
                while (predicateNode.Value != null)
                {
                    if (!predicateNode.Value.applyPrediacte(purchaseDetails))
                    {
                        return false;
                    }
                    predicateNode = predicateNode.Next;
                }
                return true;
            }
        }
    }
}
