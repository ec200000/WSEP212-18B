using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class XorSale : Sale
    {
        public Sale firstSale { get; set; }
        public Sale secondSale { get; set; }
        public Predicate<PurchaseDetails> selectionRule { get; set; }

        public XorSale(Sale firstSale, Sale secondSale, Predicate<PurchaseDetails> selectionRule) : base()
        {
            this.firstSale = firstSale;
            this.secondSale = secondSale;
            this.selectionRule = selectionRule;
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
            if (this.selectionRule(purchaseDetails))
            {
                return firstSalePercentage;
            }
            return secondSalePercentage;
        }

        // applied zero or one of the sales
        public override double applySaleOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            // checks if the first sale cannot applied
            // if so, the second sale is the determines
            double itemPriceAfterSale1 = this.firstSale.applySaleOnItem(item, purchaseDetails);
            if (itemPriceAfterSale1 == item.price)
            {
                return this.secondSale.applySaleOnItem(item, purchaseDetails);
            }
            // the first sale can be applied, need to check if the second sale can too
            // if not, return the first sale price
            double itemPriceAfterSale2 = this.secondSale.applySaleOnItem(item, purchaseDetails);
            if (itemPriceAfterSale2 == item.price)
            {
                return itemPriceAfterSale1;
            }
            // both sales can be applied, choose by the selection rule
            if(this.selectionRule(purchaseDetails))
            {
                return itemPriceAfterSale1;
            }
            return itemPriceAfterSale2;
        }
    }
}
