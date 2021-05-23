using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class ShoppingCartModel
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Address { get; set; }
        
        public string itemID { get; set; }
        public int storeID { get; set; }
        public int priceBeforeSale { get; set; }
        public int priceAfterSale { get; set; }
    }
}