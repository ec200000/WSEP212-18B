using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.SalePolicy
{
    public interface SalePolicyInterface
    {
        public String salesPolicyName { get; set; }
        public ConcurrentDictionary<int, Sale> storeSales { get; set; } //TODO: JSON

        public int addSale(int salePercentage, ApplySaleOn saleOn, String saleDescription);
        public RegularResult removeSale(int saleID);
        public ResultWithValue<int> addSaleCondition(int saleID, SimplePredicate condition, SalePredicateCompositionType compositionType);
        public ResultWithValue<int> composeSales(int firstSaleID, int secondSaleID, SaleCompositionType typeOfComposition, SimplePredicate selectionRule);
        public ConcurrentDictionary<int, String> getSalesDescriptions();
        public ConcurrentDictionary<int, double> pricesAfterSalePolicy(ConcurrentDictionary<Item, double> itemsPrices, PurchaseDetails purchaseDetails);
    }
}
