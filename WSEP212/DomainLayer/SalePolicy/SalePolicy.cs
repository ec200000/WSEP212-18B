using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class SalePolicy
    {
        public String salesPolicyName { get; set; }
        public ConcurrentLinkedList<Sale> sales { get; set; }

        public SalePolicy(String salesPolicyName, ConcurrentLinkedList<Sale> sales)
        {
            this.salesPolicyName = salesPolicyName;
            this.sales = sales;
        }

        // returns the price of each item after sales
        public ConcurrentDictionary<int, double> pricesAfterSalePolicy(ConcurrentDictionary<Item, int> items, PurchaseDetails purchaseDetails)
        {
            ConcurrentDictionary<int, double> pricesAfterSale = new ConcurrentDictionary<int, double>();
            foreach (KeyValuePair<Item, int> item in items)
            {

                pricesAfterSale.TryAdd(item.Key.itemID, item.Key.price);
            }

            // checks the user can get sales in the store
            if (true)
            {
                // checks other rules
                Node<Sale> ruleNode = policyRules.First;
                while (ruleNode.Value != null)
                {
                    RegularResult ruleRes = ruleNode.Value.applyRule(user, items);
                    if (!ruleRes.getTag())
                    {
                        return pricesAfterSale;
                    }
                    ruleNode = ruleNode.Next;
                }

                // apply sales for items - for now there is no sales
                return pricesAfterSale;
            }
            //return pricesAfterSale;
        }
    }
}
