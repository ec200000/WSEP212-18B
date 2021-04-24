using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class SimpleSale : Sale
    {
        public int salePercentage { get; set; }
        public ApplySaleOn applySaleOn { get; set; }

        public SimpleSale(int salePercentage, ApplySaleOn applySaleOn)
        {
            this.salePercentage = salePercentage;
            this.applySaleOn = applySaleOn;
        }

        public int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            // checks if the sale is relavent to this item 
            if (applySaleOn.shouldApplySale(item))
            {
                return salePercentage;
            }
            return 0;
        }

        public double applySaleOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            double itemPrice = item.price;
            // checks if the sale is relavent to this item 
            if (applySaleOn.shouldApplySale(item))
            {
                return itemPrice - ((itemPrice * salePercentage) / 100);
            }
            return itemPrice;
        }
    }
}
