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
        protected Boolean priceApproved { get; set; }
        protected double itemPrice { get; set; }

        public Boolean isApproved()
        {
            return this.priceApproved;
        }

        public double getCurrentPrice()
        {
            return this.itemPrice;
        }

        public abstract PurchaseType getPurchaseType();
        public abstract RegularResult approveItemPrice();
        public abstract RegularResult rejectItemPrice();
        // change the item price - Depending on the different types of purchase
        public abstract RegularResult changeItemPrice(double newItemPrice);
    }
}
