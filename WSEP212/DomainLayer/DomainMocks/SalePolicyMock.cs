using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class SalePolicyMock : SalePolicyInterface
    {
        public String salesPolicyName { get; set; }
        public ConcurrentDictionary<int, Sale> storeSales { get; set; }

        public SalePolicyMock() { }

        public int addSale(int salePercentage, ApplySaleOn saleOn, string saleDescription)
        {
            return -1;
        }

        // there are no sales in this policy
        public ResultWithValue<int> addSaleCondition(int saleID, SimplePredicate condition, SalePredicateCompositionType compositionType)
        {
            return null;
        }

        // there are no sales in this policy
        public ResultWithValue<int> composeSales(int firstSaleID, int secondSaleID, SaleCompositionType typeOfComposition, SimplePredicate selectionRule)
        {
            return null;
        }

        // there are no sales in this policy
        public ConcurrentDictionary<int, string> getSalesDescriptions()
        {
            return null;
        }

        // there are no sales in this policy
        public RegularResult removeSale(int saleID)
        {
            return null;
        }

        // returns the price of each item after sales - no sales!
        public ConcurrentDictionary<int, double> pricesAfterSalePolicy(ConcurrentDictionary<Item, double> itemsPrices, PurchaseDetails purchaseDetails)
        {
            ConcurrentDictionary<int, double> pricesAfterSale = new ConcurrentDictionary<int, double>();
            foreach (KeyValuePair<Item, double> itemPricePair in itemsPrices)
            {
                pricesAfterSale.TryAdd(itemPricePair.Key.itemID, itemPricePair.Value);
            }
            return pricesAfterSale;
        }
    }
}
