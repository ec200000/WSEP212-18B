using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class DoubleSales : ComposedSales
    {
        private readonly object saleLock = new object();

        public DoubleSales(ConcurrentLinkedList<Sale> sales) : base(sales) { }

        public override int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            lock (saleLock)
            {
                Node<Sale> saleNode = sales.First;
                int salePercentage = 0;
                // sum all the sale percentage for this item
                while (saleNode.Value != null)
                {
                    salePercentage += saleNode.Value.getSalePercentageOnItem(item, purchaseDetails);
                    saleNode = saleNode.Next;
                }
                return salePercentage;
            }
        }

        public override double applySaleOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            lock (saleLock)
            {
                int salePercentage = getSalePercentageOnItem(item, purchaseDetails);
                // apply all sales on the item
                return item.price - ((item.price * salePercentage) / 100);
            }
        }
    }
}
