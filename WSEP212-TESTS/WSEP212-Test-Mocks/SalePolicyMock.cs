using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TEST.UnitTests.UnitTestMocks
{
    public class SalePolicyMock : SalePolicyInterface
    {
        public SalePolicyMock() { }

        // there are no sales in this policy
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
        public ConcurrentDictionary<int, double> pricesAfterSalePolicy(ConcurrentDictionary<Item, int> items, PurchaseDetails purchaseDetails)
        {
            ConcurrentDictionary<int, double> pricesAfterSale = new ConcurrentDictionary<int, double>();
            foreach (Item item in items.Keys)
            {
                pricesAfterSale.TryAdd(item.itemID, item.price);
            }
            return pricesAfterSale;
        }
    }
}
