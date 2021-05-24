﻿using System;
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
        // A data structure associated with a user name and his reviews
        public ConcurrentDictionary<String, ItemReview> reviews { get; set; }
        public double price { get; set; }
        public ItemCategory category { get; set; }

        public Item(int quantity, String itemName, String description, double price, ItemCategory category)
        {
            if (price <= 0 || itemName.Equals("") || category.Equals("") || !setQuantity(quantity))
            {
                throw new ArithmeticException();
            }
            this.itemID = itemCounter;
            itemCounter++;
            this.itemName = itemName;
            this.description = description;
            this.reviews = new ConcurrentDictionary<string, ItemReview>();
            this.price = price;
            this.category = category;
        }

        // Add new review about an item
        public void addUserReviews(ItemReview review)   
        {
            String userName = review.reviewer.userName;
            if (!reviews.ContainsKey(userName))
            {
                reviews.TryAdd(userName, review);
            }
        }

        // remove review about an item
        public void removeUserReviews(String userName)
        {
            if (reviews.ContainsKey(userName))
            {
                reviews.TryRemove(userName, out _);
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

        // change the quantity of item by numOfItems
        // if quantity is less then zero, doesnt apply the action
        public bool changeQuantity(int numOfItems)
        {
            lock (quantitylock)
            {
                if (this.quantity + numOfItems >= 0)
                {
                    this.quantity += numOfItems;
                    return true;
                }
                return false;
            }
        }

        public override string ToString()
        {
            return "Item name: " + itemName + " Item description: " + description + " Item price: " + price +
                   " Item category: " + category + " Item quantity: " + quantity+ " Item ID: "+itemID;
        }
    }
}
