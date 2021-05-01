using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;
using System;

namespace WebApplication.Models
{
    public class PurchaseModel
    {
        [Required] public string itemInfo { get; set; }

    }

}