using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Models;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;
using System.Web;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        const string SessionName = "_Name";  
        const string SessionAge = "_Age";  
        const string SessionLogin = "_Login";  
        const string SessionStoreID = "_StoreID";
        const string SessionItemID = "_ItemID";
        const string SessionPurchaseHistory = "_History";

        private User user;
        
        public IActionResult Index()  
        {
            HttpContext.Session.SetString(SessionName, "");
            HttpContext.Session.SetInt32(SessionLogin, 0);
            return View();  
        }  

        public IActionResult ItemReview(PurchaseModel model)
        {
            string info = model.itemInfo;
            string[] subInfo = info.Split(",");
            HttpContext.Session.SetInt32(SessionItemID, int.Parse(subInfo[1].Substring(10)));
            HttpContext.Session.SetInt32(SessionStoreID, int.Parse(subInfo[3].Substring(14)));
            return View();
        }

        public IActionResult PurchaseHistory()
        {
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ConcurrentBag<PurchaseInvoice>> res = systemController.getUserPurchaseHistory(HttpContext.Session.GetString(SessionName));
            if (res.getTag())
            {
                string value = "";
                foreach (PurchaseInvoice inv in res.getValue())
                {
                    value += inv.ToString() + ";";
                }
                if(value!="")
                    value = value.Substring(0, value.Length - 1);
                HttpContext.Session.SetString(SessionPurchaseHistory, value);
                return View();
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("Index");
            }
        }

        public IActionResult ShoppingCart(ShoppingCartModel model)
        {
            SystemController systemController = SystemController.Instance;
            WSEP212.DomainLayer.ShoppingCart res = systemController.viewShoppingCart(HttpContext.Session.GetString(SessionName)).getValue();
            ShoppingCartItems(res);
            return View();
        }

        private void ShoppingCartItems(WSEP212.DomainLayer.ShoppingCart shoppingCart)
        {
            if (shoppingCart == null)
            {
                HttpContext.Session.SetObject("shoppingCart", new string[] {""});
                RedirectToAction("Login");
                return;
            }

            ConcurrentDictionary<int, ShoppingBag> shoppingBagss = shoppingCart.shoppingBags;
            LinkedList<string> storesAndItems = new LinkedList<string>();
            foreach (ShoppingBag shopBag in shoppingBagss.Values)
            {
                int storeid = shopBag.store.storeID;
                string storeAndItem = "StoreID:" + storeid;
                foreach (int itemID in shopBag.items.Keys)
                {
                    string item = " ItemsID:" + itemID;
                    //storeAndItem = storeAndItem +;
                    storesAndItems.AddLast(storeAndItem + item);
                }
            }

            string[] strs = storesAndItems.ToArray();
            HttpContext.Session.SetObject("shoppingCart", strs);
        }

        public IActionResult Privacy()
        {
            ViewBag.Name = HttpContext.Session.GetString(SessionName);  
            ViewBag.Age = HttpContext.Session.GetInt32(SessionAge);  
            ViewBag.StoreID = HttpContext.Session.GetInt32(SessionStoreID); 
            ViewData["Message"] = "Asp.Net Core !!!.";  
            
            return View();
        }
        
        public IActionResult Login()
        {
            ViewBag.Name = HttpContext.Session.GetString(SessionName);  
            ViewBag.Age = HttpContext.Session.GetInt32(SessionAge);  
            ViewData["Message"] = "Asp.Net Core !!!.";  
            
            return View();
        }
        
        public IActionResult Logout()
        {
            return View();
        }
        
        public IActionResult OpenStore()
        {
            return View();
        }
        
        public IActionResult StoreActions()
        {
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ConcurrentLinkedList<int>> res = systemController.getUsersStores(HttpContext.Session.GetString(SessionName));
            int[] stores = listToArray(res.getValue());
            HttpContext.Session.SetObject("stores", stores);
            return View();
        }
        
        public IActionResult GetStoreInformation(StoreModel model)
        {
            HttpContext.Session.SetInt32(SessionStoreID, model.storeID);
            return View();
        }
        
        public IActionResult ItemActions(StoreModel model)
        {
            SystemController systemController = SystemController.Instance;
            ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> res = systemController.getItemsInStoresInformation();
            HttpContext.Session.SetInt32(SessionStoreID, model.storeID);
            checkStoresItems(model.storeID, res);
            return View();
        }
        
        public IActionResult EditItemDetails(ItemModel model)
        {
            KeyValuePair<Item, int> pair = StoreRepository.Instance.getItemByID(model.itemID);
            //HttpContext.Session.SetInt32(SessionStoreID, pair.Value);
            HttpContext.Session.SetInt32(SessionItemID, pair.Key.itemID);
            Item item = pair.Key;
            model.storeID = pair.Value;
            model.itemName = item.itemName;
            model.category = item.category;
            model.description = item.description;
            model.quantity = item.quantity;
            model.price = item.price;
            model.reviews = item.reviews;
            return View(model);
        }
        
        public IActionResult SearchItems(SearchModel model)
        {
            SystemController systemController = SystemController.Instance;
            if (!model.flag)
            {
                ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> res = systemController.getItemsInStoresInformation();
                allItemStrings(res);
            }
            model.items = HttpContext.Session.GetObject<string[]>("allitemstrings");
            return View(model);
        }
        
        public IActionResult ShowReviews(ShowReviewsModel model)
        {
            model = new ShowReviewsModel();
            int itemID = (int)HttpContext.Session.GetInt32(SessionItemID);
            if (itemID != 0)
            {
                KeyValuePair<Item, int> pair = StoreRepository.Instance.getItemByID(itemID);
                model.reviews = pair.Key.reviews;
                model.reviewsStrings = reviewsToString(pair.Key.reviews);
            }
            else
                model.reviewsStrings = "";
            return View(model);
        }

        private string reviewsToString(ConcurrentDictionary<String, ItemReview> reviews)
        {
            string reviewsStr = "";
            foreach (ItemReview review in reviews.Values)
            {
                reviewsStr += review + "\n";
            }
            return reviewsStr;
        }
        
        public IActionResult TryShowReviews(SearchModel model)
        {
            if (model != null)
            {
                if (model.itemChosen != null)
                {
                    string[] authorsList = model.itemChosen.Split(": ");
                    int itemID = int.Parse(authorsList[authorsList.Length - 1]);
                    HttpContext.Session.SetInt32(SessionItemID, itemID);
                }
                else
                    HttpContext.Session.SetInt32(SessionItemID, 0);
            }
            else
                HttpContext.Session.SetInt32(SessionItemID, 0);
            return RedirectToAction("ShowReviews");
        }
        
        public IActionResult Subscribe(UserModel model)
        {
            SystemController systemController = SystemController.Instance;
            RegularResult res = systemController.register(model.UserName, model.Age, model.Password);
            if (res.getTag())
            {
                HttpContext.Session.SetString(SessionName, model.UserName);  
                HttpContext.Session.SetInt32(SessionAge, model.Age);
                HttpContext.Session.SetInt32(SessionLogin, 0);
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("Index");
            }
        }
        
        public IActionResult ContinueAsGuest(UserModel model)
        {
            SystemController systemController = SystemController.Instance;
            RegularResult res = systemController.continueAsGuest(model.UserName);
            if (res.getTag())
            {
                HttpContext.Session.SetString(SessionName, model.UserName);
                HttpContext.Session.SetInt32(SessionLogin, 3);
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("Index");
            }
        }
        
        public IActionResult TryEditDetails(ItemModel model)
        {
            SystemController systemController = SystemController.Instance;
            //KeyValuePair<Item, int> pair = StoreRepository.Instance.getItemByID(model.itemID);
            ItemDTO itemDto = new ItemDTO((int)HttpContext.Session.GetInt32(SessionStoreID),
                model.quantity,
                model.itemName,
                model.description,
                model.reviews,
                model.price,
                model.category);
            itemDto.itemID = (int) HttpContext.Session.GetInt32(SessionItemID);
            RegularResult res = systemController.editItemDetails(HttpContext.Session.GetString(SessionName), (int)HttpContext.Session.GetInt32(SessionStoreID), itemDto);
            if (res.getTag())
            {
                return RedirectToAction("StoreActions");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return RedirectToAction("Privacy");
            }
        }
        
        public IActionResult TrySearchItems(SearchModel model)
        {
            SystemController systemController = SystemController.Instance;
            if (model.itemName == null) model.itemName = "";
            if (model.keyWords == null) model.keyWords = "";
            if (model.minPrice == 0) model.minPrice = int.MinValue;
            if (model.maxPrice == 0) model.maxPrice = int.MaxValue;
            if (model.category == null) model.category = "";
            ConcurrentDictionary<Item,int> res = systemController.searchItems(model.itemName, model.keyWords,model.minPrice, model.maxPrice, model.category);
            if (res != null)
            {
                itemsFromSearch(res);
                model.flag = true;
            }
            else
            {
                HttpContext.Session.SetObject("allitemstrings", new string[] {""});
                model.flag = true;
            }
            //model.items = HttpContext.Session.GetObject<string[]>("allitemstrings");
            return RedirectToAction("SearchItems", model);
        }

        public IActionResult TryReviewItem(ReviewModel model)
        {
            SystemController systemController = SystemController.Instance;
            RegularResult res = systemController.itemReview(HttpContext.Session.GetString(SessionName), model.review, (int)HttpContext.Session.GetInt32(SessionItemID), (int)HttpContext.Session.GetInt32(SessionStoreID));
            if (res.getTag())
            {
                return RedirectToAction("Privacy");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("Index");
            }
        }

        public IActionResult TryLogin(UserModel model)
        {
            SystemController systemController = SystemController.Instance;
            RegularResult res = systemController.login(model.UserName, model.Password);
            if (res.getTag())
            {
                this.user = UserRepository.Instance.findUserByUserName(model.UserName).getValue();
                HttpContext.Session.SetString(SessionName, model.UserName);
                HttpContext.Session.SetInt32(SessionLogin, 1);
                return RedirectToAction("Privacy");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("Index");
            }
        }

        public IActionResult LoginAsSystemManager(UserModel model)
        {
            SystemController systemController = SystemController.Instance;
            RegularResult res = systemController.loginAsSystemManager(model.UserName, model.Password);
            if (res.getTag())
            {
                this.user = UserRepository.Instance.findUserByUserName(model.UserName).getValue();
                HttpContext.Session.SetString(SessionName, model.UserName);
                HttpContext.Session.SetInt32(SessionLogin, 2);
                return RedirectToAction("Privacy");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("Login");
            }
        }
        
        public IActionResult TryLogout(UserModel model)
        {
            if (HttpContext.Session.GetInt32(SessionLogin) != 3)
            {
                SystemController systemController = SystemController.Instance;
                RegularResult res = systemController.logout(HttpContext.Session.GetString(SessionName));
                if (res.getTag())
                {
                    HttpContext.Session.SetString(SessionName, "");
                    HttpContext.Session.SetInt32(SessionLogin, 0);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Alert = res.getMessage();
                    return View("Logout");
                }
            }
            else
            {
                HttpContext.Session.SetString(SessionName, "");
                HttpContext.Session.SetInt32(SessionLogin, 0);
                return RedirectToAction("Index");
            }
            
        }
        
        public IActionResult TryOpenStore(StoreModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            ResultWithValue<int> res = systemController.openStore(userName, model.storeName, model.storeAddress, model.purchasePolicy, model.salesPolicy);
            if (res.getTag())
            {
                HttpContext.Session.SetInt32(SessionStoreID, res.getValue());
                return RedirectToAction("Privacy");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("OpenStore");
            }
        }

        private int[] listToArray(ConcurrentLinkedList<int> lst)
        {
            int[] arr = new int[lst.size];
            int i = 0;
            Node<int> node = lst.First; // going over the user's permissions to check if he is a store manager or owner
            while(node.Next != null)
            {
                arr[i] = node.Value;
                node = node.Next;
                i++;
            }
            return arr;
        }

        public IActionResult TryAddItem(ItemModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            ItemDTO item = new ItemDTO((int) storeID, model.quantity, model.itemName, model.description,
                new ConcurrentDictionary<string, ItemReview>(), model.price, model.category);
            ResultWithValue<int> res = systemController.addItemToStorage(userName, (int)storeID, item);
            if (res.getTag())
            {
                HttpContext.Session.SetInt32(SessionItemID, res.getValue());
                return RedirectToAction("Privacy");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("OpenStore");
            }
        }
        
        public IActionResult TryRemoveItem(ItemModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            RegularResult res = systemController.removeItemFromStorage(userName, (int)storeID, model.itemID);
            if (res.getTag())
            {
                return RedirectToAction("Privacy");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("ItemActions");
            }
        }
        
        public IActionResult TryAddItemToShoppingCart(SearchModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            string[] authorsList = model.itemChosen.Split(": ");
            string[] store = authorsList[1].Split(" ");
            int storeID = int.Parse(store[0]);
            int itemID = int.Parse(authorsList[authorsList.Length - 1]);
            RegularResult res = systemController.addItemToShoppingCart(userName, storeID, itemID, model.quantity);
            if (res.getTag())
            {
                //HttpContext.Session.SetInt32(SessionItemID, res.getValue());
                return RedirectToAction("SearchItems");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("OpenStore");
            }
        }

        public IActionResult TryShowPurchaseHistory(PurchaseModel model)
        {
            return RedirectToAction("ItemReview", model);
        }

        public IActionResult TryShowShoppingCart()
        {
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ShoppingCart> res = systemController.viewShoppingCart(HttpContext.Session.GetString(SessionName));
            if (res.getTag())
            {
                HttpContext.Session.SetObject("shopping_cart", res.getValue());
                return null;
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("Index");
            }
        }

        private int[] itemListToArray(ConcurrentLinkedList<Item> lst)
        {
            int[] arr = new int[lst.size];
            int i = 0;
            Node<Item> node = lst.First; 
            while(node.Next != null)
            {
                arr[i] = node.Value.itemID;
                node = node.Next;
                i++;
            }
            return arr;
        }
        
        private void allStoresItems(ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> dict)
        {
            int[] items;
            foreach (Store store in dict.Keys)
            {
                items = itemListToArray(dict[store]);
                string itms = "searchitems" + store.storeID;
                HttpContext.Session.SetObject(itms, items);
            }
        }
        
        private void getAllItems(ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> dict)
        {
            int[] items;
            LinkedList<int> itms = new LinkedList<int>();
            allStoresItems(dict);
            foreach (Store store in dict.Keys)
            {
                string str = "searchitems" + store.storeID;
                items = HttpContext.Session.GetObject<int[]>(str);
                if (items != null)
                {
                    LinkedList<int> newlist = new LinkedList<int>(items);
                    itms.AppendRange(newlist);
                }
            }
            int[] searches = itms.ToArray();
            HttpContext.Session.SetObject("allitems", searches);
            //HttpContext.Session.SetObject("allitems!", dict);
        }
        
        private void allStoresItemStrings(ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> dict)
        {
            string[] items;
            foreach (Store store in dict.Keys)
            {
                items = itemListToStringArray(dict[store], store.storeID);
                string itms = "searchitemsstr" + store.storeID;
                HttpContext.Session.SetObject(itms, items);
            }
        }
        
        private void allItemStrings(ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> dict)
        {
            string[] items;
            LinkedList<string> itms = new LinkedList<string>();
            allStoresItemStrings(dict);
            foreach (Store store in dict.Keys)
            {
                string str = "searchitemsstr" + store.storeID;
                items = HttpContext.Session.GetObject<string[]>(str);
                if (items != null)
                {
                    LinkedList<string> newlist = new LinkedList<string>(items);
                    itms.AppendRange(newlist);
                }
            }
            string[] searches = itms.ToArray();
            HttpContext.Session.SetObject("allitemstrings", searches);
        }
        
        private void itemsFromSearch(ConcurrentDictionary<Item,int> dict)
        {
            LinkedList<string> itms = new LinkedList<string>();
            foreach (Item item in dict.Keys)
            {
                string str = "StoreID: "+dict[item]+" "+ item.ToString();
                itms.AddLast(str);
            }
            string[] searches = itms.ToArray();
            HttpContext.Session.SetObject("allitemstrings", searches);
            //HttpContext.Session.SetObject("allitems!", dict);
        }
        
        private void checkStoresItems(int lst, ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> dict)
        {
            foreach (Store store in dict.Keys)
            {
                if (lst == store.storeID)
                {
                    int[] items = itemListToArray(dict[store]);
                    string itms = "items" + lst;
                    HttpContext.Session.SetObject(itms, items);
                }
            }
        }
        
        private string[] itemListToStringArray(ConcurrentLinkedList<Item> lst, int storeID)
        {
            string[] arr = new string[lst.size];
            int i = 0;
            Node<Item> node = lst.First; // going over the user's permissions to check if he is a store manager or owner
            while(node.Next != null)
            {
                arr[i] = "StoreID: "+storeID+" "+ node.Value.ToString();
                node = node.Next;
                i++;
            }
            return arr;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
        public IActionResult TryRemoveItemFromShoppingCart(ShoppingCartModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            string itemID = model.itemID;
            string[] strs = itemID.Split(":");
            int item = int.Parse(strs[strs.Length - 1]);
            RegularResult res = systemController.removeItemFromShoppingCart(userName, (int)storeID, item);
            if (res.getTag())
            {
                return RedirectToAction("ShoppingCart");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("ShoppingCart");
            }
        }
        public IActionResult TrypurchaseItems(ShoppingCartModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            RegularResult res = systemController.purchaseItems(userName, model.Address);
            if (res.getTag())
            {
                return RedirectToAction("Privacy");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("ShoppingCart");
            }
        }
        
        public IActionResult UsersPurchaseHistory()
        {
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ConcurrentDictionary<string,ConcurrentBag<PurchaseInvoice>>> res = systemController.getUsersPurchaseHistory(HttpContext.Session.GetString(SessionName));
            if (res.getTag())
            {
                string value = "";
                foreach (KeyValuePair<string,ConcurrentBag<PurchaseInvoice>> invs in res.getValue())
                {
                    foreach (PurchaseInvoice inv in invs.Value)
                    {
                        value += invs.Key+" bought  "+inv.ToString() + "\n" + ";";
                    }
                }
                if(value!="")
                    value = value.Substring(0, value.Length - 1);
                HttpContext.Session.SetString("users_history", value);
                return View();
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("Index");
            }
        }
        
        public IActionResult StoresPurchaseHistory()
        {
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ConcurrentDictionary<int,ConcurrentBag<PurchaseInvoice>>> res = systemController.getStoresPurchaseHistory(HttpContext.Session.GetString(SessionName));
            if (res.getTag())
            {
                string value = "";
                foreach (KeyValuePair<int,ConcurrentBag<PurchaseInvoice>> invs in res.getValue())
                {
                    foreach (PurchaseInvoice inv in invs.Value)
                    {
                        value += inv.ToString() + "\n" + ";";
                    }
                }
                if(value!="")
                    value = value.Substring(0, value.Length - 1);
                HttpContext.Session.SetString("Stores_history", value);
                return View();
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("Index");
            }
        }
        
        public IActionResult StorePurchaseHistory()
        {
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ConcurrentBag<PurchaseInvoice>> res = systemController.getStorePurchaseHistory(HttpContext.Session.GetString(SessionName), (int)HttpContext.Session.GetInt32(SessionStoreID));
            if (res.getTag())
            {
                string value = "";
                foreach (PurchaseInvoice inv in res.getValue())
                {
                    value += inv.ToString() + "\n" + ";";
                }
                if(value!="")
                    value = value.Substring(0, value.Length - 1);
                HttpContext.Session.SetString("store_history", value);
                return View();
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("Index");
            }
        }
        
        public IActionResult ViewOfficials()
        {
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ConcurrentDictionary<string,ConcurrentLinkedList<Permissions>>> res = systemController.getOfficialsInformation(HttpContext.Session.GetString(SessionName), (int)HttpContext.Session.GetInt32(SessionStoreID));
            if (res.getTag())
            {
                string[] value = new string[res.getValue().Count];
                int i = 0;
                foreach (string name in res.getValue().Keys)
                {
                    value[i] = name;
                    i++;
                }
                HttpContext.Session.SetObject("officials", value);
                return View();
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View();
            }
        }
        
        public IActionResult RemoveManager(OfficialsModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            RegularResult res = systemController.removeStoreManager(userName,model.UserName, (int)storeID);
            if (res.getTag())
            {
                return RedirectToAction("ViewOfficials");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("ViewOfficials");
            }
        }
        
        public IActionResult RemoveOwner(OfficialsModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            RegularResult res = systemController.removeStoreOwner(userName,model.UserName, (int)storeID);
            if (res.getTag())
            {
                return RedirectToAction("ViewOfficials");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("ViewOfficials");
            }
        }

        public ConcurrentLinkedList<int> changeListType(ConcurrentLinkedList<Permissions> pers)
        {
            ConcurrentLinkedList<int> permissions = new ConcurrentLinkedList<int>();
            Node<Permissions> node = pers.First; // going over the user's permissions to check if he is a store manager or owner
            while(node.Next != null)
            {
                permissions.TryAdd((int) node.Value);
                node = node.Next;
            }
            return permissions;
        }
        
        public IActionResult AddManagerPermission(OfficialsModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            ConcurrentLinkedList<Permissions> pers = systemController.getOfficialsInformation(HttpContext.Session.GetString(SessionName), (int)HttpContext.Session.GetInt32(SessionStoreID)).getValue()[model.UserName];
            pers.TryAdd((Permissions) model.Permission);
            ConcurrentLinkedList<int> permissions = changeListType(pers);
            RegularResult res = systemController.editManagerPermissions(userName,model.UserName,permissions, (int)storeID);
            if (res.getTag())
            {
                return RedirectToAction("ViewOfficials");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return View("ViewOfficials");
            }
        }
    }
}