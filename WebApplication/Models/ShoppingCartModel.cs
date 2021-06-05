using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class ShoppingCartModel
    {
        public string itemID { get; set; }
        public int storeID { get; set; }
        
        public string cardNumber { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string holder { get; set; }
        public string ccv { get; set; }
        public string id { get; set; }
        
        public string sendToName { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
        public int priceBeforeSale { get; set; }
        public int priceAfterSale { get; set; }
        
        public double newOffer { get; set; }
        public string purchaseType { get; set; }
    }
}