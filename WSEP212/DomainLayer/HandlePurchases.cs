using System;
using System.Collections.Concurrent;

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

        private double calculatePurchaseTotal(User user)
        {
            ConcurrentDictionary<int, PurchaseType> purchaseTypes = new ConcurrentDictionary<int, PurchaseType>();
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                getBagPurchaseTypes(shoppingBag);
            }
            return user.shoppingCart.purchaseItemsInCart(user, purchaseTypes);
        }

        private bool externalPurchase(double amount, User user)
        {
            return Math.Abs(amount - PaymentSystem.Instance.purchaseItems(user, amount)) < 0.01; // checking the user was charged in the correct amount
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

        private void createPurchaseInfos(User user)
        {
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                double bagPrice = shoppingBag.purchaseItemsInBag(user, getBagPurchaseTypes(shoppingBag)); // the total of the prices in the bag
                PurchaseInfo purchaseInfo = new PurchaseInfo(shoppingBag.store.storeID, user.userName,
                    shoppingBag.items, bagPrice, DateTime.Now);
                user.purchases.Add(purchaseInfo);
                shoppingBag.store.addNewPurchase(purchaseInfo);
            }
        }

        private void callDeliverySystem(User user, String address)
        {
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                shoppingBag.store.deliverItems(address, shoppingBag.items); // getting the deliveries for each bag
            }
        }

        public bool purchaseItems(User user, String address)
        {
            double total = calculatePurchaseTotal(user); // calculating the total amount
            if (externalPurchase(total, user))
            {
                createPurchaseInfos(user); 
                callDeliverySystem(user, address);
                return true;
            }
            else
            {
                rollback(user);
                return false;
            }
        }
    }
}