using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy;

namespace WSEP212.DomainLayer
{
    public class PurchaseDetails
    {
        public User user { get; set; }
        // A data structure associated with a item and its quantity
        public ConcurrentDictionary<Item, int> shoppingBagItems { get; set; }
        public ConcurrentDictionary<int, double> itemsPurchasePrices { get; set; }
        public DateTime dateOfPurchase { get; set; }

        public PurchaseDetails(User user, ConcurrentDictionary<Item, int> shoppingBagItems, ConcurrentDictionary<int, double> itemsPurchasePrices)
        {
            this.user = user;
            this.shoppingBagItems = shoppingBagItems;
            this.itemsPurchasePrices = itemsPurchasePrices;
            this.dateOfPurchase = DateTime.Now;
        }

        public double totalPurchasePrice()
        {
            double totalPrice = 0;
            foreach (KeyValuePair<Item, int> itemAndQuantity in shoppingBagItems)
            {
                totalPrice += itemsPurchasePrices[itemAndQuantity.Key.itemID] * itemAndQuantity.Value;   // item price * item quantity
            }
            return totalPrice;
        }
        public double userAge()
        {
            return user.userAge;
        }

        public double totalPurchasePriceAfterSale(Sale sale)
        {
            double totalPrice = 0;
            foreach (KeyValuePair<Item, int> itemAndQuantity in shoppingBagItems)
            {
                totalPrice += sale.applySaleOnItem(itemAndQuantity.Key, itemsPurchasePrices[itemAndQuantity.Key.itemID], this) * itemAndQuantity.Value;   // item price after sale * item quantity
            }
            return totalPrice;
        }

        public int numOfItemsInPurchase()
        {
            int numOfItems = 0;
            foreach (int quantity in shoppingBagItems.Values)
            {
                numOfItems += quantity;
            }
            return numOfItems;
        }

        public bool atLeastNQuantity(int itemID, int itemQuantity)
        {
            foreach (KeyValuePair<Item, int> itemAndQuantity in shoppingBagItems)
            {
                if(itemAndQuantity.Key.itemID == itemID)
                {
                    return itemAndQuantity.Value >= itemQuantity;
                }
            }
            return false;
        }

        public bool atMostNQuantity(int itemID, int itemQuantity)
        {
            foreach (KeyValuePair<Item, int> itemAndQuantity in shoppingBagItems)
            {
                if (itemAndQuantity.Key.itemID == itemID)
                {
                    return itemAndQuantity.Value <= itemQuantity;
                }
            }
            return false;
        }
    }
}
