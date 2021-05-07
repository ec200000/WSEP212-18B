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

        public SimpleSale(int salePercentage, ApplySaleOn applySaleOn, String saleDescription) : base(saleDescription)
        {
            this.salePercentage = salePercentage;
            this.applySaleOn = applySaleOn;
        }

        public override ConditionalSale addSaleCondition(SimplePredicate condition, SalePredicateCompositionType compositionType)
        {
            return new ConditionalSale(this, condition);
        }

        public override int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            // checks if the sale is relavent to this item 
            if (applySaleOn.shouldApplySale(item))
            {
                return salePercentage;
            }
            return 0;
        }

        public override double applySaleOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            int salePercentage = getSalePercentageOnItem(item, purchaseDetails);
            return item.price - ((item.price * salePercentage) / 100);
        }
    }
}
