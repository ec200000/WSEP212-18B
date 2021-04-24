using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public interface Sale
    {
        public int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails);
        public double applySaleOnItem(Item item, PurchaseDetails purchaseDetails);
    }
}
