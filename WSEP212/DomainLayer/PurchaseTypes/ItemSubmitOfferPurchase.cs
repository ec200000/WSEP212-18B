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
            this.priceApproved = false;
            this.itemPrice = itemPrice;
        }

        public override RegularResult approveItemPrice()
        {
            this.priceApproved = true;
            return new Ok("Approved Submit Offer Item Price");
        }

        public override RegularResult rejectItemPrice()
        {
            this.priceApproved = false;
            return new Ok("Rejected Submit Offer Item Price");
        }

        public override RegularResult changeItemPrice(double newItemPrice)
        {
            // cannot bid new offer that is less then the prev bid
            if(newItemPrice < this.itemPrice)
            {
                return new Failure("Cannot Submit New Offer That Is Less Then The Previous Offer");
            }
            this.itemPrice = newItemPrice;
            return new Ok("Change Submit Offer Item Price");
        }
    }
}
