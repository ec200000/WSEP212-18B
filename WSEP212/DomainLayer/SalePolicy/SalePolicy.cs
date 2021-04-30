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
        // returns the id of the new sale
        public int addSale(int salePercentage, ApplySaleOn saleOn)
        {
            SimpleSale simpleSale = new SimpleSale(salePercentage, saleOn);
            storeSales.TryAdd(simpleSale.saleID, simpleSale);
            return simpleSale.saleID;
        }

        // remove sale from the store sale policy
        public RegularResult removeSale(int saleID)
        {
            if (storeSales.ContainsKey(saleID))
            {
                storeSales.TryRemove(saleID, out _);
                return new Ok("The Sale Was Removed To The Store's Sale Policy");
            }
            return new Failure("The Sale Is Not Exist In This Store Sale Policy");
        }

        // add conditional for getting the sale
        // if the sale already has conditionals, composed them by the composition type
        public ResultWithValue<int> addSaleCondition(int saleID, Predicate<PurchaseDetails> predicate, SalePredicateCompositionType compositionType)
        {
            if(storeSales.ContainsKey(saleID))
            {
                SimplePredicate simplePredicate = new SimplePredicate(predicate);
                storeSales.TryRemove(saleID, out Sale sale);
                ConditionalSale conditionalSale = sale.addSaleCondition(simplePredicate, compositionType);
                storeSales.TryAdd(conditionalSale.saleID, conditionalSale);
                return new OkWithValue<int>("The New Conditional Sale Added To The Store's Sale Policy", conditionalSale.saleID);
            }
            return new FailureWithValue<int>("The Sale Is Not Exist In This Store Sale Policy", -1);
        }

        // compose two sales by the type of sale 
        public ResultWithValue<int> composeSales(int firstSaleID, int secondSaleID, SaleCompositionType typeOfComposition, Predicate<PurchaseDetails> selectionRule)
        {
            if (!storeSales.ContainsKey(firstSaleID) || !storeSales.ContainsKey(secondSaleID))
            {
                return new FailureWithValue<int>("One Or More Of The Sales Are Not Exist In This Store Sale Policy", -1);
            }
            storeSales.TryRemove(firstSaleID, out Sale firstSale);
            storeSales.TryRemove(secondSaleID, out Sale secondSale);
            // composing the two sales togther
            Sale composedSale = null;
            switch (typeOfComposition)
            {
                case SaleCompositionType.XorComposition:
                    if(selectionRule == null)
                        return new FailureWithValue<int>("For Composing With Xor, You Must Note Selection Rule", -1);
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
            return new OkWithValue<int>("The Composed Sale Was Added To The Store's Sale Policy", composedSale.saleID);
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
                if(policySales == null)
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
