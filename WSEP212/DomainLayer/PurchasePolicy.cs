using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class PurchasePolicy
    {
        public String purchasePolicyName { get; set; }
        public ConcurrentLinkedList<PurchaseType> purchaseRoutes { get; set; }
        //public ConcurrentLinkedList<UserType> allowedUsers { get; set; }
        public ConcurrentLinkedList<PolicyRule> policyRules { get; set; }

        public PurchasePolicy(String purchasePolicyName, ConcurrentLinkedList<PurchaseType> purchaseRoutes, ConcurrentLinkedList<PolicyRule> policyRules)
        {
            this.purchasePolicyName = purchasePolicyName;
            this.purchaseRoutes = purchaseRoutes;
            this.policyRules = policyRules;
        }

        // checks that the user can buy in the store
        // checks that the purchase type is allowed in the store
        // checks all the other rules of the store policy
        public bool approveByPurchasePolicy(User user, ConcurrentDictionary<Item, int> items, ConcurrentDictionary<int, PurchaseType> itemsPurchaseType)
        {
            // checks the user can purchase in the store
            if(true)
            {
                // checks that all type of purchase are fine
                foreach (KeyValuePair<int, PurchaseType> purchaseType in itemsPurchaseType)
                {
                    if(!purchaseRoutes.Contains(purchaseType.Value))
                    {
                        return false;
                    }
                }
                // checks other rules
                Node<PolicyRule> ruleNode = policyRules.First;
                while(ruleNode != null)
                {
                    if(!ruleNode.Value.applyRule(user, items))
                    {
                        return false;
                    }
                    ruleNode = ruleNode.Next;
                }
                return true;
            }
            return false;
        }
    }
}
