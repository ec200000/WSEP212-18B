using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class Item
    {
        private static int itemCounter = 1;

        public int itemID { get; set; }   // different item ID for same item in different stores -> example: water is 2 in store A, and 3 in store B
        public int quantity { get; set; }
        public String itemName { get; set; }
        public String description { get; set; }
        // A data structure associated with a user name and his review 
        public ConcurrentDictionary<String,String> review { get; set; }
        public double price { get; set; }
        public String category { get; set; }

        public Item(int quantity, String itemName, String description, double price, String category)
        {
            this.itemID = itemCounter;
            itemCounter++;
            this.quantity = quantity;
            this.itemName = itemName;
            this.description = description;
            this.review = new ConcurrentDictionary<string, string>();
            this.price = price;
            this.category = category;
        }
    }
}
