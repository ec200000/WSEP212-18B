using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public abstract class Sale
    {
        // static counter for the saleIDs
        private static int saleCounter = 1;
        public int saleID { get; set; }
        public String saleDescription { get; set; }

        public Sale(String saleDescription)
        {
            this.saleID = saleCounter;
            saleCounter++;
            this.saleDescription = saleDescription;
        }

        public abstract ConditionalSale addSaleCondition(SimplePredicate condition, SalePredicateCompositionType compositionType);
        public abstract int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails);
        public abstract double applySaleOnItem(Item item, PurchaseDetails purchaseDetails);
        public override String ToString()
        {
            return this.saleDescription;
        }
    }
}
