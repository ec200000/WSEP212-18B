using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ExternalPaymentSystem;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class HandlePurchases
    {
        private static readonly Lazy<HandlePurchases> lazy
            = new Lazy<HandlePurchases>(() => new HandlePurchases());

        public static HandlePurchases Instance
            => lazy.Value;

        public PaymentInterface paymentSystem { get; set; }

        private HandlePurchases()
        {
            paymentSystem = PaymentSystem.Instance;
        }

        private RegularResult externalPurchase(double amount, User user)
        {
            if (Math.Abs(amount - paymentSystem.paymentCharge(amount)) < 0.01)
            {
                return new Ok("Payment Charged Successfully");
            }
            return new Failure("Payment Charge Failed");
        }

        private RegularResult callDeliverySystem(User user, String address)
        {
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                RegularResult deliverRes = shoppingBag.store.deliverItems(address, shoppingBag.items);
                if (!deliverRes.getTag())
                {
                    return deliverRes;
                }
            }
            return new Ok("All Items Deliver To The User Successfully");
        }

        private void rollback(User user)
        {
            user.shoppingCart.rollBackShoppingCart();
        }

        public ResultWithValue<ConcurrentLinkedList<string>> purchaseItems(User user, String address)
        {
            if (address == null) 
                return new FailureWithValue<ConcurrentLinkedList<string>>("address is null!", null);

            // returns the total price after sales for all stores. if the purchase cannot be made returns -1
            ResultWithValue<double> finalPriceRes = user.shoppingCart.purchaseItemsInCart();
            if (finalPriceRes.getTag())
            {
                double totalPrice = finalPriceRes.getValue();
                // call payment system
                RegularResult externalPurchaseRes = externalPurchase(totalPrice, user);
                if (externalPurchaseRes.getTag())
                {
                    // call delivery system
                    RegularResult deliveryRes = callDeliverySystem(user, address);
                    if (deliveryRes.getTag())
                    {
                        // create purchase invoices
                        user.shoppingCart.createPurchaseInvoices();

                        var stores = user.shoppingCart.shoppingBags.Keys;
                        ConcurrentLinkedList<string> storeOwners = new ConcurrentLinkedList<string>();
                        foreach (var store in stores)
                        {
                            ConcurrentLinkedList<string> l = StoreRepository.Instance.getStoreOwners(store);
                            Node<string> node = l.First;
                            while (node.Next != null)
                            {
                                storeOwners.TryAdd(node.Value);
                                node = node.Next;
                            }
                        }
                        // clear the shopping cart of the user
                        user.shoppingCart.clearShoppingCart();
                        return new OkWithValue<ConcurrentLinkedList<string>>("Purchase Completed Successfully!", storeOwners);
                    }
                    rollback(user);
                    return new FailureWithValue<ConcurrentLinkedList<string>>(deliveryRes.getMessage(),null);
                }
                rollback(user);
                return new FailureWithValue<ConcurrentLinkedList<string>>(externalPurchaseRes.getMessage(),null);
            }
            return new FailureWithValue<ConcurrentLinkedList<string>>(finalPriceRes.getMessage(), null);
        }
    }
}