using System.ComponentModel.DataAnnotations;

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
    }
}