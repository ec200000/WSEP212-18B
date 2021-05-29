using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.ExternalPaymentSystem;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.ConcurrentLinkedList;
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

        private ResultWithValue<int> externalPaymentSystem(PaymentParameters paymentParameters, double price)
        {
            int transactionID = paymentSystem.paymentCharge(paymentParameters.cardNumber, paymentParameters.month, paymentParameters.year,
                paymentParameters.holder, paymentParameters.ccv, paymentParameters.id, price);
            if(transactionID != -1)
            {
                return new OkWithValue<int>("External Payment System Charged", transactionID);
            }
            return new FailureWithValue<int>("External Payment System Not Charged", transactionID);
        }

        private ResultWithValue<ConcurrentDictionary<int, int>> externalDeliverySystem(User user, DeliveryParameters deliveryParameters)
        {
            ConcurrentDictionary<int, int> transactions = new ConcurrentDictionary<int, int>();
            int storeID, transactionID;
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                storeID = shoppingBag.store.storeID;
                transactionID = shoppingBag.store.deliverItems(deliveryParameters);
                if (transactionID == -1)
                {
                    rollbackDeliveries(transactions);
                    return new FailureWithValue<ConcurrentDictionary<int, int>>("One Or More Of The Deliveries Cannot Be Done", null);
                }
            }
            return new OkWithValue<ConcurrentDictionary<int, int>>("All Items Deliver To The User Successfully", transactions);
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

        private void rollbackDeliveries(ConcurrentDictionary<int, int> transactions)
        {
            Store store;
            foreach (KeyValuePair<int, int> storeTransPair in transactions)
            {
                store = StoreRepository.Instance.getStore(storeTransPair.Key).getValue();
                store.cancelDelivery(storeTransPair.Value);
            }
        }

        private void rollbackPayment(int transactionID)
        {
            paymentSystem.cancelPaymentCharge(transactionID);
        }

        private void rollbackPurchase(User user, ConcurrentDictionary<int, PurchaseInvoice> purchaseInvoices)
        {
            user.shoppingCart.rollBackBags(purchaseInvoices);
        }

        public ResultWithValue<ConcurrentLinkedList<string>> purchaseItems(User user, DeliveryParameters deliveryParameters, PaymentParameters paymentParameters)
        {
            // if the purchase cannot be made returns null
            // create purchase invoices
            ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> purchaseRes = user.shoppingCart.purchaseItemsInCart();
            if (purchaseRes.getTag())
            {
                ConcurrentDictionary<int, PurchaseInvoice> purchaseInvoices = purchaseRes.getValue();
                // calculate final price by all the invoices
                double totalPrice = calculateTotalPrice(purchaseInvoices);
                // call payment system
                ResultWithValue<int> paymentRes = externalPaymentSystem(paymentParameters, totalPrice);
                if (paymentRes.getTag())
                {
                    // call delivery system
                    ResultWithValue<ConcurrentDictionary<int, int>> deliveryRes = externalDeliverySystem(user, deliveryParameters);
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
                    rollbackPayment(paymentRes.getValue());
                    rollbackPurchase(user, purchaseInvoices);
                    return new FailureWithValue<ConcurrentLinkedList<string>>(deliveryRes.getMessage(),null);
                }
                rollbackPurchase(user, purchaseInvoices);
                return new FailureWithValue<ConcurrentLinkedList<string>>(paymentRes.getMessage(), null);
            }
            return new FailureWithValue<ConcurrentLinkedList<string>>(purchaseRes.getMessage(), null);
        }
    }
}