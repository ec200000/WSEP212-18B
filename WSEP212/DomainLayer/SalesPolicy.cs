using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class SalesPolicy
    {
        public String salesPolicyName { get; set; }
        public ConcurrentLinkedList<SalesType> saleTypes { get; set; }
        // ASSUMPTION! If there is a guest on the list then the list also has all the types of users above it
        public ConcurrentLinkedList<UserType> allowedUsers { get; set; }
        public ConcurrentLinkedList<SalesPolicyRule> policyRules { get; set; }

        public SalesPolicy(String salesPolicyName, ConcurrentLinkedList<SalesType> saleTypes, ConcurrentLinkedList<UserType> allowedUsers, ConcurrentLinkedList<SalesPolicyRule> policyRules)
        {
            this.salesPolicyName = salesPolicyName;
            this.saleTypes = saleTypes;
            this.allowedUsers = allowedUsers;
            this.policyRules = policyRules;
        }

        // checks that the user can buy in the store
        // checks that the purchase type is allowed in the store
        // checks all the other rules of the store policy
        public bool approveBySalesPolicy(UserType userType, ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, SalesType> itemsSaleType)
        {
            // type of user is approved
            if (allowedUsers.Contains(userType))
            {
                // checks that all type of purchase are fine
                foreach (KeyValuePair<int, PurchaseType> purchaseType in itemsPurchaseType)
                {
                    if (!purchaseRoutes.Contains(purchaseType.Value))
                    {
                        return false;
                    }
                }

                // checks other rules
                Node<PurchasePolicyRule> ruleNode = policyRules.First;
                while (ruleNode != null)
                {
                    if (!ruleNode.Value.applyRule(userType, items, itemsPurchaseType))
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
