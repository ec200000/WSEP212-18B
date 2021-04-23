using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class SubscribeModel
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }

    }
}