using System;
using System.Collections.Concurrent;
using WSEP212.DomainLayer;

namespace WebApplication.Models
{
    public class ShowReviewsModel
    {
        public int itemID { get; set; }
        public ConcurrentDictionary<String, ItemReview> reviews { get; set; }
        
        public string reviewsStrings { get; set; }
    }
}