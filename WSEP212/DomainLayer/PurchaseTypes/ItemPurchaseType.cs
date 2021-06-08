using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.PurchaseTypes
{
    public abstract class ItemPurchaseType
    {
        public PriceStatus priceStatus { get; set; }
        public double itemPrice { get; set; }

        public PriceStatus getPriceStatus()
        {
            return this.priceStatus;
        }

        public double getCurrentPrice()
        {
            return this.itemPrice;
        }

        public abstract PurchaseType getPurchaseType();
        public abstract RegularResult changeItemPriceStatus(PriceStatus status, string userName = "");
        // change the item price - Depending on the different types of purchase
        public abstract RegularResult changeItemPrice(double newItemPrice);
    }
}
