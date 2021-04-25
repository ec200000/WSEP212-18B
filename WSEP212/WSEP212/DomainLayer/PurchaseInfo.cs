﻿using System;
using System.Collections.Concurrent;

namespace WSEP212.DomainLayer
{
    public class PurchaseInvoice
    {
        public int storeID { get; set; }
        public String userName { get; set; }
        // A data structure associated with a item ID and its quantity
        public ConcurrentDictionary<int, int> items { get; set; }
        public double totalPrice { get; set; }
        public DateTime dateOfPurchase { get; set; }

        public PurchaseInvoice(int storeID, String userName, ConcurrentDictionary<int, int> items, double totalPrice, DateTime dateOfPurchase)
        {
            this.storeID = storeID;
            this.userName = userName;
            this.items = items;
            this.totalPrice = totalPrice;
            this.dateOfPurchase = dateOfPurchase;
        }

        public bool wasItemPurchased(int itemID)
        {
            return items.ContainsKey(itemID);
        }
    }
}
