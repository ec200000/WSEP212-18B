using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class User
    {
        [Key]
        public String userName { get; set; }
        public int userAge { get; set; }
        [NotMapped]
        [JsonIgnore]
        public UserState state { get; set; }
        [NotMapped]
        public ShoppingCart shoppingCart { get; set; }
        [NotMapped]
        public ConcurrentBag<PurchaseInvoice> purchases
        {
            get;
            set;
        }
        [NotMapped]
        public ConcurrentLinkedList<SellerPermissions> sellerPermissions
        {
            get;
            set;
        }
        public bool isSystemManager { get; set; }

        public string PurchasesJson
        {
            get => JsonConvert.SerializeObject(purchases);
            set => purchases = JsonConvert.DeserializeObject<ConcurrentBag<PurchaseInvoice>>(value);
        }
        
        public string SellerPermissionsJson
        {
            get => JsonConvert.SerializeObject(sellerPermissions);
            set => sellerPermissions = JsonConvert.DeserializeObject<ConcurrentLinkedList<SellerPermissions>>(value);
        }

        public User()
        {
        }

        public User(String userName, int userAge = int.MinValue, bool isSystemManager = false)
        {
            this.userName = userName;
            this.userAge = userAge;
            this.shoppingCart = new ShoppingCart(userName);
            this.purchases = new ConcurrentBag<PurchaseInvoice>();
            this.sellerPermissions = new ConcurrentLinkedList<SellerPermissions>();
            this.state = new GuestBuyerState(this);
            this.isSystemManager = isSystemManager;
            
            SystemDBAccess.Instance.Users.Add(this);
            SystemDBAccess.Instance.SaveChanges();
        }

        public void changeState(UserState state)
        {
            this.state = state;
        }

        // params: string username, int age, string password
        // returns: bool
        public void register(Object list)
        {
            ThreadParameters param = (ThreadParameters)list; // getting the thread parameters object for the function
            User newUser = (User)param.parameters[0]; // getting the first argument
            String password = (String)param.parameters[1]; // getting the second argument
            object res;
            try
            {
                res = state.register(newUser, password);  // calling the function of the user's state
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException(); // instead of throwing an exception here
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: string username, string password
        // returns: bool
        public void login(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            String username = (String)param.parameters[0];
            String password = (String)param.parameters[1];
            object res;
            try
            {
                res = state.login(username, password);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }
        
        public void continueAsGuest(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            String username = (String)param.parameters[0];
            object res;
            try
            {
                res = state.continueAsGuest(username);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: string username
        // returns: bool
        public void logout(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            String username = (String)param.parameters[0];
            object res;
            try
            {
                res = state.logout(username);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: int storeID, int itemID
        // returns: bool
        public void addItemToShoppingCart(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            int itemID = (int)param.parameters[1];
            int quantity = (int)param.parameters[2];
            object res;
            try
            {
                res = state.addItemToShoppingCart(storeID, itemID, quantity);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: int storeID, int itemID
        // returns: bool
        public void removeItemFromShoppingCart(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            int itemID = (int)param.parameters[1];
            object res;
            try
            {
                res = state.removeItemFromShoppingCart(storeID, itemID);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }
        //edit item in shopping cart is equal to -> remove + add

        // params: ?
        // returns: bool
        public void purchaseItems(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            String address = (String)param.parameters[0];
            object res;
            try
            {
                res = state.purchaseItems(address);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: String storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy
        // returns: bool
        public void openStore(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            String storeName = (String)param.parameters[0];
            String storeAdress = (String)param.parameters[1];
            PurchasePolicy purchasePolicy = (PurchasePolicy)param.parameters[2];
            SalePolicy salesPolicy = (SalePolicy)param.parameters[3];
            object res;
            try
            {
                res = state.openStore(storeName, storeAdress, purchasePolicy, salesPolicy);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: String review, int itemID, int storeID
        // returns: bool
        public void itemReview(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            String review = (String)param.parameters[0];
            int itemID = (int)param.parameters[1];
            int storeID = (int)param.parameters[2];
            object res;
            try
            {
                res = state.itemReview(review, itemID, storeID);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: int storeID, Item item, int quantity
        // returns: bool
        public void addItemToStorage(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            int quantity = (int)param.parameters[1];
            String itemName = (String)param.parameters[2];
            String description = (String)param.parameters[3];
            double price = (double)param.parameters[4];
            String category = (String)param.parameters[5];
            object res;
            try
            {
                res = state.addItemToStorage(storeID, quantity, itemName, description, price, category);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: int storeID, Item item
        // returns: bool
        public void removeItemFromStorage(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            int itemID = (int)param.parameters[1];
            object res;
            try
            {
                res = state.removeItemFromStorage(storeID, itemID);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: int storeID, Item item
        // returns:  bool
        public void editItemDetails(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            int itemID = (int)param.parameters[1];
            int quantity = (int)param.parameters[2];
            String itemName = (String)param.parameters[3];
            String description = (String)param.parameters[4];
            double price = (double)param.parameters[5];
            String category = (String)param.parameters[6];
            object res;
            try
            {
                res = state.editItemDetails(storeID, itemID, quantity, itemName, description, price, category);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: String managerName, int storeID
        // returns: bool
        public void appointStoreManager(Object list) //the store manager will receive default permissions(4.9)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[1];
            String managerName = (String)param.parameters[0];
            object res;
            try
            {
                res = state.appointStoreManager(managerName, storeID);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: String storeOwnerName, int storeID
        // returns: 
        public void appointStoreOwner(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[1];
            String storeOwnerName = (String)param.parameters[0];
            object res;
            try
            {
                res = state.appointStoreOwner(storeOwnerName, storeID);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: String managerName, ConcurrentBag<Permissions> permissions, int storeID
        // returns: bool
        public void editManagerPermissions(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            String managerName = (String)param.parameters[0];
            ConcurrentLinkedList<Permissions> permissions = (ConcurrentLinkedList<Permissions>)param.parameters[1];
            int storeID = (int)param.parameters[2];
            object res;
            try
            {
                res = state.editManagerPermissions(managerName, permissions, storeID);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        // params: String managerName, int storeID
        // returns: bool
        public void removeStoreManager(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            String managerName = (String)param.parameters[0];
            int storeID = (int)param.parameters[1];
            object res;
            try
            {
                res = state.removeStoreManager(managerName, storeID);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }
        
        // params: String managerName, int storeID
        // returns: bool
        public void removeStoreOwner(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            String ownerName = (String)param.parameters[0];
            int storeID = (int)param.parameters[1];
            object res;
            try
            {
                res = state.removeStoreOwner(ownerName, storeID);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }

        // params: int storeID, Predicate pred
        // returns: int - the id of the new pred
        public void addPurchasePredicate(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            Predicate<PurchaseDetails> newPredicate = (Predicate<PurchaseDetails>)param.parameters[1];
            String predDescription = (String)param.parameters[2];
            object res;
            try
            {
                res = state.addPurchasePredicate(storeID, newPredicate, predDescription);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }

        // params: int storeID, int predID
        // returns: bool
        public void removePurchasePredicate(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            int predicateID = (int)param.parameters[1];
            object res;
            try
            {
                res = state.removePurchasePredicate(storeID, predicateID);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }

        // params: int storeID, int firstPredID, int secondPredID, PurchasePredicateCompositionType composion
        // returns: int - the id of the new composed pred
        public void composePurchasePredicates(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            int firstPredicateID = (int)param.parameters[1];
            int secondPredicateID = (int)param.parameters[2];
            PurchasePredicateCompositionType composionType = (PurchasePredicateCompositionType)param.parameters[3];
            object res;
            try
            {
                res = state.composePurchasePredicates(storeID, firstPredicateID, secondPredicateID, composionType);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }

        // params: int storeID, int salePercentage, ApplySaleOn saleOn
        // returns: int - the id of the new sale
        public void addSale(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            int salePercentage = (int)param.parameters[1];
            ApplySaleOn saleOn = (ApplySaleOn)param.parameters[2];
            String saleDescription = (String)param.parameters[3];
            object res;
            try
            {
                res = state.addSale(storeID, salePercentage, saleOn, saleDescription);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }

        // params: int storeID, int saleID
        // returns: bool
        public void removeSale(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            int saleID = (int)param.parameters[1];
            object res;
            try
            {
                res = state.removeSale(storeID, saleID);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }

        // params: int storeID, int saleID, Predicate<PurchaseDetails> condition, SalePredicateCompositionType compositionType
        // returns: int - the id of the new composed sale
        public void addSaleCondition(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            int saleID = (int)param.parameters[1];
            SimplePredicate condition = (SimplePredicate)param.parameters[2];
            SalePredicateCompositionType compositionType = (SalePredicateCompositionType)param.parameters[3];
            object res;
            try
            {
                res = state.addSaleCondition(storeID, saleID, condition, compositionType);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }

        // params: int storeID, int firstSaleID, int secondSaleID, SaleCompositionType composion, Predicate<PurchaseDetails> selectionRule
        // returns: int - the id of the new composed sale
        public void composeSales(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            int firstSaleID = (int)param.parameters[1];
            int secondSaleID = (int)param.parameters[2];
            SaleCompositionType typeOfComposition = (SaleCompositionType)param.parameters[3];
            SimplePredicate selectionRule = (SimplePredicate)param.parameters[4];
            object res;
            try
            {
                res = state.composeSales(storeID, firstSaleID, secondSaleID, typeOfComposition, selectionRule);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }

        // params: int storeID
        // returns: ConcurrentDictionary<String, ConcurrentBag<Permissions>>
        public void getOfficialsInformation(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            object res;
            try
            {
                res = state.getOfficialsInformation(storeID);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }

        // params: int storeID
        // returns: ConcurrentBag<PurchaseInfo>
        public void getStorePurchaseHistory(Object list) //all the purchases of the store that I manage/own
        {
            ThreadParameters param = (ThreadParameters)list;
            int storeID = (int)param.parameters[0];
            object res;
            try
            {
                res = state.getStorePurchaseHistory(storeID);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }

        // params: NONE
        // returns: ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>>
        public void getUsersPurchaseHistory(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            object res;
            try
            {
                res = state.getUsersPurchaseHistory();
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }

        // params: NONE
        // returns: ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>>
        public void getStoresPurchaseHistory(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            object res;
            try
            {
                res = state.getStoresPurchaseHistory();
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set();
        }
        
        // params: string username, string password
        // returns: bool
        public void loginAsSystemManager(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            String username = (String)param.parameters[0];
            String password = (String)param.parameters[1];
            object res;
            try
            {
                res = state.loginAsSystemManager(username, password);
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

        public void addPurchase(PurchaseInvoice info)
        {
            var result = SystemDBAccess.Instance.Users.SingleOrDefault(u => u.userName == this.userName);
            if (result != null)
            {
                this.purchases.Add(info);
                result.purchases = purchases;
                if(!JToken.DeepEquals(result.PurchasesJson, this.PurchasesJson))
                    result.PurchasesJson = this.PurchasesJson;
                SystemDBAccess.Instance.SaveChanges();
            }
        }
        
        public bool addSellerPermissions(SellerPermissions permissions)
        {
            var res = false;
            var result = SystemDBAccess.Instance.Users.SingleOrDefault(u => u.userName == this.userName);
            if (result != null)
            {
                result.SellerPermissionsJson = this.SellerPermissionsJson;
                res = this.sellerPermissions.TryAdd(permissions);
                result.sellerPermissions = this.sellerPermissions;
                SystemDBAccess.Instance.SaveChanges();
            }
            return res;
        }
        
        public void getUsersStores(Object list)
        {
            ThreadParameters param = (ThreadParameters)list;
            object res;
            try
            {
                res = state.getUsersStores();
            }
            catch (NotImplementedException)
            {
                res = new NotImplementedException();
            }
            param.result = res;
            param.eventWaitHandle.Set(); // signal we're done
        }

    }
}
