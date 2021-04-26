using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public abstract class ComposedPredicates : PolicyPredicate
    {
        public ConcurrentLinkedList<PolicyPredicate> predicates { get; set; }

        public ComposedPredicates(ConcurrentLinkedList<PolicyPredicate> predicates) : base()
        {
            this.predicates = predicates;
        }

        public RegularResult addPredicate(PolicyPredicate predicate)
        {
            if (!predicates.Contains(predicate))
            {
                predicates.TryAdd(predicate);
                return new Ok("The Prediacte Added Successfully");
            }
            return new Failure("The Prediacte Is Already Included Here");
        }

        public RegularResult removePredicate(PolicyPredicate predicate)
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
