using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class DoubleSale : Sale
    {
        public Sale firstSale { get; set; }
        public Sale secondSale { get; set; }

        public DoubleSale(Sale firstSale, Sale secondSale)
        {
            this.firstSale = firstSale;
            this.secondSale = secondSale;
        }

        public override int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            return firstSale.getSalePercentageOnItem(item, purchaseDetails) + secondSale.getSalePercentageOnItem(item, purchaseDetails);
        }

        public override double applySaleOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            int salePercentage = getSalePercentageOnItem(item, purchaseDetails);
            // apply the two sales on the item
            return item.price - ((item.price * salePercentage) / 100);
        }
    }
}
