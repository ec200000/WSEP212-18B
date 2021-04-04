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
        // ASSUMPTION! If there is a guest on the list then the list also has all the types of users above it
        public ConcurrentLinkedList<UserType> allowedUsers { get; set; }
        public ConcurrentLinkedList<PurchasePolicyRule> policyRules { get; set; }

        public PurchasePolicy(String purchasePolicyName, ConcurrentLinkedList<PurchaseType> purchaseRoutes, ConcurrentLinkedList<UserType> allowedUsers, ConcurrentLinkedList<PurchasePolicyRule> policyRules)
        {
            this.purchasePolicyName = purchasePolicyName;
            this.purchaseRoutes = purchaseRoutes;
            this.allowedUsers = allowedUsers;
            this.policyRules = policyRules;
        }

        // checks that the user can buy in the store
        // checks that the purchase type is allowed in the store
        // checks all the other rules of the store policy
        public bool approveByPurchasePolicy(UserType userType, ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, PurchaseType> itemsPurchaseType)
        {
            // type of user is approved
            if(allowedUsers.Contains(userType))
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
                Node<PurchasePolicyRule> ruleNode = policyRules.First;
                while(ruleNode != null)
                {
                    if(!ruleNode.Value.applyRule(userType, items, itemsPurchaseType))
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
