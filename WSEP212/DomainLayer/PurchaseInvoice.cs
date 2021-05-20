﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WSEP212.DomainLayer
{
    public class PurchaseInvoice
    {
        // static counter for the purchaseInvoices
        private static int invoiceCounter = 1;

        public int purchaseInvoiceID { get; set; }
        public int storeID { get; set; }
        public String userName { get; set; }
        // A data structure associated with a item ID and its quantity
        public ConcurrentDictionary<int, int> items { get; set; }
        public ConcurrentDictionary<int, double> itemsPrices { get; set; }
        public DateTime dateOfPurchase { get; set; }

        public PurchaseInvoice(int storeID, String userName, ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, double> itemsPrices, DateTime dateOfPurchase)
        {
            this.purchaseInvoiceID = invoiceCounter;
            invoiceCounter++;
            this.storeID = storeID;
            this.userName = userName;
            this.items = items;
            this.itemsPrices = itemsPrices;
            this.dateOfPurchase = dateOfPurchase;
        }

        public double getPurchaseTotalPrice()
        {
            double totalPrice = 0;
            foreach (KeyValuePair<int, double> itemPricePair in itemsPrices)
            {
                totalPrice += itemPricePair.Value * items[itemPricePair.Key];
            }
            return totalPrice;
        }

        public bool wasItemPurchased(int itemID)
        {
            return items.ContainsKey(itemID);
        }

        public override string ToString()
        {
            string value = "";
            foreach (KeyValuePair<int, int> item in items)
            {
                value += "Item Name: " + StoreRepository.Instance.getStore(storeID).getValue().storage[item.Key].itemName + 
                         ", Item ID: " + item.Key + 
                         ", Item quantity: " + item.Value + 
                         ", In Store ID: " + storeID + ";";
            }

            value = value.Substring(0, value.Length - 1);
            return value;
        }
    }
}
