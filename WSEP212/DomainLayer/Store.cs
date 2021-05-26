using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.ServiceLayer.Result;
using Serialize.Linq;
using Serialize.Linq.Extensions;
using Serialize.Linq.Serializers;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace WSEP212.DomainLayer
{
    [Serializable]
    public class Store
    {
        private readonly object itemInStorageLock = new object();
        private readonly object purchaseItemsLock = new object();
        private readonly object addStoreSellerLock = new object();

        // static counter for the storeIDs of diffrent stores
        private static int storeCounter = 1;

        [JsonIgnore]
        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            SerializationBinder = new KnownTypesBinder
            {
                KnownTypes = new List<Type>
                {
                    typeof(SalePolicy.SalePolicy),
                    typeof(SalePolicyMock),
                    typeof(PurchasePolicy.PurchasePolicy),
                    typeof(PurchasePolicyMock),
                    typeof(ItemImmediatePurchase),
                    typeof(ItemSubmitOfferPurchase),
                    typeof(SimplePredicate),
                    typeof(AndPredicates),
                    typeof(OrPredicates),
                    typeof(ConditioningPredicate),
                    typeof(ConditionalSale),
                    typeof(DoubleSale),
                    typeof(MaxSale),
                    typeof(XorSale),
                    typeof(SimpleSale)
                }
            }
        };
        
        // A data structure associated with a item ID and its item
        public ConcurrentDictionary<int, Item> storage { get; set; }
        public string StorageAsJson
        {
            get => JsonConvert.SerializeObject(storage,settings);
            set => storage = JsonConvert.DeserializeObject<ConcurrentDictionary<int, Item>>(value);
        }
        
        [Key]
        public int storeID { get; set; }
        public String storeName { get; set; }
        public String storeAddress { get; set; }
        public bool activeStore { get; set; }
        public ConcurrentDictionary<int, PurchaseInvoice> purchasesHistory { get; set; }
        
        [NotMapped]
        public SalePolicyInterface salesPolicy { get; set; }
        
        public string SalesPolicyAsJson
        {
            get => JsonConvert.SerializeObject(salesPolicy,settings);
            set => setSalesJson(value);
        }

        [NotMapped]
        public PurchasePolicyInterface purchasePolicy { get; set; }
        public string PurchasePolicyAsJson
        {
            get => JsonConvert.SerializeObject(purchasePolicy,settings);
            set => setPurchaseJson(value);
        }

        public string PurchasesHistoryAsJson
        {
            get => JsonConvert.SerializeObject(purchasesHistory,settings);
            set => purchasesHistory = JsonConvert.DeserializeObject<ConcurrentDictionary<int, PurchaseInvoice>>(value);
        }
        // A data structure associated with a user name and seller permissions
        [NotMapped]
        public ConcurrentDictionary<String, SellerPermissions> storeSellersPermissions { get; set; }
        public string StoreSellersPermissionsAsJson
        {
            get => JsonConvert.SerializeObject(storeSellersPermissions,settings);
            set => storeSellersPermissions = JsonConvert.DeserializeObject<ConcurrentDictionary<String, SellerPermissions>>(value);
        }
        [NotMapped]
        [JsonIgnore]
        public DeliveryInterface deliverySystem { get; set; }

        public Store() {}
        public Store(String storeName, String storeAddress, SalePolicyInterface salesPolicy, PurchasePolicyInterface purchasePolicy, User storeFounder)
        {
            this.storage = new ConcurrentDictionary<int, Item>();
            this.storeID = StoreRepository.Instance.stores.Count + 1;
            storeCounter++;
            this.activeStore = true;
            this.salesPolicy = salesPolicy;
            SalesPolicyAsJson = JsonConvert.SerializeObject(salesPolicy,settings);
            this.purchasePolicy = purchasePolicy;
            PurchasePolicyAsJson = JsonConvert.SerializeObject(purchasePolicy,settings);
            this.purchasesHistory = new ConcurrentDictionary<int, PurchaseInvoice>();
            this.storeName = storeName;
            this.storeAddress = storeAddress;

            // create the founder seller permissions
            ConcurrentLinkedList<Permissions> founderPermissions = new ConcurrentLinkedList<Permissions>();
            founderPermissions.TryAdd(Permissions.AllPermissions);   // founder has all permisiions

            SellerPermissions storeFounderPermissions = SellerPermissions.getSellerPermissions(storeFounder.userName, this.storeID, "", founderPermissions);
            
            this.storeSellersPermissions = new ConcurrentDictionary<String, SellerPermissions>();
            this.storeSellersPermissions.TryAdd(storeFounder.userName, storeFounderPermissions);

            this.deliverySystem = DeliverySystemAdapter.Instance;
        }

        public void setPurchaseJson(string json)
        {
            if(purchasePolicy is PurchasePolicyMock)
                purchasePolicy = JsonConvert.DeserializeObject<PurchasePolicyMock>(json, settings);
            else
                purchasePolicy = JsonConvert.DeserializeObject<PurchasePolicy.PurchasePolicy>(json, settings);
        }

        public void setSalesJson(string json)
        {
            if(salesPolicy is SalePolicyMock)
                salesPolicy = JsonConvert.DeserializeObject<SalePolicyMock>(json, settings);
            else
                salesPolicy = JsonConvert.DeserializeObject<SalePolicy.SalePolicy>(json, settings);
        }
        
        public void addToDB()
        {
            SystemDBAccess.Instance.Stores.Add(this);
            SystemDBAccess.Instance.SaveChanges();
        }
        // Check if the item exist and available in the store
        public RegularResult isAvailableInStorage(int itemID, int quantity)
        {
            lock (itemInStorageLock)
            {
                if (storage.ContainsKey(itemID))   // checks if item exist in the store
                {
                    Item item = storage[itemID];
                    if (quantity <= item.quantity)   // checks if there is enough of the item in stock
                    {
                        return new Ok("The Item Is Available In The Store Storage");
                    }
                    return new Failure("The Item Is Not Available In The Store Storage At The Moment");
                }
                return new Failure("Item Not Exist In Storage");
            }
        }

        // Add new item to the store with his personal details
        // if the item has the same name, category and price we treat the product as an existing product
        // the quantity of an existing product will update
        // returns the itemID of the item, otherwise -1
        public ResultWithValue<int> addItemToStorage(int quantity, String itemName, String description, double price, ItemCategory category)
        {
            // checks that the item not already exist
            foreach (KeyValuePair<int, Item> pairItem in storage)
            {
                Item item = pairItem.Value;
                if (item.itemName.Equals(itemName) && item.category.Equals(category) && item.price == price)   // the item already exist
                {
                    if (item.setQuantity(quantity))
                    {
                        var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
                        if (result != null)
                        {
                            result.storage = storage;
                            if(!JToken.DeepEquals(result.StorageAsJson, this.StorageAsJson))
                                result.StorageAsJson = this.StorageAsJson;
                            SystemDBAccess.Instance.SaveChanges();
                            return new OkWithValue<int>("The Item Is Already In Storage, The Quantity Of The Item Has Been Updated Accordingly", pairItem.Key);
                        }
                    }
                    return new FailureWithValue<int>("One Or More Of The Item Details Are Invalid", -1);
                }
            }
            // item not exist - add new item to storage
            try {
                Item newItem = new Item(quantity, itemName, description, price, category);
                newItem.addToDB();
                this.storage.TryAdd(newItem.itemID, newItem);
                var res = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
                if (res != null)
                {
                    res.storage = storage;
                    if(!JToken.DeepEquals(res.StorageAsJson, this.StorageAsJson))
                        res.StorageAsJson = this.StorageAsJson;
                    SystemDBAccess.Instance.SaveChanges();
                }
                return new OkWithValue<int>("The Item Was Successfully Added To The Store Storage", newItem.itemID);
            } 
            catch (ArithmeticException)
            {
                return new FailureWithValue<int>("One Or More Of The Item Details Are Invalid", -1);
            }
        }

        // Remove item from the store
        // If the item has n quantity in the store, all the n will be deleted
        public RegularResult removeItemFromStorage(int itemID)
        {
            if (storage.ContainsKey(itemID))
            {
                var res = storage.TryRemove(itemID, out _);
                if (res)
                {
                    var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
                    if (result != null)
                    {
                        result.storage = storage;
                        if(!JToken.DeepEquals(result.StorageAsJson, this.StorageAsJson))
                            result.StorageAsJson = this.StorageAsJson;
                        SystemDBAccess.Instance.SaveChanges();
                        return new Ok("Item Was Successfully Removed From The Store's Storage");
                    }
                    return new Failure("Could not remove the item!");
                }
                
            }
            return new Failure("Item Not Exist In Storage");
        }

        // Change item quantity in the storage of the store
        // numOfItems can be positive or negative - which means remove or add to the storage
        public RegularResult changeItemQuantity(int itemID, int numOfItems)
        {
            if (storage.ContainsKey(itemID))
            {
                Item item = storage[itemID];
                if (item.changeQuantity(numOfItems))
                {
                    var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
                    if (result != null)
                    {
                        result.storage = storage;
                        if(!JToken.DeepEquals(result.StorageAsJson, this.StorageAsJson))
                            result.StorageAsJson = this.StorageAsJson;
                        SystemDBAccess.Instance.SaveChanges();
                        return new Ok("Item Was Successfully Removed From The Store's Storage");
                    }
                    return new Ok("The Item Quantity In The Store's Storage Has Been Successfully Changed");
                }
                return new Failure("Item quantity can't be negative");

            }
            return new Failure("Item Is Not Exist In Storage");
        }

        // returns the obj Item that corresponds to the requested ID number
        public ResultWithValue<Item> getItemById(int itemID)
        {
            if (storage.ContainsKey(itemID))
            {
                return new OkWithValue<Item>("Item Found In Storage Successfully", storage[itemID]);
            }
            return new FailureWithValue<Item>("Item Not Exist In Storage", null);
        }

        // edit the personal details of an item
        public RegularResult editItem(int itemID, String itemName, String description, double price, ItemCategory category, int quantity)
        {
            // checks that the item exists
            if (storage.ContainsKey(itemID))
            {
                Item item = storage[itemID];
                var res = item.editItem(itemName, description, price, category, quantity);
                var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
                if (res.getTag() && result != null)
                {
                    result.storage = storage;
                    if(!JToken.DeepEquals(result.StorageAsJson, this.StorageAsJson))
                        result.StorageAsJson = this.StorageAsJson;
                    SystemDBAccess.Instance.SaveChanges();
                }
                return res;
            }
            return new Failure("Item Not Exist In Storage");
        }

        public void supportPurchaseType(PurchaseType purchaseType)
        {
            purchasePolicy.supportPurchaseType(purchaseType);
        }

        public void unsupportPurchaseType(PurchaseType purchaseType)
        {
            purchasePolicy.unsupportPurchaseType(purchaseType);
        }

        public Boolean isStoreSupportPurchaseType(PurchaseType purchaseType)
        {
            return purchasePolicy.hasPurchaseTypeSupport(purchaseType);
        }

        // add a new purchase prediacte for the store
        public int addPurchasePredicate(LocalPredicate<PurchaseDetails> newPredicate, String predDescription)
        {
            var res = this.purchasePolicy.addPurchasePredicate(newPredicate, predDescription);
            var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
            if (result != null)
            {
                result.purchasePolicy = purchasePolicy;
                //result.PurchasePolicyAsJson = PurchasePolicyAsJson;
                SystemDBAccess.Instance.SaveChanges();
            }
            return 1;
        }

        // removes purchase prediacte from the store
        public RegularResult removePurchasePredicate(int predicateID)
        {
            var res = this.purchasePolicy.removePurchasePredicate(predicateID);
            var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
            if (res.getTag() && result != null)
            {
                result.purchasePolicy = purchasePolicy;
                result.PurchasePolicyAsJson = PurchasePolicyAsJson;
                SystemDBAccess.Instance.SaveChanges();
            }

            return res;
        }

        // compose two predicates by the type of predicate 
        public ResultWithValue<int> composePurchasePredicates(int firstPredicateID, int secondPredicateID, PurchasePredicateCompositionType typeOfComposition)
        {
            var res = this.purchasePolicy.composePurchasePredicates(firstPredicateID, secondPredicateID, typeOfComposition);
            var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
            if (res.getTag() && result != null)
            {
                result.purchasePolicy = purchasePolicy;
                result.PurchasePolicyAsJson = PurchasePolicyAsJson;
                SystemDBAccess.Instance.SaveChanges();
            }

            return res;
        }

        // returns the descriptions of all preds for presenting them to the user
        public ConcurrentDictionary<int, String> getPurchasePredicatesDescriptions()
        {
            return this.purchasePolicy.getPurchasePredicatesDescriptions();
        }

        // add new sale for the store sale policy
        public int addSale(int salePercentage, ApplySaleOn saleOn, String saleDescription)
        {
            var res = this.salesPolicy.addSale(salePercentage, saleOn, saleDescription);
            var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
            if (result != null)
            {
                result.salesPolicy = salesPolicy;
                result.SalesPolicyAsJson = SalesPolicyAsJson;
                SystemDBAccess.Instance.SaveChanges();
            }

            return res;
        }

        // remove sale from the store sale policy
        public RegularResult removeSale(int saleID)
        {
            var res = this.salesPolicy.removeSale(saleID);
            var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
            if (res.getTag() && result != null)
            {
                result.salesPolicy = salesPolicy;
                result.SalesPolicyAsJson = SalesPolicyAsJson;
                SystemDBAccess.Instance.SaveChanges();
            }

            return res;
        }

        // add conditional for getting the sale
        public ResultWithValue<int> addSaleCondition(int saleID, SimplePredicate condition, SalePredicateCompositionType compositionType)
        {
            var res = this.salesPolicy.addSaleCondition(saleID, condition, compositionType);
            var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
            if (res.getTag() && result != null)
            {
                result.salesPolicy = salesPolicy;
                result.SalesPolicyAsJson = SalesPolicyAsJson;
                SystemDBAccess.Instance.SaveChanges();
            }

            return res;
        }

        // compose two sales by the type of sale 
        public ResultWithValue<int> composeSales(int firstSaleID, int secondSaleID, SaleCompositionType typeOfComposition, SimplePredicate selectionRule)
        {
            var res = this.salesPolicy.composeSales(firstSaleID, secondSaleID, typeOfComposition, selectionRule);
            var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
            if (res.getTag() && result != null)
            {
                result.salesPolicy = salesPolicy;
                result.SalesPolicyAsJson = SalesPolicyAsJson;
                SystemDBAccess.Instance.SaveChanges();
            }

            return res;
        }

        // returns the descriptions of all sales for presenting them to the user
        public ConcurrentDictionary<int, String> getSalesDescriptions()
        {
            return this.salesPolicy.getSalesDescriptions();
        }

        // Apply the sales policy on a list of items
        // The function will return the price after discount for each of the items
        // <itemID, price>
        public ConcurrentDictionary<int, double> applySalesPolicy(ConcurrentDictionary<Item, double> itemsPrices, PurchaseDetails purchaseDetails)
        {
            return salesPolicy.pricesAfterSalePolicy(itemsPrices, purchaseDetails);
        }

        // Apply the purchase policy on a list of items and their type of purchase
        // The function will return if the purchase can be made
        public RegularResult applyPurchasePolicy(PurchaseDetails purchaseDetails)
        {
            return purchasePolicy.approveByPurchasePolicy(purchaseDetails);
        }

        // create purchase details of the relevent purchase
        private ResultWithValue<PurchaseDetails> createPurchaseDetails(User buyer, ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, double> itemsPurchasePrices)
        {
            ResultWithValue<ConcurrentDictionary<Item, int>> itemsQuantitiesRes = getObjItemsQuantities(items);
            if (!itemsQuantitiesRes.getTag())
            {
                return new FailureWithValue<PurchaseDetails>(itemsQuantitiesRes.getMessage(), null);
            }
            PurchaseDetails purchaseDetails = new PurchaseDetails(buyer, itemsQuantitiesRes.getValue(), itemsPurchasePrices);
            return new OkWithValue<PurchaseDetails>("Purchase Details Created Successfully", purchaseDetails);
        }

        // returns the final price of a purchase - doesnt purchase the items, just calculate the final price!
        public ResultWithValue<ConcurrentDictionary<int, double>> itemsAfterSalePrices(User buyer, ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, double> itemsPurchasePrices)
        {
            ResultWithValue<PurchaseDetails> purchaseDetailsRes = createPurchaseDetails(buyer, items, itemsPurchasePrices);
            ResultWithValue<ConcurrentDictionary<Item, double>> itemsPricesRes = getObjItemsPrices(items, itemsPurchasePrices);
            if (!purchaseDetailsRes.getTag() || !itemsPricesRes.getTag())
            {
                return new FailureWithValue<ConcurrentDictionary<int, double>>(purchaseDetailsRes.getMessage(), null);
            }

            // calculate the price of each item after sales
            ConcurrentDictionary<int, double> pricesAfterSaleRes = applySalesPolicy(itemsPricesRes.getValue(), purchaseDetailsRes.getValue());
            return new OkWithValue<ConcurrentDictionary<int, double>>("The Final Price Calculated For Each Of The Purchase Items", pricesAfterSaleRes);
        }

        // Purchase items from a store if all items are available in storage
        // The purchase of the items updates the quantity of the items in storage
        public RegularResult purchaseItemsIfAvailable(ConcurrentDictionary<int, int> items)
        {
            ConcurrentDictionary<int, int> updatedItems = new ConcurrentDictionary<int, int>();
            lock (purchaseItemsLock)
            {
                foreach (KeyValuePair<int, int> item in items)
                {
                    int itemID = item.Key;
                    int quantity = item.Value;
                    RegularResult itemAvailableRes = isAvailableInStorage(itemID, quantity);
                    if (itemAvailableRes.getTag())
                    {
                        RegularResult changeQuantityRes = changeItemQuantity(itemID, -1 * quantity);
                        if(changeQuantityRes.getTag())
                        {
                            updatedItems.TryAdd(itemID, quantity);
                        }
                        else
                        {
                            rollBackPurchase(updatedItems);   // if at least one of the items not available, the purchase is canceled
                            return new Failure("One Or More Of The " + changeQuantityRes.getMessage());
                        }
                    }
                    else
                    {
                        rollBackPurchase(updatedItems);   // if at least one of the items not available, the purchase is canceled
                        return new Failure("One Or More Of " + itemAvailableRes.getMessage());
                    }
                }
                return new Ok("All Items Are Available In The Store's Storage");
            }
        }

        // purchase the items if the purchase request suitable with the store's policy and the products are also in stock
        public ResultWithValue<ConcurrentDictionary<int, double>> purchaseItems(User buyer, ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, double> itemsPurchasePrices)
        {
            ResultWithValue<PurchaseDetails> purchaseDetailsRes = createPurchaseDetails(buyer, items, itemsPurchasePrices);
            if (!purchaseDetailsRes.getTag())
            {
                return new FailureWithValue<ConcurrentDictionary<int, double>>(purchaseDetailsRes.getMessage(), null);
            }

            // checks the purchase can be made by the purchase policy
            RegularResult purchasePolicyRes = applyPurchasePolicy(purchaseDetailsRes.getValue());
            if (purchasePolicyRes.getTag())
            {
                // checks that all the items available in the store storage
                RegularResult availableItemsRes = purchaseItemsIfAvailable(items);
                if (availableItemsRes.getTag())
                {
                    ResultWithValue<ConcurrentDictionary<Item, double>> itemsPricesRes = getObjItemsPrices(items, itemsPurchasePrices);
                    // calculate the price of each item after sales
                    ConcurrentDictionary<int, double> pricesAfterSale = applySalesPolicy(itemsPricesRes.getValue(), purchaseDetailsRes.getValue());
                    return new OkWithValue<ConcurrentDictionary<int, double>>("The Purchase Can Be Made, The Items Are Available In Storage And The Final Price Calculated For Each Item", pricesAfterSale);
                }
                return new FailureWithValue<ConcurrentDictionary<int, double>>(availableItemsRes.getMessage(), null);
            }
            return new FailureWithValue<ConcurrentDictionary<int, double>>(purchasePolicyRes.getMessage(), null);
        }

        private ResultWithValue<ConcurrentDictionary<Item, int>> getObjItemsQuantities(ConcurrentDictionary<int, int> items)
        {
            ConcurrentDictionary<Item, int> objItemsQuantities = new ConcurrentDictionary<Item, int>();
            Item objItem;
            int itemID;
            foreach (KeyValuePair<int, int> itemQuantityPair in items)
            {
                itemID = itemQuantityPair.Key;
                if (!storage.ContainsKey(itemID))
                {
                    return new FailureWithValue<ConcurrentDictionary<Item, int>>("One Or More Of The Items Not Exist In Storage", null);
                }
                objItem = storage[itemID];
                objItemsQuantities.TryAdd(objItem, itemQuantityPair.Value);
            }
            return new OkWithValue<ConcurrentDictionary<Item, int>>("All Items Exist In Storage", objItemsQuantities);
        }

        private ResultWithValue<ConcurrentDictionary<Item, double>> getObjItemsPrices(ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, double> itemsPurchasePrices)
        {
            ConcurrentDictionary<Item, double> objItemsPrices = new ConcurrentDictionary<Item, double>();
            Item objItem;
            foreach (int itemID in items.Keys)
            {
                if (!storage.ContainsKey(itemID))
                {
                    return new FailureWithValue<ConcurrentDictionary<Item, double>>("One Or More Of The Items Not Exist In Storage", null);
                }
                objItem = storage[itemID];
                objItemsPrices.TryAdd(objItem, itemsPurchasePrices[itemID]);
            }
            return new OkWithValue<ConcurrentDictionary<Item, double>>("All Items Exist In Storage", objItemsPrices);
        }

        // In case the purchase is canceled (payment is not made / system collapses) -
        // the items that were supposed to be purchased return to the store
        public void rollBackPurchase(ConcurrentDictionary<int, int> items)
        {
            foreach (KeyValuePair<int, int> item in items)
            {
                changeItemQuantity(item.Key, item.Value);
            }
        }

        // Deliver all the items to the address
        // returns transaction id if the delivery done successfully
        public int deliverItems(DeliveryParameters deliveryParameters)
        {
            if (this.deliverySystem == null)
            {
                this.deliverySystem = DeliverySystemAdapter.Instance;
            }
            return this.deliverySystem.deliverItems(deliveryParameters.sendToName, deliveryParameters.address, deliveryParameters.city,
                deliveryParameters.country, deliveryParameters.zip);
        }
        
        // cancel the delivery of the transaction id
        public bool cancelDelivery(int transactionID)
        {
            return this.deliverySystem.cancelDelivery(transactionID);
        }

        // Adds a new store seller for this store
        public RegularResult addNewStoreSeller(SellerPermissions sellerPermissions)
        {
            lock (addStoreSellerLock)
            {
                String sellerUserName = sellerPermissions.SellerName;
                if (!storeSellersPermissions.ContainsKey(sellerUserName))
                {
                    storeSellersPermissions.TryAdd(sellerUserName, sellerPermissions);
                    var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
                    if (result != null)
                    {
                        result.storeSellersPermissions = storeSellersPermissions;
                        if(!JToken.DeepEquals(result.StoreSellersPermissionsAsJson, this.StoreSellersPermissionsAsJson))
                            result.StoreSellersPermissionsAsJson = StoreSellersPermissionsAsJson;
                        SystemDBAccess.Instance.SaveChanges();
                    }
                    return new Ok("The New Store Seller Added To The Store Successfully");
                }
                return new Failure("The Store Seller Is Already Defined As A Seller In This Store");
            }
        }

        // Removes a store seller from this store
        public RegularResult removeStoreSeller(String sellerUserName)
        {
            if (storeSellersPermissions.ContainsKey(sellerUserName))
            {
                storeSellersPermissions.TryRemove(sellerUserName, out _);
                var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
                if (result != null)
                {
                    result.storeSellersPermissions = storeSellersPermissions;
                    if(!JToken.DeepEquals(result.StoreSellersPermissionsAsJson, this.StoreSellersPermissionsAsJson))
                        result.StoreSellersPermissionsAsJson = StoreSellersPermissionsAsJson;
                    SystemDBAccess.Instance.SaveChanges();
                }
                return new Ok("The Store Seller Removed From The Store Successfully");
            }
            return new Failure("The Store Seller Is Not Defined As A Seller In This Store");
        }

        // Removes a store seller from this store
        public RegularResult removeStoreOwner(String sellerUserName)
        {
            if (storeSellersPermissions.ContainsKey(sellerUserName))
            {
                storeSellersPermissions.TryRemove(sellerUserName, out _);
                foreach(KeyValuePair<string,SellerPermissions> val in storeSellersPermissions)
                {
                    if (val.Value.GrantorName.Equals(sellerUserName))
                    {
                        storeSellersPermissions.TryRemove(val.Key, out _);
                    }
                }
                return new Ok("The Store Seller Removed From The Store Successfully");
            }
            return new Failure("The Store Seller Is Not Defined As A Seller In This Store");
        }


        // checks if the user is seller in this store
        // if he is, returns his seller permissions in the store
        public ResultWithValue<SellerPermissions> getStoreSellerPermissions(String userName)
        {
            if(storeSellersPermissions.ContainsKey(userName))
            {
                return new OkWithValue<SellerPermissions>("The User Is Seller In This Store", storeSellersPermissions[userName]);
            }
            return new FailureWithValue<SellerPermissions>("The User Is Not Seller In This Store", null);
        }

        // Returns information about all the store official (store owner and manager) in the store
        public ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>> getStoreOfficialsInfo()
        {
            ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>> officialsInfo = new ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>();
            foreach (KeyValuePair<string, SellerPermissions> sellerEntry in storeSellersPermissions)
            {
                SellerPermissions seller = sellerEntry.Value;
                officialsInfo.TryAdd(seller.SellerName, seller.permissionsInStore);
            }
            return officialsInfo;
        }

        // Add purchase made by user in the store
        public void addPurchaseInvoice(PurchaseInvoice purchase)
        {
            var result = SystemDBAccess.Instance.Stores.SingleOrDefault(s => s.storeID == this.storeID);
            if (result != null)
            {
                purchasesHistory.TryAdd(purchase.purchaseInvoiceID, purchase);
                result.purchasesHistory = purchasesHistory;
                if(!JToken.DeepEquals(result.PurchasesHistoryAsJson, this.PurchasesHistoryAsJson))
                    result.PurchasesHistoryAsJson = PurchasesHistoryAsJson;
                SystemDBAccess.Instance.SaveChanges();
            }
        }
        
        // Remove purchase made by user in the store
        // done only if the rollback function was called
        public void removePurchaseInvoice(int purchaseInvoiceID)
        {
            purchasesHistory.TryRemove(purchaseInvoiceID, out _);
        }
    }
}
