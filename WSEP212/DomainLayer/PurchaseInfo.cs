using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class PurchaseInfo
    {
        public int storeID { get; set; }
        public String userName { get; set; }
        // A data structure associated with a item ID and its quantity
        public Dictionary<int, int> items { get; set; }
        public double totalPrice { get; set; }
        public DateTime dateOfPurchase { get; set; }

        public PurchaseInfo(int storeID, String userName, Dictionary<int, int> items, double totalPrice, DateTime dateOfPurchase)
        {
            this.storeID = storeID;
            this.userName = userName;
            this.items = items;
            this.totalPrice = totalPrice;
            this.dateOfPurchase = dateOfPurchase;
        }

    }
}
