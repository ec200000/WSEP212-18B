using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.PurchaseTypes
{
    public class ItemImmediatePurchase : ItemPurchaseType
    {
        public ItemImmediatePurchase(double itemPrice)
        {
            this.priceStatus = PriceStatus.Approved;
            this.itemPrice = itemPrice;
        }

        public override PurchaseType getPurchaseType()
        {
            return PurchaseType.ImmediatePurchase;
        }

        // item price of immediate purchase is always approve 
        public override RegularResult changeItemPriceStatus(PriceStatus status)
        {
            return new Failure("Cannot Change Immediate Purchase Item Price Status");
        }

        // item price of immediate purchase is always the regular price
        public override RegularResult changeItemPrice(double newItemPrice)
        {
            return new Failure("Cannot Change Immediate Purchase Item Price");
        }
    }
}
