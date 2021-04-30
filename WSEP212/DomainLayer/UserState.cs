using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
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
        public abstract RegularResult logout(String userName);
        // * End Of User Management In The System * //


        // * Purchase Items Functions *//
        public RegularResult addItemToShoppingCart(int storeID, int itemID, int quantity)
        {
            return this.user.shoppingCart.addItemToShoppingBag(storeID, itemID, quantity);
        }
        public RegularResult removeItemFromShoppingCart(int storeID, int itemID)
        {
            return this.user.shoppingCart.removeItemFromShoppingBag(storeID, itemID);
        }
        public RegularResult purchaseItems(string address)
        {
            return HandlePurchases.Instance.purchaseItems(this.user, address); // handling the purchase procedure
        }
        // * End Of Purchase Items Functions *//
        

        // * Store Storage Management * //
        public abstract ResultWithValue<int> openStore(String storeName, String storeAddress, PurchasePolicy purchasePolicy, SalePolicy salesPolicy);
        public abstract ResultWithValue<int> addItemToStorage(int storeID, int quantity, String itemName, String description, double price, String category);
        public abstract RegularResult removeItemFromStorage(int storeID, int itemID);
        public abstract RegularResult editItemDetails(int storeID, int itemID, int quantity, String itemName, String description, double price, String category);
        // * End Of Store Storage Management * //


        // * Store Policies Management * //
        public abstract ResultWithValue<int> addPurchasePredicate(int storeID, Predicate<PurchaseDetails> newPredicate);
        public abstract RegularResult removePurchasePredicate(int storeID, int predicateID);
        public abstract ResultWithValue<int> composePurchasePredicates(int storeID, int firstPredicateID, int secondPredicateID, PurchasePredicateCompositionType typeOfComposition);
        public abstract ResultWithValue<int> addSale(int storeID, int salePercentage, ApplySaleOn saleOn);
        public abstract RegularResult removeSale(int storeID, int saleID);
        public abstract ResultWithValue<int> addSaleCondition(int storeID, int saleID, Predicate<PurchaseDetails> condition, SalePredicateCompositionType compositionType);
        public abstract ResultWithValue<int> composeSales(int storeID, int firstSaleID, int secondSaleID, SaleCompositionType typeOfComposition, Predicate<PurchaseDetails> selectionRule);
        // * End Of Store Policies Management * //


        // * Item Reviews * //
        public abstract RegularResult itemReview(String review, int itemID, int storeID);
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
        public abstract ConcurrentBag<PurchaseInvoice> getStorePurchaseHistory(int storeID); //all the purchases of the store that I manage/own
        public abstract ConcurrentDictionary<String, ConcurrentBag<PurchaseInvoice>> getUsersPurchaseHistory();
        public abstract ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>> getStoresPurchaseHistory();
        // * End Of Get Informations * //
    }
}
