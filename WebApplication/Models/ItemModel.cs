using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class ReviewModel
    {
        [Required]
        public string review { get; set; }
        
        [Required]
        public int itemID { get; set; }
        
        [Required]
        public int storeID { get; set; }
    }
}