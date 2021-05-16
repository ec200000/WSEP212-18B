using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer.PolicyPredicate;

namespace WSEP212.DomainLayer.SalePolicy
{
    public class DoubleSale : Sale
    {
        public Sale firstSale { get; set; }
        public Sale secondSale { get; set; }

        public DoubleSale(Sale firstSale, Sale secondSale) :
            base("(Both Sales Can Be Applied: " + firstSale.ToString() + ", AND: " + secondSale.ToString() + ")")
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
            return firstSale.getSalePercentageOnItem(item, purchaseDetails) + secondSale.getSalePercentageOnItem(item, purchaseDetails);
        }

        public override double applySaleOnItem(Item item, double purchaseItemPrice, PurchaseDetails purchaseDetails)
        {
            int salePercentage = getSalePercentageOnItem(item, purchaseDetails);
            // apply the two sales on the item
            return purchaseItemPrice - ((purchaseItemPrice * salePercentage) / 100);
        }
    }
}
