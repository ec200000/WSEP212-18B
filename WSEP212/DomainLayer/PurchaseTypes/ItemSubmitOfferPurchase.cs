using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.PurchaseTypes
{
    public class ItemSubmitOfferPurchase : ItemPurchaseType
    {
        public ItemSubmitOfferPurchase(double itemPrice)
        {
            if(itemPrice <= 0)
            {
                throw new ArithmeticException();
            }
            this.priceStatus = PriceStatus.Pending;
            this.itemPrice = itemPrice;
        }

        public override PurchaseType getPurchaseType()
        {
            return PurchaseType.SubmitOfferPurchase;
        }

        public override RegularResult changeItemPriceStatus(PriceStatus status)
        {
            this.priceStatus = status;
            return new Ok("Change Item Price Status");
        }

        public override RegularResult changeItemPrice(double newItemPrice)
        {
            this.itemPrice = newItemPrice;
            this.priceStatus = PriceStatus.Pending;
            return new Ok("Change Submit Offer Item Price");
        }
    }
}
