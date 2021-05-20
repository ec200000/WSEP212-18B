using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.SalePolicy.SaleOn;

namespace WSEP212.DomainLayer.SalePolicy
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

        public override double applySaleOnItem(Item item, double purchaseItemPrice, PurchaseDetails purchaseDetails)
        {
            int salePercentage = getSalePercentageOnItem(item, purchaseDetails);
            return purchaseItemPrice - ((purchaseItemPrice * salePercentage) / 100);
        }
    }
}
