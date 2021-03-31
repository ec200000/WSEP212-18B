using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.ServiceLayer
{
    class ItemDTO
    {
        public int itemID; //deifferent item ID for same item in different stores -> example: water is 2 in store A, and 3 in store B
        public int storeID;
        public String itemName;
        public String description;
        public ConcurrentDictionary<String, String> review;
        public double price;
        public int rank;
        public String category;
    }
}
