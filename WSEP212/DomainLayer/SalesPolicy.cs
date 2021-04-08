using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.Result;

namespace WSEP212.DomainLayer
{
    public class SalesPolicy
    {
        public String salesPolicyName { get; set; }
        //public ConcurrentLinkedList<UserType> allowedUsers { get; set; }
        public ConcurrentLinkedList<PolicyRule> policyRules { get; set; }

        public SalesPolicy(String salesPolicyName, ConcurrentLinkedList<PolicyRule> policyRules)
        {
            this.salesPolicyName = salesPolicyName;
            this.policyRules = policyRules;
        }

        // checks that the user can get sales
        // checks all the other rules of the store policy
        // returns the price of each item after sales
        public ConcurrentDictionary<int, double> pricesAfterSalePolicy(User user, ConcurrentDictionary<Item, int> items)
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
                Node<PolicyRule> ruleNode = policyRules.First;
                while (ruleNode.Value != null)
                {
                    Result<Object> ruleRes = ruleNode.Value.applyRule(user, items);
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
