﻿using System;
using System.Collections.Concurrent;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.ExternalPaymentSystem;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public abstract class UserState
    {
        public User user { get; set; }

        public UserState(User user)
        {
            this.user = user;
        }

        // * User Management In The System * //
        public abstract RegularResult register(String userName, int userAge, String password);

        public abstract RegularResult login(String userName, String password);   
        public abstract RegularResult loginAsSystemManager(String userName, String password);

        public abstract RegularResult continueAsGuest(String userName);
        public abstract RegularResult logout(String userName);
        // * End Of User Management In The System * //


        // * Purchase Items Functions *//
        public RegularResult addItemToShoppingCart(int storeID, int itemID, int quantity, ItemPurchaseType purchaseType)
        {
            return this.user.shoppingCart.addItemToShoppingBag(storeID, itemID, quantity, purchaseType);
        }
        public RegularResult removeItemFromShoppingCart(int storeID, int itemID)
        {
            return this.user.shoppingCart.removeItemFromShoppingBag(storeID, itemID);
        }
        public RegularResult changeItemQuantityInShoppingCart(int storeID, int itemID, int updatedQuantity)
        {
            return this.user.shoppingCart.changeItemQuantityInShoppingBag(storeID, itemID, updatedQuantity);
        }
        public RegularResult changeItemPurchaseType(int storeID, int itemID, ItemPurchaseType itemPurchaseType)
        {
            return this.user.shoppingCart.changeItemPurchaseTypeInShoppingBag(storeID, itemID, itemPurchaseType);
        }
        public RegularResult submitPriceOffer(int storeID, int itemID, double offerItemPrice)
        {
            return this.user.shoppingCart.submitPriceOffer(storeID, itemID, offerItemPrice);
        }
        public RegularResult counterOfferDecision(int storeID, int itemID, double counterOffer, PriceStatus myDecision)
        {
            return this.user.shoppingCart.counterOfferDecision(storeID, itemID, counterOffer, myDecision);
        }
        public abstract RegularResult confirmPriceStatus(String userName, int storeID, int itemID, PriceStatus priceStatus);
        public ResultWithValue<ConcurrentLinkedList<string>> purchaseItems(DeliveryParameters deliveryParameters, PaymentParameters paymentParameters)
        {
            return HandlePurchases.Instance.purchaseItems(this.user, deliveryParameters, paymentParameters); // handling the purchase procedure
        }
        // * End Of Purchase Items Functions *//


        // * Store Storage Management * //
        public abstract ResultWithValue<int> openStore(String storeName, String storeAddress, PurchasePolicyInterface purchasePolicy, SalePolicyInterface salesPolicy);
        public abstract ResultWithValue<int> addItemToStorage(int storeID, int quantity, String itemName, String description, double price, ItemCategory category);
        public abstract RegularResult removeItemFromStorage(int storeID, int itemID);
        public abstract RegularResult editItemDetails(int storeID, int itemID, int quantity, String itemName, String description, double price, ItemCategory category);
        // * End Of Store Storage Management * //


        // * Store Policies Management * //
        public abstract RegularResult supportPurchaseType(int storeID, PurchaseType purchaseType);
        public abstract RegularResult unsupportPurchaseType(int storeID, PurchaseType purchaseType);
        public abstract ResultWithValue<int> addPurchasePredicate(int storeID, Predicate<PurchaseDetails> newPredicate, String predDescription);
        public abstract RegularResult removePurchasePredicate(int storeID, int predicateID);
        public abstract ResultWithValue<int> composePurchasePredicates(int storeID, int firstPredicateID, int secondPredicateID, PurchasePredicateCompositionType typeOfComposition);
        public abstract ResultWithValue<int> addSale(int storeID, int salePercentage, ApplySaleOn saleOn, String saleDescription);
        public abstract RegularResult removeSale(int storeID, int saleID);
        public abstract ResultWithValue<int> addSaleCondition(int storeID, int saleID, SimplePredicate condition, SalePredicateCompositionType compositionType);
        public abstract ResultWithValue<int> composeSales(int storeID, int firstSaleID, int secondSaleID, SaleCompositionType typeOfComposition, SimplePredicate selectionRule);
        // * End Of Store Policies Management * //


        // * Item Reviews * //
        public abstract ResultWithValue<ConcurrentLinkedList<string>> itemReview(String review, int itemID, int storeID);
        // * End Of Item Reviews * //


        // * Store Managers Management * //
        public abstract RegularResult appointStoreManager(String managerName, int storeID); //the store manager will receive default permissions(4.9)
        public abstract RegularResult appointStoreOwner(String storeOwnerName, int storeID);
        public abstract RegularResult editManagerPermissions(String managerName, ConcurrentLinkedList<Permissions> permissions, int storeID);
        public abstract RegularResult removeStoreManager(String managerName, int storeID);
        public abstract RegularResult removeStoreOwner(String ownerName, int storeID);
        // * End Of Store Managers Management * //


        // * Get Informations * //
        public abstract ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>> getOfficialsInformation(int storeID);
        public abstract ConcurrentDictionary<int, PurchaseInvoice> getStorePurchaseHistory(int storeID); //all the purchases of the store that I manage/own
        public abstract ConcurrentDictionary<String, ConcurrentDictionary<int, PurchaseInvoice>> getUsersPurchaseHistory();
        public abstract ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>> getStoresPurchaseHistory();
        public abstract ConcurrentLinkedList<int> getUsersStores();
        // * End Of Get Informations * //
    }
}
