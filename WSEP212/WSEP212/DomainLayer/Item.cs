using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class Item
    {
        private static int itemCounter = 1;
        
        private readonly object quantitylock = new object();

        public int itemID { get; set; }   // different item ID for same item in different stores -> example: water is 2 in store A, and 3 in store B
        public int quantity { get; set; }
        public String itemName { get; set; }
        public String description { get; set; }
        // A data structure associated with a user name and his review 
        public ConcurrentDictionary<String, ItemReview> reviews { get; set; }
        public double price { get; set; }
        public String category { get; set; }

        public Item(int quantity, String itemName, String description, double price, String category)
        {
            this.itemID = itemCounter;
            itemCounter++;
            this.quantity = quantity;
            this.itemName = itemName;
            this.description = description;
            this.reviews = new ConcurrentDictionary<string, ItemReview>();
            this.price = price;
            this.category = category;
        }

        // Add new review about an item
        public void addReview(String username, String review)   
        {
            if(reviews.ContainsKey(username))
            {
                reviews[username].addReview(review);
            }
            else
            {
                ItemReview areview = new ItemReview(UserRepository.Instance.findUserByUserName(username).getValue());
                areview.reviews.TryAdd(review);
                reviews.TryAdd(username, areview);
            }
        }

        public bool setQuantity(int quantity)
        {
            lock (quantitylock)
            {
                if (quantity >= 0)
                {
                    this.quantity = quantity;
                    return true;
                }
                return false;
            }
        }
        
        public bool changeQuantity(int quantity)
        {
            lock (quantitylock)
            {
                if (this.quantity + quantity >= 0)
                {
                    this.quantity += quantity;
                    return true;
                }
                return false;
            }
        }
    }
}
