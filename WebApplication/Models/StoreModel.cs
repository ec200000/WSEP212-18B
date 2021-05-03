using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class StoreModel
    {
        [Required]
        public string storeName { get; set; }
        
        [Required]
        public string storeAddress { get; set; }
        
        [Required]
        public string purchasePolicy { get; set; }
        
        [Required]
        public string salesPolicy { get; set; }
        
        public int storeID { get; set; }
    }
}