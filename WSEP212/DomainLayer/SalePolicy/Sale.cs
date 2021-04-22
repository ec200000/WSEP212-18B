using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public abstract class Sale
    {
        public int salePercentage { get; set; }
        public ApplySaleOn applySaleOn { get; set; }

        public Sale(int salePercentage, ApplySaleOn applySaleOn)
        {
            this.salePercentage = salePercentage;
            this.applySaleOn = applySaleOn;
        }

        public abstract bool applyPredicate(ConcurrentDictionary<Item, int> shoppingBagItems);

        public double applySaleOnItem(Item item, ConcurrentDictionary<Item, int> shoppingBagItems)
        {
            double itemPrice = item.price;
            // checks if the shopping bag match the predicate
            if (applyPredicate(shoppingBagItems))
            {
                // checks if the sale is relavent to this item 
                if (applySaleOn.shouldApplySale(item))
                {
                    return itemPrice - ((itemPrice * salePercentage) / 100);
                }
            }
            return itemPrice;
        }
    }
}
