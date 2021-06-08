using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.PurchaseTypes
{
    public class ItemSubmitOfferPurchase : ItemPurchaseType
    {
        public LinkedList<string> storeOwners { get; set; }
        
        public int storeID { get; set; }
        public ItemSubmitOfferPurchase(double itemPrice, int storeID)
        {
            if(itemPrice <= 0)
            {
                throw new ArithmeticException();
            }
            this.priceStatus = PriceStatus.Pending;
            this.itemPrice = itemPrice;
            this.storeID = storeID;
            this.storeOwners = concurrentToList(StoreRepository.Instance.getStoreOwners(storeID));
        }

        public override PurchaseType getPurchaseType()
        {
            return PurchaseType.SubmitOfferPurchase;
        }

        public override RegularResult changeItemPriceStatus(PriceStatus status, string userName)
        {
            if (status == PriceStatus.Pending)
            {
                this.storeOwners = concurrentToList(StoreRepository.Instance.getStoreOwners(storeID));
                this.priceStatus = status;
            }
            
            if (status == PriceStatus.Rejected || status == PriceStatus.CounterOffer)
            {
                this.storeOwners = new LinkedList<string>();
                this.priceStatus = status;
            }

            if (status == PriceStatus.Approved)
            {
                if (this.storeOwners.Contains(userName))
                {
                    this.storeOwners.Remove(userName);
                    if (this.storeOwners.Count == 0) // all store owners approved
                    {
                        this.priceStatus = status;
                    }
                    else
                    {
                        this.priceStatus = PriceStatus.Pending;
                    }
                }
                else
                    return new Failure("Store owner doesn't exist");
            }
            //this.priceStatus = status;
            return new Ok("Change Item Price Status");
        }

        public override RegularResult changeItemPrice(double newItemPrice)
        {
            this.itemPrice = newItemPrice;
            return new Ok("Change Submit Offer Item Price");
        }

        private LinkedList<string> concurrentToList(ConcurrentLinkedList<string> lst)
        {
            LinkedList<string> arr = new LinkedList<string>();
            int i = 0;
            Node<string> node = lst.First; // going over the user's permissions to check if he is a store manager or owner
            int size = lst.size;
            while(size > 0)
            {
                arr.AddFirst(node.Value);
                node = node.Next;
                i++;
                size--;
            }
            return arr;
        }
    }
}
