namespace WebApplication.Models
{
    public class SearchModel
    {
        public int[] itemIDs { get; set; }
        
        public string[] items { get; set; }
        
        public string itemChosen { get; set; }
        
        public int quantity { get; set; }

        public string itemName { get; set; }
        
        public string keyWords { get; set; }
        public double minPrice { get; set; }
        public double maxPrice { get; set; }
        public string category { get; set; }
        
        public bool flag { get; set; }
        
        public string purchaseType { get; set; }
        
        public double priceOffer { get; set; }
    }
}