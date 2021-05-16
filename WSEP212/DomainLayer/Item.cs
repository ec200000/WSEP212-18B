using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class Item
    {
        [JsonIgnore]
        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        
        [NotMapped]
        private static int itemCounter = 1;
        
        private readonly object quantitylock = new object();
        [Key]
        public int itemID { get; set; }   // different item ID for same item in different stores -> example: water is 2 in store A, and 3 in store B
        public int quantity { get; set; }
        public String itemName { get; set; }
        public String description { get; set; }
        // A data structure associated with a user name and his reviews
        public ConcurrentDictionary<String, ItemReview> reviews { get; set; }
        public double price { get; set; }
        public String category { get; set; }
        
        public string DictionaryAsJson
        {
            get => JsonConvert.SerializeObject(reviews,settings);
            set => reviews = JsonConvert.DeserializeObject<ConcurrentDictionary<string, ItemReview>>(value);
        }
        
        public Item(){}
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
            
            SystemDBAccess.Instance.Items.Add(this);
            SystemDBAccess.Instance.SaveChanges();
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
                areview.addReview(review);
                reviews.TryAdd(username, areview);
            }
            var result = SystemDBAccess.Instance.Items.SingleOrDefault(i => i.itemID == this.itemID);
            if (result != null)
            {
                result.reviews = reviews;
                if(!JToken.DeepEquals(result.DictionaryAsJson, this.DictionaryAsJson))
                    result.DictionaryAsJson = this.DictionaryAsJson;
                SystemDBAccess.Instance.SaveChanges();
            }
        }

        public bool setQuantity(int quantity)
        {
            lock (quantitylock)
            {
                if (quantity >= 0)
                {
                    this.quantity = quantity;
                    var result = SystemDBAccess.Instance.Items.SingleOrDefault(i => i.itemID == this.itemID);
                    if (result != null)
                    {
                        result.quantity = quantity;
                        SystemDBAccess.Instance.SaveChanges();
                    }
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
                    var result = SystemDBAccess.Instance.Items.SingleOrDefault(i => i.itemID == this.itemID);
                    if (result != null)
                    {
                        result.quantity = quantity;
                        SystemDBAccess.Instance.SaveChanges();
                    }
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

        public RegularResult editItem(String itemName, String description, double price, String category, int quantity)
        {
            if (itemName.Equals("") || price <= 0 || category.Equals(""))
                return new Failure("One Or More Of The New Item Details Are Invalid");
            this.itemName = itemName;
            this.description = description;
            this.price = price;
            this.category = category;
            if (this.setQuantity(quantity))
            {
                var result = SystemDBAccess.Instance.Items.SingleOrDefault(i => i.itemID == this.itemID);
                if (result != null)
                {
                    result.itemName = itemName;
                    result.description = description;
                    result.price = price;
                    result.category = category;
                    result.quantity = quantity;
                    SystemDBAccess.Instance.SaveChanges();
                }
                return new Ok("Item Details Have Been Successfully Updated In The Store");
            }

            return new Failure("Item quantity can't be negative");
        }
    }
}
