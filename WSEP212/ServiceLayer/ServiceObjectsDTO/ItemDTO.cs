using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.ServiceLayer.ServiceObjectsDTO
{
    public class ItemDTO
    {
        public int itemID; //deifferent item ID for same item in different stores -> example: water is 2 in store A, and 3 in store B
        public int storeID;
        public int quantity;
        public String itemName;
        public String description;
        public ConcurrentDictionary<String, String> review;
        public double price;
        public int rank;
        public String category;

        public ItemDTO(int storeId, int quantity, string itemName, string description, ConcurrentDictionary<string, string> review, double price, string category)
        {
            storeID = storeId;
            this.quantity = quantity;
            this.itemName = itemName;
            this.description = description;
            this.review = review;
            this.price = price;
            this.category = category;
        }
    }
}
