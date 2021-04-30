using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class UserModel
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        [Required]
        public int Age { get; set; }

    }
}