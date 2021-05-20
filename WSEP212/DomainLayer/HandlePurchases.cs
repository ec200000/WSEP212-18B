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
            paymentSystem = PaymentSystemAdapter.Instance;
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

        private double calculateTotalPrice(ConcurrentDictionary<int, PurchaseInvoice> purchaseInvoices)
        {
            double totalPrice = 0;
            foreach (PurchaseInvoice purchaseInvoice in purchaseInvoices.Values)
            {
                totalPrice += purchaseInvoice.getPurchaseTotalPrice();
            }
            return totalPrice;
        }

        private void rollback(User user, ConcurrentDictionary<int, PurchaseInvoice> purchaseInvoices)
        {
            user.shoppingCart.rollBackBags(purchaseInvoices);
        }

        public ResultWithValue<ConcurrentLinkedList<string>> purchaseItems(User user, String address)
        {
            if (address == null) 
                return new FailureWithValue<ConcurrentLinkedList<string>>("address is null!", null);

            // if the purchase cannot be made returns null
            // create purchase invoices
            ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> purchaseRes = user.shoppingCart.purchaseItemsInCart();
            if (purchaseRes.getTag())
            {
                ConcurrentDictionary<int, PurchaseInvoice> purchaseInvoices = purchaseRes.getValue();
                // calculate final price by all the invoices
                double totalPrice = calculateTotalPrice(purchaseInvoices);
                // call payment system
                RegularResult externalPurchaseRes = externalPurchase(totalPrice, user);
                if (externalPurchaseRes.getTag())
                {
                    // call delivery system
                    RegularResult deliveryRes = callDeliverySystem(user, address);
                    if (deliveryRes.getTag())
                    {
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
                    rollback(user, purchaseInvoices);
                    return new FailureWithValue<ConcurrentLinkedList<string>>(deliveryRes.getMessage(),null);
                }
                rollback(user, purchaseInvoices);
                return new FailureWithValue<ConcurrentLinkedList<string>>(externalPurchaseRes.getMessage(),null);
            }
            return new FailureWithValue<ConcurrentLinkedList<string>>(purchaseRes.getMessage(), null);
        }
    }
}