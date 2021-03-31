using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class Item
    {
        private static int itemCounter = 1;

        public int itemID { get; set; }   // different item ID for same item in different stores -> example: water is 2 in store A, and 3 in store B
        public int quantity { get; set; }
        public String itemName { get; set; }
        public String description { get; set; }
        // A data structure associated with a user name and his review 
        public ConcurrentDictionary<String, LinkedList<String>> reviews { get; set; }
        public double price { get; set; }
        public String category { get; set; }

        public Item(int quantity, String itemName, String description, double price, String category)
        {
            this.itemID = itemCounter;
            itemCounter++;
            this.quantity = quantity;
            this.itemName = itemName;
            this.description = description;
            this.reviews = new ConcurrentDictionary<string, LinkedList<string>>();
            this.price = price;
            this.category = category;
        }

        // Add new review about an item
        public bool addReview(String username, String review)   
        {
            if(reviews.ContainsKey(username))
            {
                reviews[username].AddFirst(review);
                return true;
            }
            else
            {
                LinkedList<String> newUserReviews = new LinkedList<string>();
                newUserReviews.AddFirst(review);
                return reviews.TryAdd(username, newUserReviews);
            }
        }

    }
}
