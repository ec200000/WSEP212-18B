using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class AppointModel
    {
        [Required]
        public string userName { get; set; }
    }
}