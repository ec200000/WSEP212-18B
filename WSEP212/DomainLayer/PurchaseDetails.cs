using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class PurchaseDetails
    {
        public User user { get; set; }
        // A data structure associated with a item and its quantity
        public ConcurrentDictionary<Item, int> shoppingBagItems { get; set; }
        public ConcurrentDictionary<int, PurchaseType> itemsPurchaseType { get; set; }
        public DateTime dateOfPurchase { get; set; }

        public PurchaseDetails(User user, ConcurrentDictionary<Item, int> shoppingBagItems, ConcurrentDictionary<int, PurchaseType> itemsPurchaseType)
        {
            this.user = user;
            this.shoppingBagItems = shoppingBagItems;
            this.itemsPurchaseType = itemsPurchaseType;
            this.dateOfPurchase = DateTime.Now;
        }

        public double totalPurchasePrice()
        {
            double totalPrice = 0;
            foreach (KeyValuePair<Item, int> itemAndQuantity in shoppingBagItems)
            {
                totalPrice += itemAndQuantity.Key.price * itemAndQuantity.Value;   // item price * item quantity
            }
            return totalPrice;
        }

        public double totalPurchasePriceAfterSale(Sale sale)
        {
            double totalPrice = 0;
            foreach (KeyValuePair<Item, int> itemAndQuantity in shoppingBagItems)
            {
                totalPrice += sale.applySaleOnItem(itemAndQuantity.Key, this) * itemAndQuantity.Value;   // item price after sale * item quantity
            }
            return totalPrice;
        }
    }
}
