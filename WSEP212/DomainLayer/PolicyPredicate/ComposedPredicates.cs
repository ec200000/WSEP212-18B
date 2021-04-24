using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public abstract class ComposedPredicates : SalePredicate, PurchasePredicate
    {
        public ConcurrentLinkedList<SalePredicate> predicates { get; set; }

        public ComposedPredicates(ConcurrentLinkedList<SalePredicate> predicates)
        {
            this.predicates = predicates;
        }

        public abstract bool applyPrediacte(PurchaseDetails purchaseDetails);

        public RegularResult addPredicate(SalePredicate predicate)
        {
            if (!predicates.Contains(predicate))
            {
                predicates.TryAdd(predicate);
                return new Ok("The Prediacte Added Successfully");
            }
            return new Failure("The Prediacte Is Already Included Here");
        }

        public RegularResult removePredicate(SalePredicate predicate)
        {
            if (predicates.Contains(predicate))
            {
                predicates.Remove(predicate, out _);
                return new Ok("The Prediacte Removed Successfully");
            }
            return new Failure("The Prediacte Is Not Included Here");
        }
    }
}
