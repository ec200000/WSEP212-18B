using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class SimpleSale : Sale
    {
        public SimpleSale(int salePercentage, ApplySaleOn applySaleOn) : base(salePercentage, applySaleOn) { }

        // Simple sale has no predicate to validate
        public override bool applyPredicate(ConcurrentDictionary<Item, int> shoppingBagItems)
        {
            return true;
        }
    }
}
