using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure.Design;
using System.Linq;
using WSEP212.DataAccessLayer;

namespace WSEP212.DomainLayer.PurchaseTypes
{
    public class BidInfo
    {
        [Key]
        public int bidID { get; set; }
        
        public int itemID { get; set; }

        public string buyer {get; set; }
        
        public double itemPrice { get; set; }

        public BidInfo()
        {
            
        }

        public BidInfo(int itemId, string buyer, double price)
        {
            this.itemID = itemId;
            this.buyer = buyer;
            this.itemPrice = price;
            this.bidID = SystemDBAccess.Instance.Bids.Count() + 1;
        }
    }
}