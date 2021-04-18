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

        private HandlePurchases() {
            
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
            if(Math.Abs(amount - PaymentSystem.Instance.paymentCharge(amount)) < 0.01)
            {
                return new Ok("Payment Charged Successfully");
            }
            return new Failure("Payment Charged Failed");
        }

        private void rollback(User user)
        {
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                shoppingBag.store.rollBackPurchase(shoppingBag.items); // if the purchase failed
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

        private RegularResult callDeliverySystem(User user, String address)
        {
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                RegularResult deliverRes = shoppingBag.store.deliverItems(address, shoppingBag.items);
                if(!deliverRes.getTag())
                {
                    return deliverRes;
                }
            }
            return new Ok("All Items Deliver To The User Successfully");
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
                    RegularResult purchaseInfosRes = createPurchaseInfos(user, pricePerStoreRes.getValue());
                    if(purchaseInfosRes.getTag())
                    {
                        RegularResult deliveryRes = callDeliverySystem(user, address);
                        if(deliveryRes.getTag())
                        {
                            user.shoppingCart.clearShoppingCart();
                            return new Ok("Purchase Completed Successfully!");
                        }
                        rollback(user);
                        return deliveryRes;
                    }
                    rollback(user);
                    return purchaseInfosRes;
                }
                rollback(user);
                return externalPurchaseRes;
            }
            return new Failure(pricePerStoreRes.getMessage());

            
        }
    }
}