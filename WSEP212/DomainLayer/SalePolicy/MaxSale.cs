using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class MaxSale : ComposedSales
    {
        private readonly object saleLock = new object();

        public MaxSale(ConcurrentLinkedList<Sale> sales) : base(sales) { }

        public override int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            lock (saleLock)
            {
                Node<Sale> saleNode = sales.First;
                double cheapestPrice = Double.MaxValue, priceAfterSale;
                Sale bestSale = null;
                while (saleNode.Value != null)
                {
                    // checks if there is better sale
                    priceAfterSale = purchaseDetails.totalPurchasePriceAfterSale(saleNode.Value);
                    if (priceAfterSale < cheapestPrice)
                    {
                        bestSale = saleNode.Value;
                        cheapestPrice = priceAfterSale;
                    }
                    saleNode = saleNode.Next;
                }
                if (bestSale == null)
                {
                    return 0;
                }
                // apply the best sale on the item
                return bestSale.getSalePercentageOnItem(item, purchaseDetails);
            }
        }

        public override double applySaleOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            lock (saleLock)
            {
                Node<Sale> saleNode = sales.First;
                double cheapestPrice = Double.MaxValue, priceAfterSale;
                Sale bestSale = null;
                while (saleNode.Value != null)
                {
                    // checks if there is better sale
                    priceAfterSale = purchaseDetails.totalPurchasePriceAfterSale(saleNode.Value);
                    if (priceAfterSale < cheapestPrice)
                    {
                        bestSale = saleNode.Value;
                        cheapestPrice = priceAfterSale;
                    }
                    saleNode = saleNode.Next;
                }
                
                if(bestSale == null)
                {
                    return item.price;
                }
                // apply the best sale on the item
                return bestSale.applySaleOnItem(item, purchaseDetails);
            }
        }
    }
}
