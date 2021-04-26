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

        public Sale()
        {
            this.saleID = saleCounter;
            saleCounter++;
        }

        public abstract int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails);
        public abstract double applySaleOnItem(Item item, PurchaseDetails purchaseDetails);
    }
}
