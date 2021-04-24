using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class ConditionalSale : Sale
    {
        public int salePercentage { get; set; }
        public ApplySaleOn applySaleOn { get; set; }
        public SalePredicate predicate { get; set; }

        public ConditionalSale(int salePercentage, ApplySaleOn applySaleOn, SalePredicate predicate)
        {
            this.salePercentage = salePercentage;
            this.applySaleOn = applySaleOn;
            this.predicate = predicate;
        }

        public int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            // checks if the predicate is met
            if (this.predicate.applyPrediacte(purchaseDetails))
            {
                // checks if the sale is relavent to this item 
                if (applySaleOn.shouldApplySale(item))
                {
                    return salePercentage;
                }
            }
            return 0;
        }

        // apply the sale only if the conditional is met and the sale is relevant to this item
        public double applySaleOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            double itemPrice = item.price;
            // checks if the predicate is met
            if(this.predicate.applyPrediacte(purchaseDetails))
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
