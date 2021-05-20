using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace WSEP212.DomainLayer
{
    public class PurchaseInvoice
    {
        [Key]
        [Column(Order=1)]
        public int storeID { get; set; }
        [Key]
        [Column(Order=2)]
        public String userName { get; set; }
        // A data structure associated with a item ID and its quantity
        [NotMapped]
        public ConcurrentDictionary<int, int> items { get; set; }
        public double totalPrice { get; set; }
        [Key]
        [Column(Order=3)]
        public DateTime dateOfPurchase { get; set; }
        
        public string ItemsAsJson
        {
            get => JsonConvert.SerializeObject(items);
            set => items = JsonConvert.DeserializeObject<ConcurrentDictionary<int, int>>(value);
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

        public PurchaseInvoice(int storeID, String userName, ConcurrentDictionary<int, int> items, double totalPrice, DateTime dateOfPurchase)
        {
            this.storeID = storeID;
            this.userName = userName;
            this.items = items;
            this.totalPrice = totalPrice;
            this.dateOfPurchase = dateOfPurchase;
            SystemDBAccess.Instance.Invoices.Add(this);
            SystemDBAccess.Instance.SaveChanges();
        }

        public bool wasItemPurchased(int itemID)
        {
            return items.ContainsKey(itemID);
        }
    }
}
