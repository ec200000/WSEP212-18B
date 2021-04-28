using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class MaxSale : Sale
    {
        // implement Max Sale With Xor Sale
        public XorSale xorSale { get; set; }

        public MaxSale(Sale firstSale, Sale secondSale)
        {
            Predicate<PurchaseDetails> maxPredicate = 
                pd => pd.totalPurchasePriceAfterSale(firstSale) < pd.totalPurchasePriceAfterSale(secondSale);
            this.xorSale = new XorSale(firstSale, secondSale, maxPredicate);
        }

        public override int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            return this.xorSale.getSalePercentageOnItem(item, purchaseDetails);
        }

        public override double applySaleOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            return this.applySaleOnItem(item, purchaseDetails);
        }
    }
}
