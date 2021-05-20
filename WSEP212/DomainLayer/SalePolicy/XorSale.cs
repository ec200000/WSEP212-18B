using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer.PolicyPredicate;

namespace WSEP212.DomainLayer.SalePolicy
{
    public class XorSale : Sale
    {
        public Sale firstSale { get; set; }
        public Sale secondSale { get; set; }
        public SimplePredicate selectionRule { get; set; }

        public XorSale(Sale firstSale, Sale secondSale, SimplePredicate selectionRule) :
            base("(Only One Of The Two Sales Can Be Applied: " + firstSale.ToString() + ", OR: " + secondSale.ToString() + ", " +
                "Select The Sale To Apply By The Selection Rule: " + selectionRule.ToString() + ")")
        {
            this.firstSale = firstSale;
            this.secondSale = secondSale;
            this.selectionRule = selectionRule;
        }

        public override ConditionalSale addSaleCondition(SimplePredicate condition, SalePredicateCompositionType compositionType)
        {
            return new ConditionalSale(this, condition);
        }

        public override int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            // checks the first sale percentage
            // if 0, the second sale is the determines
            int firstSalePercentage = this.firstSale.getSalePercentageOnItem(item, purchaseDetails);
            if (firstSalePercentage == 0)
            {
                return this.secondSale.getSalePercentageOnItem(item, purchaseDetails);
            }
            // the first sale percentage is not 0, need to check if the second sale percentage
            // if 0, return the first sale percentage
            int secondSalePercentage = this.secondSale.getSalePercentageOnItem(item, purchaseDetails);
            if (secondSalePercentage == 0)
            {
                return firstSalePercentage;
            }
            // both sales can be applied, choose by the selection rule
            if (this.selectionRule.applyPrediacte(purchaseDetails))
            {
                return firstSalePercentage;
            }
            return secondSalePercentage;
        }

        // applied zero or one of the sales
        public override double applySaleOnItem(Item item, double purchaseItemPrice, PurchaseDetails purchaseDetails)
        {
            // checks if the first sale cannot applied
            // if so, the second sale is the determines
            double itemPriceAfterSale1 = this.firstSale.applySaleOnItem(item, purchaseItemPrice, purchaseDetails);
            if (itemPriceAfterSale1 == purchaseItemPrice)
            {
                return this.secondSale.applySaleOnItem(item, purchaseItemPrice, purchaseDetails);
            }
            // the first sale can be applied, need to check if the second sale can too
            // if not, return the first sale price
            double itemPriceAfterSale2 = this.secondSale.applySaleOnItem(item, purchaseItemPrice, purchaseDetails);
            if (itemPriceAfterSale2 == purchaseItemPrice)
            {
                return itemPriceAfterSale1;
            }
            // both sales can be applied, choose by the selection rule
            if(this.selectionRule.applyPrediacte(purchaseDetails))
            {
                return itemPriceAfterSale1;
            }
            return itemPriceAfterSale2;
        }
    }
}
