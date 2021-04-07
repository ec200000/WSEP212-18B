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
            return Math.Abs(amount - PaymentSystem.Instance.purchaseItems(user, amount)) < 0.01;
        }

        private void rollback(User user)
        {
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                shoppingBag.store.rollBackPurchase(shoppingBag.items);
            }
        }

        private ConcurrentDictionary<int, PurchaseType> getBagPurchaseTypes(ShoppingBag shoppingBag)
        {
            ConcurrentDictionary<int, PurchaseType> purchaseTypes = new ConcurrentDictionary<int, PurchaseType>();
            foreach (int itemID in shoppingBag.items.Keys)
            {
                purchaseTypes.TryAdd(itemID, PurchaseType.ImmediatePurchase);
            }
            return purchaseTypes;
        }

        private void createPurchaseInfos(User user)
        {
            foreach (ShoppingBag shoppingBag in user.shoppingCart.shoppingBags.Values)
            {
                double bagPrice = shoppingBag.purchaseItemsInBag(user, getBagPurchaseTypes(shoppingBag));
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
                shoppingBag.store.deliverItems(address, shoppingBag.items);
            }
        }

        public bool purchaseItems(User user, String address)
        {
            double total = calculatePurchaseTotal(user);
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