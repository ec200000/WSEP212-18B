﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212.ServiceLayer
{
    public interface ISystemController
    {
        public RegularResult register(String userName,  int userAge, String password); // USE CASE 2.3
        public RegularResult login(String userName, String password); // USE CASE 2.4
        
        public RegularResult continueAsGuest(String userName); // USE CASE 2.1
        public RegularResult logout(String userName); // USE CASE 3.1

        public ResultWithValue<NotificationDTO> addItemToShoppingCart(String userName, int storeID, int itemID, int quantity, Int32 purchaseType, double startPrice); // USE CASE 2.7
        public RegularResult removeItemFromShoppingCart(String userName, int storeID, int itemID); // USE CASE 2.8
        public RegularResult changeItemQuantityInShoppingCart(String userName, int storeID, int itemID, int updatedQuantity); // USE CASE 2.8
        public RegularResult changeItemPurchaseType(String userName, int storeID, int itemID, Int32 purchaseType, double startPrice); // USE CASE 2.8
        public ResultWithValue<NotificationDTO> submitPriceOffer(String userName, int storeID, int itemID, double offerItemPrice); // USE CASE 2.8
        public ResultWithValue<NotificationDTO> itemCounterOffer(String storeManager, String userName, int storeID, int itemID, double counterOffer); // USE CASE 2.8
        public ResultWithValue<NotificationDTO> confirmPriceStatus(String storeManager, String userToConfirm, int storeID, int itemID, Int32 priceStatus); // USE CASE 2.8
        public RegularResult addBidOffer(String userName, int storeID, int itemID, string buyer, double offerItemPrice); // USE CASE 2.8
        public RegularResult removeBidOffer(string userName, int storeID, int itemId, string buyer);
        public ResultWithValue<NotificationDTO> purchaseItems(string userName, DeliveryParametersDTO deliveryParameters, PaymentParametersDTO paymentParameters); // USE CASE 2.9
        public ResultWithValue<int> openStore(String userName, String storeName, String storeAddress,
            String purchasePolicy, String salesPolicy); // USE CASE 3.2

        public ConcurrentLinkedList<PurchaseType> getStorePurchaseTypes(String userName, int storeID); // USE CASE 4.2
        public RegularResult supportPurchaseType(String userName, int storeID, Int32 purchaseType); // USE CASE 4.2
        public RegularResult unsupportPurchaseType(String userName, int storeID, Int32 purchaseType); // USE CASE 4.2
        public ResultWithValue<int> addPurchasePredicate(String userName, int storeID, LocalPredicate<PurchaseDetails> newPredicate, String predDescription); // USE CASE 4.2
        public RegularResult removePurchasePredicate(String userName, int storeID, int predicateID); // USE CASE 4.2
        public ResultWithValue<int> composePurchasePredicates(String userName, int storeID, int firstPredicateID, 
            int secondPredicateID, Int32 typeOfComposition); // USE CASE 4.2
        public ResultWithValue<int> addSale(String userName, int storeID, int salePercentage, ApplySaleOn saleOn, String predDescription); // USE CASE 4.2
        public RegularResult removeSale(String userName, int storeID, int saleID); // USE CASE 4.2
        public ResultWithValue<int> addSaleCondition(String userName, int storeID, int saleID, SimplePredicate condition,
            Int32 compositionType); // USE CASE 4.2
        public ResultWithValue<int> composeSales(String userName, int storeID, int firstSaleID, int secondSaleID,
            Int32 typeOfComposition, SimplePredicate selectionRule); // USE CASE 4.2
        public ResultWithValue<ConcurrentDictionary<int, string>> getStorePredicatesDescription(int storeID); // USE CASE 4.2
        public ResultWithValue<ConcurrentDictionary<int, string>> getStoreSalesDescription(int storeID); // USE CASE 4.2
        public ResultWithValue<NotificationDTO> itemReview(String userName, String review, int itemID, int storeID); // USE CASE 3.3
        public ResultWithValue<int> addItemToStorage(String userName, int storeID, ItemDTO item); // USE CASE  4.1.1
        public RegularResult removeItemFromStorage(String userName, int storeID, int itemID); // USE CASE 4.1.2
        public RegularResult editItemDetails(String userName, int storeID, ItemDTO item); // USE CASE 4.1.3
        public RegularResult appointStoreManager(String userName, String managerName, int storeID); // USE CASE 4.5, 5.1
        public RegularResult appointStoreOwner(String userName, String storeOwnerName, int storeID); // USE CASE 4.3
        public RegularResult editManagerPermissions(String userName, String managerName,
            ConcurrentLinkedList<Int32> permissions, int storeID); // USE CASE 4.6, 5.1
        public ResultWithValue<NotificationDTO> removeStoreManager(String userName, String managerName, int storeID); // USE CASE 4.7
        public ResultWithValue<NotificationDTO> removeStoreOwner(String userName, String ownerName, int storeID); // USE CASE 4.4
        public ResultWithValue<ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>> getOfficialsInformation(
            String userName, int storeID); // USE CASE 4.9
        public ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> getStorePurchaseHistory(String userName, int storeID); // USE CASE 4.11
        public ResultWithValue<ConcurrentDictionary<String, ConcurrentDictionary<int, PurchaseInvoice>>> getUsersPurchaseHistory(String userName); // USE CASE 6.4.1
        public ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>>> getStoresPurchaseHistory(String userName); // USE CASE 6.4.2
        public ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> getUserPurchaseHistory(string userName); // USE CASE 3.7
        public ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> getItemsInStoresInformation(); // USE CASE 2.5
        public ConcurrentDictionary<Item, int> searchItems(String itemName, String keyWords, double minPrice, double maxPrice, Int32 category); // USE CASE 2.6
        public RegularResult loginAsSystemManager(string userName, string password);
        public ResultWithValue<ShoppingCart> viewShoppingCart(string userName);

        public ResultWithValue<ConcurrentDictionary<int, int>> bagItemsQuantities(String userName, int storeID);

        public ResultWithValue<ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>> offerItemsPricesAndStatus(string userName);
        public ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>> getItemsBeforeSalePrices(String userName);
        public ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>> getItemsAfterSalePrices(String userName);
        public ResultWithValue<ConcurrentLinkedList<int>> getUsersStores(String userName);
        public RegularResult isStoreOwner(string userName, int storeID);
        public RegularResult hasPermission(string userName, int storeID, Permissions permission);
        public string[] getAllSignedUpUsers();
        public Store getStoreByID(int storeID);
        public KeyValuePair<Item,int> getItemByID(int itemID);
    }
}
