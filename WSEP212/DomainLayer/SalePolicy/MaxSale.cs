using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class MaxSale : Sale
    {
        public Sale firstSale { get; set; }
        public Sale secondSale { get; set; }

        public MaxSale(Sale firstSale, Sale secondSale) :
            base("(Only One Of The Two Sales Can Be Applied: " + firstSale.ToString() + ", OR: " + secondSale.ToString() + ", " +
                "Select The Sale To Apply By The Biggest Sale")
        {
            this.firstSale = firstSale;
            this.secondSale = secondSale;
        }

        public override ConditionalSale addSaleCondition(SimplePredicate condition, SalePredicateCompositionType compositionType)
        {
            return new ConditionalSale(this, condition);
        }

        public override int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            double firstSaleTotalPrice = purchaseDetails.totalPurchasePriceAfterSale(firstSale);
            double secondSaleTotalPrice = purchaseDetails.totalPurchasePriceAfterSale(secondSale);
            if(firstSaleTotalPrice < secondSaleTotalPrice)
            {
                return firstSale.getSalePercentageOnItem(item, purchaseDetails);
            }
            return secondSale.getSalePercentageOnItem(item, purchaseDetails);
        }

        public override double applySaleOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            double firstSaleTotalPrice = purchaseDetails.totalPurchasePriceAfterSale(firstSale);
            double secondSaleTotalPrice = purchaseDetails.totalPurchasePriceAfterSale(secondSale);
            if (firstSaleTotalPrice < secondSaleTotalPrice)
            {
                return firstSale.applySaleOnItem(item, purchaseDetails);
            }
            return secondSale.applySaleOnItem(item, purchaseDetails);
        }
    }
}
