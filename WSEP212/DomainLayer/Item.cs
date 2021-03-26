using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class Item
    {
        public int itemID { get; set; } //deifferent item ID for same item in different stores -> example: water is 2 in store A, and 3 in store B
        public int storeID { get; set; }
        public String itemName { get; set; }
        public String description { get; set; }
        public Dictionary<String,String> review { get; set; }
        public double price { get; set; }
        public int rank { get; set; }
        public String category { get; set; }
    }
}
