using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class OrPredicates : ComposedPredicates
    {
        private readonly object applyPredicatesLock = new object();

        public OrPredicates(ConcurrentLinkedList<PolicyPredicate> predicates) : base(predicates) { }

        // return true if one or more of the predicates are met, else false
        public override bool applyPrediacte(PurchaseDetails purchaseDetails)
        {
            lock (applyPredicatesLock)
            {
                Node<PolicyPredicate> predicateNode = predicates.First;
                while (predicateNode.Value != null)
                {
                    if (predicateNode.Value.applyPrediacte(purchaseDetails))
                    {
                        return true;
                    }
                    predicateNode = predicateNode.Next;
                }
                return false;
            }
        }
    }
}
