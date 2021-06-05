using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class ShoppingBag
    {
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
                    typeof(SimpleSale),
                    typeof(SaleOnAllStore),
                    typeof(SaleOnCategory),
                    typeof(SaleOnItem)
                }
            }
        };
        
        public int StoreIDRef { get; set; }
        [Key]
        [ForeignKey("StoreIDRef")]
        public Store store { get; set; }
        public string bagOwner { get; set; }
        // A data structure associated with a item ID and its quantity - more effective when there will be a sales policy
        // There is no need for a structure that allows threads use, since only a single user can use these actions on his shopping bag
        [NotMapped]
        public ConcurrentDictionary<int, int> items { get; set; }
        public string DictionaryAsJson
        {
            get => JsonConvert.SerializeObject(items);
            set => items = JsonConvert.DeserializeObject<ConcurrentDictionary<int, int>>(value);
        }
        [NotMapped]
        public ConcurrentDictionary<int, ItemPurchaseType> itemsPurchaseTypes { get; set; }

        public string ItemsPurchaseTypesAsJson
        {
            get => JsonConvert.SerializeObject(itemsPurchaseTypes,settings);
            set => itemsPurchaseTypes = JsonConvert.DeserializeObject<ConcurrentDictionary<int, ItemPurchaseType>>(value,settings);
        }
        
        public ShoppingBag(Store store, string bagOwner)
        {
            this.store = store;
            this.bagOwner = bagOwner;
            this.items = new ConcurrentDictionary<int, int>();
            this.itemsPurchaseTypes = new ConcurrentDictionary<int, ItemPurchaseType>();
            this.StoreIDRef = store.storeID;
        }

        // return true if the shopping bag is empty
        public bool isEmpty()
        {
            return items.Count == 0;
        }

        // Adds item to the shopping bag if the item exist and available in the store
        // quantity is the number of the same item to add
        public RegularResult addItem(int itemID, int quantity, ItemPurchaseType purchaseType)
        {
            if (quantity > 0)
            {
                if(!items.ContainsKey(itemID))
                {
                    // check if the item quantity available in storage
                    RegularResult itemAvailableRes = store.isAvailableInStorage(itemID, quantity);
                    if (itemAvailableRes.getTag())
                    {
                        // checks that the store support this purchase type
                        if (store.isStoreSupportPurchaseType(purchaseType.getPurchaseType()))
                        {
                            items.TryAdd(itemID, quantity);
                            itemsPurchaseTypes.TryAdd(itemID, purchaseType);
                            return new Ok("The Item Was Successfully Added To The Shopping Bag");
                        }
                        return new Failure("Cannot Add Item Because The Store Doesn't Support Purchase Type");
                    }
                    return itemAvailableRes;
                }
                return new Failure("The Item Already Exist In The Shopping Bag");
            }
            return new Failure("Cannot Add A Item To The Shopping Bag With A Non-Positive Quantity");
        }

        // Removes the item from the shopping bag if it exists
        // If the item has n quantity in the basket, all the n will be deleted
        public RegularResult removeItem(int itemID)
        {
            if(items.ContainsKey(itemID) && itemsPurchaseTypes.ContainsKey(itemID))
            {
                items.TryRemove(itemID, out _);
                itemsPurchaseTypes.TryRemove(itemID, out _);
                return new Ok("The Item Was Successfully Removed From The Shopping Bag");
            }
            return new Failure("The Item Is Not Exist In The Shopping Bag");
        }

        // Changes the quantity of item in the bag if the item is in the bag and available in the store
        public RegularResult changeItemQuantity(int itemID, int updatedQuantity)
        {
            if(updatedQuantity == 0)
            {
                return removeItem(itemID);
            }
            else if(updatedQuantity > 0) 
            { 
                if (items.ContainsKey(itemID))  // check if item in the shopping bag
                {
                    RegularResult itemAvailableRes = store.isAvailableInStorage(itemID, updatedQuantity);
                    if (itemAvailableRes.getTag())   // check if item available in store
                    {
                        items[itemID] = updatedQuantity;
                        return new Ok("Item Quantity Was Successfully Changed In The Shopping Bag");
                    }
                    return itemAvailableRes;
                }
                return new Failure("The Item Is Not Exist In The Shopping Bag");
            }
            return new Failure("Cannot Change Item Quantity To A Non-Positive Number");
        }

        // Changes the item purchase type if the store support this purchase type
        public RegularResult changeItemPurchaseType(int itemID, ItemPurchaseType itemPurchaseType)
        {
            if(itemsPurchaseTypes.ContainsKey(itemID))
            {
                // checks that the store support this purchase type
                if(store.isStoreSupportPurchaseType(itemPurchaseType.getPurchaseType()))
                {
                    itemsPurchaseTypes[itemID] = itemPurchaseType;
                    return new Ok("Change The Item Purchase Type Successfully");
                }
                return new Failure("Cannot Change Item Purchase Type Because The Store Doesn't Support It");
            }
            return new Failure("The Item Not Exist In The Shopping Bag");
        }

        // submit new item price offer
        public RegularResult submitItemPriceOffer(int itemID, double offerItemPrice)
        {
            if(itemsPurchaseTypes.ContainsKey(itemID))
            {
                ItemPurchaseType purchaseType = itemsPurchaseTypes[itemID];
                RegularResult changingPriceRes = purchaseType.changeItemPrice(offerItemPrice);
                if(changingPriceRes.getTag())
                {
                    return purchaseType.changeItemPriceStatus(PriceStatus.Pending);
                }
                return changingPriceRes;
            }
            return new Failure("The Item Not Exist In The Shopping Bag");
        }

        // store manager send counter offer
        public RegularResult counterOffer(int itemID, double counterOffer)
        {
            if(itemsPurchaseTypes.ContainsKey(itemID))
            {
                ItemPurchaseType purchaseType = itemsPurchaseTypes[itemID];
                RegularResult changingPriceRes = purchaseType.changeItemPrice(counterOffer);
                if (changingPriceRes.getTag())
                {
                    return purchaseType.changeItemPriceStatus(PriceStatus.Approved);
                }
                return changingPriceRes;
            }
            return new Failure("The Item Not Exist In The Shopping Bag");
        }

        // update the status of the price - can be done only by store owners & managers
        public RegularResult itemPriceStatusDecision(int itemID, PriceStatus priceStatus)
        {
            if (itemsPurchaseTypes.ContainsKey(itemID))
            {
                return itemsPurchaseTypes[itemID].changeItemPriceStatus(priceStatus);
            }
            return new Failure("The Item Not Exist In The Shopping Bag");
        }
        
        // returns all items in bag with their quantities
        public ConcurrentDictionary<int, int> viewItemsQuantities()
        {
            return this.items;
        }

        // returns the prices of the items in the shopping bags, and their status
        public ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>> allItemsPricesAndStatus()
        {
            ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>> itemsPrices = new ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>();
            KeyValuePair<double, PriceStatus> itemStatus;
            foreach (KeyValuePair<int, ItemPurchaseType> itemPurchaseType in itemsPurchaseTypes)
            {
                itemStatus = new KeyValuePair<double, PriceStatus>(itemPurchaseType.Value.getCurrentPrice(), itemPurchaseType.Value.getPriceStatus());
                itemsPrices.TryAdd(itemPurchaseType.Key, itemStatus);
            }
            return itemsPrices;
        }
        
        // calculate the final price of each item in the bag, after sales
        // for all the items that not approved, not apply the sales on them
        public ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>> getItemsAfterSalePrices()
        {
            var user = UserRepository.Instance.findUserByUserName(bagOwner).getValue();
            ConcurrentDictionary<int, double> approvedItemsAndPrices = approvedItemsPrices();
            // build new shopping bag with only approved items
            ConcurrentDictionary<int, int> approvedItems = new ConcurrentDictionary<int, int>();
            foreach (int itemID in approvedItemsAndPrices.Keys)
            {
                approvedItems.TryAdd(itemID, items[itemID]);
            }
            // calculate the price after sale for approved items only
            ResultWithValue<ConcurrentDictionary<int, double>> approvedAfterSale = store.itemsAfterSalePrices(user, approvedItems, approvedItemsAndPrices);
            if(approvedAfterSale.getTag())
            {
                ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>> itemsPricesAndStatus = allItemsPricesAndStatus();
                int itemID;
                double prevPrice, salePrice;
                // update all approved items price after sale 
                foreach (KeyValuePair<int, double> itemPriceSale in approvedAfterSale.getValue())
                {
                    itemID = itemPriceSale.Key;
                    prevPrice = itemsPricesAndStatus[itemID].Key;
                    salePrice = itemPriceSale.Value;
                    if (salePrice < prevPrice)
                    {
                        KeyValuePair<double, PriceStatus> salePriceAndStatus = new KeyValuePair<double, PriceStatus>(salePrice, PriceStatus.Approved);
                        itemsPricesAndStatus[itemID] = salePriceAndStatus;
                    }
                }
                return itemsPricesAndStatus;
            }
            return null;
        }

        // returns all the approved items and their prices out of all the items in the bag
        private ConcurrentDictionary<int, double> approvedItemsPrices()
        {
            ConcurrentDictionary<int, double> itemsPrices = new ConcurrentDictionary<int, double>();
            foreach (KeyValuePair<int, ItemPurchaseType> itemPurchaseType in itemsPurchaseTypes)
            {
                if(itemPurchaseType.Value.getPriceStatus().Equals(PriceStatus.Approved))
                {
                    itemsPrices.TryAdd(itemPurchaseType.Key, itemPurchaseType.Value.getCurrentPrice());
                }
            }
            return itemsPrices;
        }

        // purchase all the items in the shopping bag
        // returns the total price after sales. if the purchase cannot be made returns -1
        public ResultWithValue<PurchaseInvoice> purchaseItemsInBag()
        {
            ConcurrentDictionary<int, double> itemsPrices = approvedItemsPrices();
            // if all the items were approved
            if(itemsPrices.Count == itemsPurchaseTypes.Count)
            {
                var user = UserRepository.Instance.findUserByUserName(bagOwner).getValue();
                ResultWithValue<ConcurrentDictionary<int, double>> purchaseItemsRes = store.purchaseItems(user, items, itemsPrices);
                if(purchaseItemsRes.getTag())
                {
                    // create purchase invoice
                    // if the purchase will be canceled, roll back will clean this invoices
                    PurchaseInvoice purchaseInvoice = new PurchaseInvoice(store.storeID, bagOwner, items, purchaseItemsRes.getValue(), DateTime.Now);
                    purchaseInvoice.addToDB();
                    store.addPurchaseInvoice(purchaseInvoice);
                    user.addPurchase(purchaseInvoice);
                    return new OkWithValue<PurchaseInvoice>(purchaseItemsRes.getMessage(), purchaseInvoice);
                }
                return new FailureWithValue<PurchaseInvoice>(purchaseItemsRes.getMessage(), null);
            }
            return new FailureWithValue<PurchaseInvoice>("One Or More Of The Items Price Weren't Approved Yet", null);
        }

        // roll back purchase - returns all the items in the bag to the store
        public void rollBackPurchase(int purchaseInvoiceID)
        {
            var user = UserRepository.Instance.findUserByUserName(bagOwner).getValue();
            store.rollBackPurchase(items);
            store.removePurchaseInvoice(purchaseInvoiceID);
            user.removePurchase(purchaseInvoiceID);
        }

        // Removes all the items in the shopping bag
        public void clearShoppingBag()
        {
            items.Clear();
            itemsPurchaseTypes.Clear();
        }

    }
}
