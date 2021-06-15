﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.ExternalPaymentSystem;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212.DomainLayer
{
    public interface ISystemControllerFacade
    {
        public RegularResult register(String userName, int userAge, String password);
        
        public RegularResult registerAsSystemManager(String userName, int userAge, String password);
        public RegularResult login(String userName, String password);

        public RegularResult continueAsGuest(String userName);
        public RegularResult logout(String userName);

        public ResultWithValue<ConcurrentLinkedList<string>> addItemToShoppingCart(string userName, int storeID, int itemID, int quantity, PurchaseType purchaseType, double startPrice);
        public RegularResult removeItemFromShoppingCart(String userName, int storeID, int itemID);
        public RegularResult changeItemQuantityInShoppingCart(String userName, int storeID, int itemID, int updatedQuantity);
        public RegularResult changeItemPurchaseType(String userName, int storeID, int itemID, PurchaseType purchaseType, double startPrice);
        public ResultWithValue<ConcurrentLinkedList<string>> submitPriceOffer(String userName, int storeID, int itemID, double offerItemPrice);
        public ResultWithValue<string> itemCounterOffer(String storeManager, String userName, int storeID, int itemID, double counterOffer);
        public ResultWithValue<string> confirmPriceStatus(String storeManager, String userToConfirm, int storeID, int itemID, PriceStatus priceStatus);
        public RegularResult addBidOffer(string userName, int storeID, int itemId, string buyer, double offerItemPrice);
        public RegularResult removeBidOffer(string userName, int storeID, int itemId, string buyer);
        public ResultWithValue<ConcurrentLinkedList<string>> purchaseItems(string userName, DeliveryParameters deliveryParameters, PaymentParameters paymentParameters); 
        public ResultWithValue<int> openStore(String userName, String storeName, String storeAddress, PurchasePolicyInterface purchasePolicy, SalePolicyInterface salesPolicy);

        public RegularResult supportPurchaseType(String userName, int storeID, PurchaseType purchaseType);
        public RegularResult unsupportPurchaseType(String userName, int storeID, PurchaseType purchaseType);
        public ResultWithValue<int> addPurchasePredicate(String userName, int storeID, LocalPredicate<PurchaseDetails> newPredicate, String predDescription);
        public RegularResult removePurchasePredicate(String userName, int storeID, int predicateID);
        public ResultWithValue<int> composePurchasePredicates(String userName, int storeID, int firstPredicateID, int secondPredicateID, PurchasePredicateCompositionType typeOfComposition);
        public ResultWithValue<ConcurrentDictionary<int, string>> getStorePredicatesDescription(int storeID);
        public ResultWithValue<int> addSale(String userName, int storeID, int salePercentage, ApplySaleOn saleOn, String saleDescription);
        public RegularResult removeSale(String userName, int storeID, int saleID);
        public ResultWithValue<int> addSaleCondition(String userName, int storeID, int saleID, SimplePredicate condition, SalePredicateCompositionType compositionType);
        public ResultWithValue<int> composeSales(String userName, int storeID, int firstSaleID, int secondSaleID, SaleCompositionType typeOfComposition, SimplePredicate selectionRule);
        public ResultWithValue<ConcurrentDictionary<int, string>> getStoreSalesDescription(int storeID);

        public ResultWithValue<ConcurrentLinkedList<string>> itemReview(String userName, String review, int itemID, int storeID);
        public ResultWithValue<int> addItemToStorage(string userName, int storeID, int quantity, String itemName, String description, double price, ItemCategory category);
        public RegularResult removeItemFromStorage(String userName, int storeID, int itemID);
        public RegularResult editItemDetails(string userName, int storeID, int itemID, int quantity, String itemName, String description, double price, ItemCategory category);
        public RegularResult appointStoreManager(String userName, String managerName, int storeID); //the store manager will receive default permissions(4.9)
        public RegularResult appointStoreOwner(String userName, String storeOwnerName, int storeID);
        public RegularResult editManagerPermissions(String userName, String managerName, ConcurrentLinkedList<Permissions> permissions, int storeID);
        public RegularResult removeStoreManager(String userName, String managerName, int storeID);
        public RegularResult removeStoreOwner(String userName, String ownerName, int storeID);
        public ResultWithValue<ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>> getOfficialsInformation(String userName, int storeID);
        public ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> getStorePurchaseHistory(String userName, int storeID); //all the purchases of the store that I manage/own
        public ResultWithValue<ConcurrentDictionary<String, ConcurrentDictionary<int, PurchaseInvoice>>> getUsersPurchaseHistory(String userName);
        public ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>>> getStoresPurchaseHistory(String userName);
        
        public RegularResult loginAsSystemManager(string userName, string password);
        public ResultWithValue<ShoppingCart> viewShoppingCart(string userName);
        public ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> getUserPurchaseHistory(string userName);
        public ResultWithValue<ConcurrentDictionary<int, int>> bagItemsQuantities(String userName, int storeID);
        public ResultWithValue<ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>> offerItemsPricesAndStatus(string userName);
        public ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>> getItemsBeforeSalePrices(String userName);
        public ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>> getItemsAfterSalePrices(String userName);
        public ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> getItemsInStoresInformation();
        public ConcurrentDictionary<Item, int> searchItems(SearchItemsDTO searchItemsDTO);

        public ResultWithValue<ConcurrentLinkedList<int>> getUsersStores(String userName);
    }
}
