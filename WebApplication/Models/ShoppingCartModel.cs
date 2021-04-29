using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class ShoppingCartModel
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Address { get; set; }
        
        public int itemID { get; set; }
        public int storeID { get; set; }
    }
}