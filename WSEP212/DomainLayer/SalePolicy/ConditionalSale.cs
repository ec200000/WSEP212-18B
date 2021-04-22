using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class ConditionalSale : Sale
    {
        public Predicate<ConcurrentDictionary<Item, int>> predicate { get; set; }

        public ConditionalSale(int salePercentage, ApplySaleOn applySaleOn, Predicate<ConcurrentDictionary<Item, int>> predicate) : base(salePercentage, applySaleOn)
        {
            this.predicate = predicate;
        }

        public override bool applyPredicate(ConcurrentDictionary<Item, int> shoppingBagItems)
        {
            return this.predicate(shoppingBagItems);
        }
    }
}
