using System;
using System.Collections.Concurrent;
using System.Threading;
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
        public DeliveryInterface deliverySystem { get; set; }

        private HandlePurchases() {
            paymentSystem = PaymentSystem.Instance;
            deliverySystem = DeliverySystem.Instance;
        }

        // returns the total price after sales for each store. if the purchase cannot be made returns null
        private ResultWithValue<ConcurrentDictionary<int, double>> calculatePurchaseTotal(User user)
        {
            ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseType>> purchaseTypes = new ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseType>>();
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                purchaseTypes.TryAdd(shoppingBag.store.storeID, getBagPurchaseTypes(shoppingBag));
            }
            return user.shoppingCart.purchaseItemsInCart(user, purchaseTypes);
        }

        private RegularResult externalPurchase(double amount, User user)
        {
            if(Math.Abs(amount - paymentSystem.paymentCharge(amount)) < 0.01)
            {
                return new Ok("Payment Charged Successfully");
            }
            return new Failure("Payment Charge Failed");
        }

        private void rollback(User user)
        {
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                shoppingBag.rollBackItems(); // if the purchase failed
            }
        }

        private ConcurrentDictionary<int, PurchaseType> getBagPurchaseTypes(ShoppingBag shoppingBag)
        {
            ConcurrentDictionary<int, PurchaseType> purchaseTypes = new ConcurrentDictionary<int, PurchaseType>();
            foreach (int itemID in shoppingBag.items.Keys)
            {
                purchaseTypes.TryAdd(itemID, PurchaseType.ImmediatePurchase); // getting the purchase types for each item
            }
            return purchaseTypes;
        }

        private RegularResult createPurchaseInfos(User user, ConcurrentDictionary<int, double> pricePerStore)
        {
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                int storeID = shoppingBag.store.storeID;
                if(pricePerStore.ContainsKey(storeID))
                {
                    PurchaseInfo purchaseInfo = new PurchaseInfo(storeID, user.userName, shoppingBag.items, pricePerStore[storeID], DateTime.Now);
                    user.purchases.Add(purchaseInfo);
                    shoppingBag.store.addNewPurchase(purchaseInfo);
                }
                else
                {
                    return new Failure("The Final Price For The Store Does Not Exist");
                }
            }
            return new Ok("All Purchase Histories Has Been Successfully Added");
        }

        // Deliver all the items to the address
        // returns true if the delivery done successfully
        private RegularResult callDeliverySystem(User user, String address)
        {
            ConcurrentDictionary<int, int> allItems = new ConcurrentDictionary<int, int>();
            int indexToAppend = 0;
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                shoppingBag.items.ToArray().CopyTo(allItems.ToArray(), indexToAppend);
                indexToAppend += shoppingBag.items.Count;
            }
            return deliverySystem.deliverItems(address, allItems);
        }

        public RegularResult purchaseItems(User user, String address)
        {
            if(address == null) return new Failure("address is null!");
            ResultWithValue<ConcurrentDictionary<int, double>> pricePerStoreRes = calculatePurchaseTotal(user);
            if(pricePerStoreRes.getTag())
            {
                // calculate total price for all stores
                double totalPrice = 0;
                foreach (double price in pricePerStoreRes.getValue().Values)
                {
                    totalPrice += price;
                }

                RegularResult externalPurchaseRes = externalPurchase(totalPrice, user);
                if (externalPurchaseRes.getTag())
                {
                    RegularResult deliveryRes = callDeliverySystem(user, address);
                    if(deliveryRes.getTag())
                    {
                        RegularResult purchaseInfosRes = createPurchaseInfos(user, pricePerStoreRes.getValue());
                        if(purchaseInfosRes.getTag())
                        {
                            user.shoppingCart.clearShoppingCart();
                            return new Ok("Purchase Completed Successfully!");
                        }
                        rollback(user);
                        return purchaseInfosRes;
                    }
                    rollback(user);
                    return deliveryRes;
                }
                rollback(user);
                return externalPurchaseRes;
            }
            return new Failure(pricePerStoreRes.getMessage());
        }
    }
}