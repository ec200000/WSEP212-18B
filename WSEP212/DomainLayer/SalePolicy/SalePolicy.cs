using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class SalePolicy
    {
        public String salesPolicyName { get; set; }
        public ConcurrentDictionary<int, Sale> storeSales { get; set; }

        public SalePolicy(String salesPolicyName)
        {
            this.salesPolicyName = salesPolicyName;
            this.storeSales = new ConcurrentDictionary<int, Sale>();
        }

        // add new sale for the store sale policy
        // add the sale to the other sales by composing them with Double sale - done by the build 
        public RegularResult addSale(SimpleSale newSale)
        {
            int saleID = newSale.saleID;
            if (!storeSales.ContainsKey(saleID))
            {
                storeSales.TryAdd(saleID, newSale);
                return new Ok("The Sale Was Added To The Store's Sale Policy");
            }
            return new Failure("The Sale Is Already Exist In This Store Sale Policy");
        }

        // remove sale from the store sale policy
        public RegularResult removeSale(int saleID)
        {
            if (storeSales.ContainsKey(saleID))
            {
                storeSales.TryRemove(saleID, out _);
                return new Ok("The Sale Was Removed To The Store's Purchase Policy");
            }
            return new Failure("The Sale Is Not Exist In This Store Sale Policy");
        }

        // compose two sales by the type of sale 
        public RegularResult composeSales(int firstSaleID, int secondSaleID, SaleCompositionType typeOfComposition, Predicate<PurchaseDetails> selectionRule)
        {
            if (!storeSales.ContainsKey(firstSaleID) || !storeSales.ContainsKey(secondSaleID))
            {
                return new Failure("One Or More Of The Sales Are Not Exist In This Store Sale Policy");
            }
            storeSales.TryRemove(firstSaleID, out Sale firstSale);
            storeSales.TryRemove(secondSaleID, out Sale secondSale);
            // composing the two sales togther
            Sale composedSale = null;
            switch (typeOfComposition)
            {
                case SaleCompositionType.XorComposition:
                    if(selectionRule == null)
                        return new Failure("For Composing With Xor, You Must Note Selection Rule");
                    composedSale = new XorSale(firstSale, secondSale, selectionRule);
                    break;
                case SaleCompositionType.MaxComposition:
                    composedSale = new MaxSale(firstSale, secondSale);
                    break;
                case SaleCompositionType.DoubleComposition:
                    composedSale = new DoubleSale(firstSale, secondSale);
                    break;
            }
            storeSales.TryAdd(composedSale.saleID, composedSale);
            return new Ok("The Composed Sale Was Added To The Store's Sale Policy");
        }

        // uncompose the sale - split it to the two diffrent sales
        public RegularResult uncomposeSale(int saleID)
        {

        }

        // builds the purchase policy by all the predicates in the store
        // build it by composing all predicates with AND composition
        private Sale buildSalePolicy()
        {
            Sale policySales = null;
            foreach (Sale sale in storeSales.Values)
            {
                if (policySales == null)
                {
                    policySales = sale;
                }
                else
                {
                    policySales = new DoubleSale(sale, policySales);
                }
            }
            return policySales;
        }

        // returns the price of each item after sales
        public ConcurrentDictionary<int, double> pricesAfterSalePolicy(ConcurrentDictionary<Item, int> items, PurchaseDetails purchaseDetails)
        {
            ConcurrentDictionary<int, double> pricesAfterSale = new ConcurrentDictionary<int, double>();
            double itemPriceAfterSale;
            Sale policySales = buildSalePolicy();
            foreach (Item item in items.Keys)
            {
                if(storeSales == null)
                {
                    itemPriceAfterSale = item.price;
                }
                else
                {
                    itemPriceAfterSale = policySales.applySaleOnItem(item, purchaseDetails);
                }
                pricesAfterSale.TryAdd(item.itemID, itemPriceAfterSale);
            }
            return pricesAfterSale;
        }
    }
}
