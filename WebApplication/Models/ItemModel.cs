using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using WSEP212.DomainLayer;

namespace WebApplication.Models
{
    public class ItemModel
    {
        [Required]
        public int quantity { get; set; }
        
        [Required]
        public string itemName { get; set; }
        
        [Required]
        public string description { get; set; }
        
        [Required]
        public double price { get; set; }
        
        [Required]
        public string category { get; set; }
        
        public int itemID { get; set; }
        
        public int storeID { get; set; }
        
        public ConcurrentDictionary<String, ItemUserReviews> reviews { get; set; }
    }
}