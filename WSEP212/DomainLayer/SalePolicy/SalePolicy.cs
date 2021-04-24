using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class SalePolicy
    {
        public String salesPolicyName { get; set; }
        public Sale storeSales { get; set; }

        public SalePolicy(String salesPolicyName, Sale storeSales = null)
        {
            this.salesPolicyName = salesPolicyName;
            this.storeSales = storeSales;
        }

        // returns the price of each item after sales
        public ConcurrentDictionary<int, double> pricesAfterSalePolicy(ConcurrentDictionary<Item, int> items, PurchaseDetails purchaseDetails)
        {
            ConcurrentDictionary<int, double> pricesAfterSale = new ConcurrentDictionary<int, double>();
            double itemPriceAfterSale;
            foreach (Item item in items.Keys)
            {
                if(storeSales == null)
                {
                    itemPriceAfterSale = item.price;
                }
                else
                {
                    itemPriceAfterSale = storeSales.applySaleOnItem(item, purchaseDetails);
                }
                pricesAfterSale.TryAdd(item.itemID, itemPriceAfterSale);
            }
            return pricesAfterSale;
        }
    }
}
