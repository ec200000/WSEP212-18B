using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Views.Home
{
    public class Login : PageModel
    {
        
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        public void OnGet()
        {
            
        }
    }
}