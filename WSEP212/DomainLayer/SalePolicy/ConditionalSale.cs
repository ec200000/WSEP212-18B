using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class ConditionalSale : Sale
    {
        public Sale sale { get; set; }
        public SalePredicate predicate { get; set; }

        public ConditionalSale(Sale sale, SalePredicate predicate) : base()
        {
            this.sale = sale;
            this.predicate = predicate;
        }

        public override int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            // checks if the predicate is met
            if (this.predicate.applyPrediacte(purchaseDetails))
            {
                return sale.getSalePercentageOnItem(item, purchaseDetails);
            }
            return 0;
        }

        // apply the sale only if the conditional is met and the sale is relevant to this item
        public override double applySaleOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            int salePercentage = getSalePercentageOnItem(item, purchaseDetails);
            return item.price - ((item.price * salePercentage) / 100);
        }
    }
}
