using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Models;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;
using Microsoft.AspNetCore.SignalR;
using WebApplication.Communication;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<NotificationHub> _notificationUserHubContext;
        //private readonly IUserConnectionManager _userConnectionManager;
        public HomeController(ILogger<HomeController> logger, IHubContext<NotificationHub> notificationUserHubContext) 
            //IUserConnectionManager userConnectionManager)
        {
            _logger = logger;
            _notificationUserHubContext = notificationUserHubContext;
            //_userConnectionManager = userConnectionManager;
        }

        const string SessionName = "_Name";  
        const string SessionAge = "_Age";  
        const string SessionLogin = "_Login";  
        const string SessionStoreID = "_StoreID";
        const string SessionItemID = "_ItemID";
        const string SessionPurchaseHistory = "_History";
        
        public IActionResult Index()  
        {
            HttpContext.Session.SetString(SessionName, "");
            HttpContext.Session.SetInt32(SessionLogin, 0);
            return View();  
        }  
        
        [HttpPost]
        public async void SendToSpecificUser(String userName, String msg)
        {
            var connections = UserConnectionManager.Instance.GetUserConnections(userName);
            if (connections != null && connections.Count > 0) //the user is logged in
            {
                foreach (var connectionId in connections)
                {
                    await _notificationUserHubContext.Clients.Client(connectionId).SendAsync("sendToUser", msg);
                }
            }
            else
            {
                UserConnectionManager.Instance.KeepNotification(userName,msg);
            }
        }

        public IActionResult ItemReview(PurchaseModel model)
        {
            string info = model.itemInfo;
            if (info != null)
            {
                string[] subInfo = info.Split(",");
                HttpContext.Session.SetInt32(SessionItemID, int.Parse(subInfo[1].Substring(10)));
                HttpContext.Session.SetInt32(SessionStoreID, int.Parse(subInfo[3].Substring(14)));
                return View();
            }

            return RedirectToAction("PurchaseHistory");
        }
        
        public IActionResult AppointOfficials()
        {
            string[] users = SystemController.Instance.getAllSignedUpUsers();
            HttpContext.Session.SetObject("allUsers", users);
            return View();
        }

        public IActionResult PurchaseHistory()
        {
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> res = systemController.getUserPurchaseHistory(HttpContext.Session.GetString(SessionName));
            if (res.getTag())
            {
                string value = "";
                foreach (PurchaseInvoice inv in res.getValue().Values)
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
                TempData["alert"] = res.getMessage();
                return View("Index");
            }
        }

        public IActionResult ShoppingCart(ShoppingCartModel model)
        {
            SystemController systemController = SystemController.Instance;
            ShoppingCart res = systemController.viewShoppingCart(HttpContext.Session.GetString(SessionName)).getValue();
            ShoppingCartItems(res);
            TryPriceBeforeSale(model);
            TryPriceAftereSale(model);
            return View();
        }

        private void ShoppingCartItems(ShoppingCart shoppingCart)
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
        
        private string[] listToArray(ConcurrentLinkedList<PurchaseType> lst)
        {
            string[] arr = new string[lst.size];
            int i = 0;
            Node<PurchaseType> node = lst.First; // going over the user's permissions to check if he is a store manager or owner
            int size = lst.size;
            while(size > 0)
            {
                arr[i] = node.Value.ToString();
                node = node.Next;
                i++;
                size--;
            }
            return arr;
        }
        
        public IActionResult PurchaseTypes(StoreModel model)
        {
            if (model.storeInfo != null)
            {
                model.storeID = int.Parse(model.storeInfo.Split(",")[0].Substring(10));
                HttpContext.Session.SetInt32(SessionStoreID, model.storeID);
            }
            SystemController systemController = SystemController.Instance;
            string[] types = {PurchaseType.ImmediatePurchase.ToString(), PurchaseType.SubmitOfferPurchase.ToString()};
            string userName = HttpContext.Session.GetString(SessionName);
            int storeID = (int)HttpContext.Session.GetInt32(SessionStoreID);
            ConcurrentLinkedList<PurchaseType> lst = systemController.getStorePurchaseTypes(userName, storeID);
            HttpContext.Session.SetObject("storepurchasetypes", listToArray(lst));
            HttpContext.Session.SetObject("purchasetypes", types);
            return View();
        }
        
        public IActionResult StoreActions()
        {
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ConcurrentLinkedList<int>> res = systemController.getUsersStores(HttpContext.Session.GetString(SessionName));
            int[] stores = listToArray(res.getValue());
            string[] storesValues = new string[stores.Length];
            for (int i = 0; i < stores.Length; i++)
            {
                string value = "Store ID: " + stores[i] + ", Store Name: " +
                               systemController.getStoreByID(stores[i]).storeName +
                               ", Store Address: " + systemController.getStoreByID(stores[i]).storeAddress; 
                storesValues[i] = value;
            }
            HttpContext.Session.SetObject("stores", storesValues);
            return View();
        }

        public IActionResult ItemActions(StoreModel model)
        {
            model.storeID = int.Parse(model.storeInfo.Split(",")[0].Substring(10));
            if (model.storeID != 0)
            {
                SystemController systemController = SystemController.Instance;
                ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> res = systemController.getItemsInStoresInformation();
                HttpContext.Session.SetInt32(SessionStoreID, model.storeID);
                checkStoresItems(model.storeID, res);
                return View();
            }
            return RedirectToAction("StoreActions");
        }
        
        public IActionResult EditItemDetails(ItemModel model)
        {
            SystemController systemController = SystemController.Instance;
            KeyValuePair<Item, int> pair = systemController.getItemByID(model.itemID);
            HttpContext.Session.SetInt32(SessionItemID, pair.Key.itemID);
            Item item = pair.Key;
            model.storeID = pair.Value;
            model.itemName = item.itemName;
            // !!! TODO: fix category to work with ItemCategory !!!
            model.category = "";
            model.description = item.description;
            model.quantity = item.quantity;
            model.price = item.price;
            model.reviews = item.reviews;
            return View(model);
        }
        
        public IActionResult SearchItems(SearchModel model)
        {
            //this is the first screen that the user sees after he logs in/continue as guest
            //SendDelayedNotificationsToUser(HttpContext.Session.GetString(SessionName));
            SystemController systemController = SystemController.Instance;
            if (!model.flag)
            {
                ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> res = systemController.getItemsInStoresInformation();
                allItemStrings(res);
            }
            string[] types = {PurchaseType.ImmediatePurchase.ToString(), PurchaseType.SubmitOfferPurchase.ToString()};
            HttpContext.Session.SetObject("purchasetypes", types);
            model.items = HttpContext.Session.GetObject<string[]>("allitemstrings");
            return View(model);
        }
        
        public IActionResult ShowReviews(ShowReviewsModel model)
        {
            model = new ShowReviewsModel();
            SystemController systemController = SystemController.Instance;
            int itemID = (int)HttpContext.Session.GetInt32(SessionItemID);
            if (itemID != 0)
            {
                KeyValuePair<Item, int> pair = systemController.getItemByID(itemID);
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
            TempData["alert"] = null;
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
                TempData["alert"] = res.getMessage();
                return RedirectToAction("Index");
            }
        }
        
        public IActionResult ContinueAsGuest(UserModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            RegularResult res = systemController.continueAsGuest(model.UserName);
            if (res.getTag())
            {
                HttpContext.Session.SetString(SessionName, model.UserName);
                HttpContext.Session.SetInt32(SessionLogin, 3);
                return RedirectToAction("SearchItems");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("Index");
            }
        }
        
        public IActionResult TryEditDetails(ItemModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            //KeyValuePair<Item, int> pair = StoreRepository.Instance.getItemByID(model.itemID);
            ItemDTO itemDto = new ItemDTO((int)HttpContext.Session.GetInt32(SessionStoreID),
                model.quantity,
                model.itemName,
                model.description,
                model.reviews,
                model.price,
                0);
            itemDto.itemID = (int) HttpContext.Session.GetInt32(SessionItemID);
            RegularResult res = systemController.editItemDetails(HttpContext.Session.GetString(SessionName), (int)HttpContext.Session.GetInt32(SessionStoreID), itemDto);
            if (res.getTag())
            {
                return RedirectToAction("StoreActions");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("StoreActions");
            }
        }
        
        public IActionResult TrySearchItems(SearchModel model)
        {
            SystemController systemController = SystemController.Instance;
            if (model.itemName == null) model.itemName = "";
            if (model.keyWords == null) model.keyWords = "";
            //if (model.minPrice == 0) model.minPrice = int.MinValue;
            if (model.maxPrice == 0) model.maxPrice = int.MaxValue;
            if (model.category == null) model.category = "";
            ConcurrentDictionary<Item,int> res = systemController.searchItems(model.itemName, model.keyWords,model.minPrice, model.maxPrice, 0);
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
            return RedirectToAction("SearchItems", model);
        }

        public IActionResult TryReviewItem(ReviewModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            ResultWithValue<NotificationDTO> res = systemController.itemReview(HttpContext.Session.GetString(SessionName), model.review, (int)HttpContext.Session.GetInt32(SessionItemID), (int)HttpContext.Session.GetInt32(SessionStoreID));
            if (res.getTag())
            {
                Node<string> node = res.getValue().usersToSend.First; // going over the user's permissions to check if he is a store manager or owner
                string userName = HttpContext.Session.GetString(SessionName);
                int itemID= (int)HttpContext.Session.GetInt32("_ItemID");
                while (node.Next != null)
                {
                    SendToSpecificUser(node.Value, res.getValue().msgToSend);
                    node = node.Next;
                }
                return RedirectToAction("ItemReview");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("ItemReview");
            }
        }
        
        public IActionResult TryAppointManager(AppointModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            RegularResult res = systemController.appointStoreManager(HttpContext.Session.GetString(SessionName),
                model.userName, (int)HttpContext.Session.GetInt32(SessionStoreID));
            if (res.getTag())
            {
                return RedirectToAction("AppointOfficials");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("AppointOfficials");
            }
        }
        
        public IActionResult TryAppointOwner(AppointModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            RegularResult res = systemController.appointStoreOwner(HttpContext.Session.GetString(SessionName),
                model.userName, (int)HttpContext.Session.GetInt32(SessionStoreID));
            if (res.getTag())
            {
                return RedirectToAction("AppointOfficials");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("AppointOfficials");
            }
        }

        public IActionResult TryLogin(UserModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            RegularResult res = systemController.login(model.UserName, model.Password);
            string userName = model.UserName;
            if (res.getTag())
            {
                HttpContext.Session.SetString(SessionName, model.UserName);
                HttpContext.Session.SetInt32(SessionLogin, 1);
                return RedirectToAction("SearchItems");
            }
            else
            {
                UserConnectionManager.Instance.RemoveUser(userName);
                TempData["alert"] = res.getMessage();
                return RedirectToAction("Login");
            }
        }

        public IActionResult LoginAsSystemManager(UserModel model)
        {
            SystemController systemController = SystemController.Instance;
            RegularResult res = systemController.loginAsSystemManager(model.UserName, model.Password);
            if (res.getTag())
            {
                HttpContext.Session.SetString(SessionName, model.UserName);
                HttpContext.Session.SetInt32(SessionLogin, 2);
                return RedirectToAction("SearchItems");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("Login");
            }
        }
        
        public IActionResult TryLogout(UserModel model)
        {
            TempData["alert"] = null;
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
                    TempData["alert"] = res.getMessage();
                    return RedirectToAction("Logout");
                }
            }
            else
            {
                HttpContext.Session.SetString(SessionName, "");
                HttpContext.Session.SetInt32(SessionLogin, 0);
                return RedirectToAction("Logout");
            }
            
        }
        
        public IActionResult TryOpenStore(StoreModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            ResultWithValue<int> res = systemController.openStore(userName, model.storeName, model.storeAddress, model.purchasePolicy, model.salesPolicy);
            if (res.getTag())
            {
                HttpContext.Session.SetInt32(SessionStoreID, res.getValue());
                return RedirectToAction("StoreActions");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("StoreActions");
            }
        }

        private int[] listToArray(ConcurrentLinkedList<int> lst)
        {
            int[] arr = new int[lst.size];
            int i = 0;
            Node<int> node = lst.First; // going over the user's permissions to check if he is a store manager or owner
            int size = lst.size;
            while(size > 0)
            {
                arr[i] = node.Value;
                node = node.Next;
                i++;
                size--;
            }
            return arr;
        }

        public IActionResult TryAddItem(ItemModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            // !!! TODO: fix category to work with ItemCategory !!!
            ItemDTO item = new ItemDTO((int) storeID, model.quantity, model.itemName, model.description,
                new ConcurrentDictionary<string, ItemReview>(), model.price, 0);
            ResultWithValue<int> res = systemController.addItemToStorage(userName, (int)storeID, item);
            if (res.getTag())
            {
                HttpContext.Session.SetInt32(SessionItemID, res.getValue());
                return RedirectToAction("StoreActions");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("StoreActions");
            }
        }
        
        public IActionResult TryRemoveItem(ItemModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            RegularResult res = systemController.removeItemFromStorage(userName, (int)storeID, model.itemID);
            if (res.getTag())
            {
                return RedirectToAction("StoreActions");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("StoreActions");
            }
        }
        
        private Item[] itemListToArray2(ConcurrentLinkedList<Item> lst)
        {
            Item[] arr = new Item[lst.size];
            int i = 0;
            Node<Item> node = lst.First;
            int size = lst.size;
            while(size > 0)
            {
                arr[i] = node.Value;
                node = node.Next;
                i++;
                size--;
            }
            return arr;
        }

        private Item findThisItem(ConcurrentLinkedList<Item> items, int itemID)
        {
            Item[] itms = itemListToArray2(items);
            for (int i = 0; i < itms.Length; i++)
            {
                if (itms[i].itemID == itemID)
                    return itms[i];
            }
            return null;
        }

        private double findItem(ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> stores, int itemID, int storeID)
        {
            foreach (var pair in stores)
            {
                if (pair.Key.storeID == storeID)
                {
                    return findThisItem(pair.Value, itemID).price;
                }
            }
            return -1;
        }
        
        public IActionResult TryAddItemToShoppingCart(SearchModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            if (model.itemChosen != null)
            {
                string[] authorsList = model.itemChosen.Split(": ");
                string[] store = authorsList[1].Split(" ");
                int storeID = int.Parse(store[0]);
                int itemID = int.Parse(authorsList[authorsList.Length - 1]);
                int purchaseType = stringToEnumPT(model.purchaseType);
                double price = findItem(systemController.getItemsInStoresInformation(), itemID, storeID);
                if (purchaseType == 1)
                {
                    price = model.priceOffer;
                }
                ResultWithValue<NotificationDTO> res = systemController.addItemToShoppingCart(userName, storeID, itemID, model.quantity, purchaseType, price);
                if (res.getTag())
                {
                    return RedirectToAction("SearchItems");
                }
                else
                {
                    TempData["alert"] = res.getMessage();
                    return RedirectToAction("SearchItems");
                }
            }
            else
            {
                return RedirectToAction("SearchItems");
            }
        }

        public IActionResult TryShowPurchaseHistory(PurchaseModel model)
        {
            TempData["alert"] = null;
            return RedirectToAction("ItemReview", model);
        }

        public IActionResult TryShowShoppingCart()
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ShoppingCart> res = systemController.viewShoppingCart(HttpContext.Session.GetString(SessionName));
            if (res.getTag())
            {
                HttpContext.Session.SetObject("shopping_cart", res.getValue());
                return null;
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("SearchItems");
            }
        }

        private int[] itemListToArray(ConcurrentLinkedList<Item> lst)
        {
            int[] arr = new int[lst.size];
            int i = 0;
            Node<Item> node = lst.First;
            int size = lst.size;
            while(size > 0)
            {
                arr[i] = node.Value.itemID;
                node = node.Next;
                i++;
                size--;
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
            int size = lst.size;
            while(size > 0)
            {
                arr[i] = "StoreID: "+storeID+" "+ node.Value.ToString();
                node = node.Next;
                i++;
                size--;
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
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            string itemID = model.itemID;
            if (itemID!=null && itemID != "")
            {
                string[] strs = itemID.Split(":");
                int item = int.Parse(strs[strs.Length - 1]);
                RegularResult res = systemController.removeItemFromShoppingCart(userName, (int)storeID, item);
                if (res.getTag())
                {
                    return RedirectToAction("ShoppingCart");
                }
                else
                {
                    TempData["alert"] = res.getMessage();
                    return RedirectToAction("ShoppingCart");
                }
            }
            else
            {
                return RedirectToAction("ShoppingCart");
            }
        }
        public IActionResult TrypurchaseItems(ShoppingCartModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            PaymentParametersDTO payment = new PaymentParametersDTO(model.cardNumber, model.month, model.year,
                model.holder, model.ccv, model.id);
            DeliveryParametersDTO delivery =
                new DeliveryParametersDTO(model.sendToName, model.address, model.city, model.country, model.zip);
            ResultWithValue<NotificationDTO> res = systemController.purchaseItems(userName, delivery, payment);
            if (res.getTag())
            {
                Node<string> node = res.getValue().usersToSend.First;
                while (node.Next != null)
                {
                    SendToSpecificUser(node.Value, res.getValue().msgToSend);
                    node = node.Next;
                }
                return RedirectToAction("ShoppingCart");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("ShoppingCart");
            }
        }
        
        public IActionResult UsersPurchaseHistory()
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ConcurrentDictionary<string, ConcurrentDictionary<int, PurchaseInvoice>>> res = systemController.getUsersPurchaseHistory(HttpContext.Session.GetString(SessionName));
            if (res.getTag())
            {
                string value = "";
                foreach (KeyValuePair<string, ConcurrentDictionary<int, PurchaseInvoice>> invs in res.getValue())
                {
                    foreach (PurchaseInvoice inv in invs.Value.Values)
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
                TempData["alert"] = res.getMessage();
                return View();
            }
        }
        
        public IActionResult StoresPurchaseHistory()
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>>> res = systemController.getStoresPurchaseHistory(HttpContext.Session.GetString(SessionName));
            if (res.getTag())
            {
                string value = "";
                foreach (KeyValuePair<int, ConcurrentDictionary<int, PurchaseInvoice>> invs in res.getValue())
                {
                    foreach (PurchaseInvoice inv in invs.Value.Values)
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
                TempData["alert"] = res.getMessage();
                return View();
            }
        }
        
        public IActionResult StorePurchaseHistory()
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> res = systemController.getStorePurchaseHistory(HttpContext.Session.GetString(SessionName), (int)HttpContext.Session.GetInt32(SessionStoreID));
            if (res.getTag())
            {
                string value = "";
                foreach (PurchaseInvoice inv in res.getValue().Values)
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
                TempData["alert"] = res.getMessage();
                return RedirectToAction("StoreActions");
            }
        }
        
        public IActionResult ViewOfficials()
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            ResultWithValue<ConcurrentDictionary<string,ConcurrentLinkedList<Permissions>>> res = systemController.getOfficialsInformation(HttpContext.Session.GetString(SessionName), (int)HttpContext.Session.GetInt32(SessionStoreID));
            if (res.getTag()&&res.getValue()!=null&&res.getValue().Count!=0)
            {
                string[] value = new string[res.getValue().Count];
                int i = 0;
                foreach (string name in res.getValue().Keys)
                {
                    value[i] = name;
                    if (res.getValue()[name].Contains(Permissions.AllPermissions))
                        value[i] += ", Store Owner";
                    else
                        value[i] += ", Store Manager";
                    i++;
                }
                HttpContext.Session.SetObject("officials", value);
                return View();
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("StoreActions");
            }
        }
        
        public IActionResult RemoveManager(OfficialsModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            string managerName = model.UserName.Split(",")[0];
            ResultWithValue<NotificationDTO> res = systemController.removeStoreManager(userName,managerName, (int)storeID);
            if (res.getTag())
            {
                SendToSpecificUser(managerName, res.getValue().msgToSend);
                return RedirectToAction("ViewOfficials");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("ViewOfficials");
            }
        }
        
        public IActionResult RemoveOwner(OfficialsModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            string ownerName = model.UserName.Split(",")[0];
            ResultWithValue<NotificationDTO> res = systemController.removeStoreOwner(userName,ownerName, (int)storeID);
            if (res.getTag())
            {
                SendToSpecificUser(ownerName, res.getValue().msgToSend);
                return RedirectToAction("ViewOfficials");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("ViewOfficials");
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
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            string managerName = model.UserName.Split(",")[0];
            ConcurrentLinkedList<Permissions> pers = systemController.getOfficialsInformation(HttpContext.Session.GetString(SessionName), (int)HttpContext.Session.GetInt32(SessionStoreID)).getValue()[managerName];
            int per = stringToEnum(model.Permission);
            pers.TryAdd((Permissions) per);
            ConcurrentLinkedList<int> permissions = changeListType(pers);
            RegularResult res = systemController.editManagerPermissions(userName,managerName,permissions, (int)storeID);
            if (res.getTag())
            {
                return RedirectToAction("ViewOfficials");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("ViewOfficials");
            }
        }

        public int stringToEnum(string pred)
        {
            switch (pred)
            {
                case "AllPermissions":
                    return 0;
                case "StorageManagment":
                    return 1;
                case "AppointStoreManager":
                    return 2;
                case "AppointStoreOwner":
                    return 3;
                case "RemoveStoreManager":
                    return 4;
                case "EditManagmentPermissions":
                    return 5;
                case "StorePoliciesManagement":
                    return 6;
                case "GetOfficialsInformation":
                    return 7;
                case "GetStorePurchaseHistory":
                    return 8;
            }
            return -1;
        }

        public IActionResult EditSalePredicates(StoreModel model)
        {
            if (model.storeInfo != null)
            {
                model.storeID = int.Parse(model.storeInfo.Split(",")[0].Substring(10));
                HttpContext.Session.SetInt32(SessionStoreID, model.storeID);
            }
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            ResultWithValue<ConcurrentDictionary<int, string>> res = systemController.getStoreSalesDescription((int) storeID);
            if (res.getTag())
            {
                string[] preds = new string[res.getValue().Count];
                int i = 0;
                foreach (KeyValuePair<int,string> pred in res.getValue())
                {
                    preds[i] = pred.Value+ ": " + pred.Key.ToString();
                    i++;
                }
                HttpContext.Session.SetObject("sale_predicates", preds);
                return View();
            }
            else
            {
                return RedirectToAction("StoreActions");
            }
        }
        public IActionResult AddSale()
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> itemsandstores = systemController.getItemsInStoresInformation();
            Store store = null;
            ConcurrentLinkedList<Item> items = null;
            foreach (var storeid in itemsandstores.Keys)
            {
                if (storeid.storeID == storeID)
                {
                    store = storeid;
                    items = itemsandstores.GetValueOrDefault(storeid);
                    break;
                }
            }
            if (items != null)
            {
                string[] arr = new string[items.size];
                int i = 0;
                Node<Item> node = items.First;
                int size = items.size;
                while(size > 0)
                {
                    arr[i] = node.Value.ToString();
                    node = node.Next;
                    i++;
                    size--;
                }
                HttpContext.Session.SetObject("itemsList", arr);
            }
            return View();
        }
        public int categorytoenum(string pred)
        {
            switch (pred)
            {
                case "AllCategories":
                    return 0;
                case "Dairy":
                    return 1;
                case "Meat":
                    return 2;
                case "Clothing":
                    return 3;
                case "Footwear":
                    return 4;
                case "Cleaners":
                    return 5;
                case "Vegetables":
                    return 6;
                case "Electronics":
                    return 7;
                case "Health":
                    return 8;
                case "Sport":
                    return 9;
                case "Dinnerware":
                    return 10;
                case "Fruits":
                    return 11;
                case "Snacks":
                    return 12;
                case "Pastries":
                    return 13;
                case "Drinks":
                    return 14;
                case "Tools":
                    return 15;
                case "Other":
                    return 16;
            }
            return -1;
        }
        public IActionResult TryAddSale(SalesModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            ApplySaleOn typeSale = null;
            if (model.itemID != null)
            {
                string itemid = model.itemID;
                string[] s = itemid.Split(": ");
                int itemidd = int.Parse(s[s.Length - 1]);
                typeSale = new SaleOnItem(itemidd);
            }
            else if (model.category != null)
            {
                string category = model.category; 
                typeSale = new SaleOnCategory((ItemCategory)categorytoenum(category));
            }
            else
            {
                typeSale = new SaleOnAllStore();
            }

            if (typeSale != null)
            {
                ResultWithValue<int> res = systemController.addSale(userName, (int) storeID, model.salePercentage, typeSale,
                    model.saleDescription);
                if (res.getTag())
                {
                    return RedirectToAction("AddSale");
                }
                else
                {
                    TempData["alert"] = res.getMessage();
                    return RedirectToAction("AddSale");
                }
            }
            return RedirectToAction("AddSale");
        }
        private void TryAllSales()
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            ResultWithValue<ConcurrentDictionary<int, string>> salesinfo = systemController.getStoreSalesDescription((int)storeID);
            if (salesinfo.getTag())
            {
                string[] saleidanddesc = new string[salesinfo.getValue().Count];
                int i = 0;
                foreach (KeyValuePair<int,string> pred in salesinfo.getValue())
                {
                    saleidanddesc[i] = pred.Value+ ": " + pred.Key.ToString();
                    i++;
                }
                HttpContext.Session.SetObject("sales_info", saleidanddesc);
            }
        }

        public IActionResult TryAddSaleCondition(SalesModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            int composetype = saleStringToEnum(model.compositionType);
            SimplePredicate typeCondition = null;
            LocalPredicate<PurchaseDetails> pred = null;
            //Predicate<PurchaseDetails> newPred = null;
            if (model.numbersOfProducts != 0)
            {
                Expression<Func<PurchaseDetails, double>> exp = pd => pd.numOfItemsInPurchase();
                pred = new LocalPredicate<PurchaseDetails>(exp, model.numbersOfProducts);
                //newPred = pd => pd.numOfItemsInPurchase() >= model.numbersOfProducts;
            }

            if (model.priceOfShoppingBag != 0)
            {
                Expression<Func<PurchaseDetails, double>> exp = pd => pd.totalPurchasePrice();
                pred = new LocalPredicate<PurchaseDetails>(exp, model.priceOfShoppingBag);
                //newPred = pd => pd.totalPurchasePrice() >= model.priceOfShoppingBag;
            }

            if (model.ageOfUser != 0)
            {
                Expression<Func<PurchaseDetails, double>> exp = pd => pd.userAge();
                pred = new LocalPredicate<PurchaseDetails>(exp, model.ageOfUser);
                //newPred = pd => pd.userAge() >= model.ageOfUser;
            }
            typeCondition = new SimplePredicate(pred, model.saleDescription);
            int saleID = 0;
            if (model.saleinfo != null)
            {
                string saleid = model.saleinfo;
                string[] s = saleid.Split(": ");
                saleID = int.Parse(s[s.Length - 1]);
            }

            ResultWithValue<int> salecond =
                systemController.addSaleCondition(userName, (int)storeID, saleID, typeCondition, composetype);
            if (salecond.getTag())
            {
                return RedirectToAction("AddSale");
            }
            else
            {
                TempData["alert"] = salecond.getMessage();
                return RedirectToAction("AddSale");
            }
        }
        
        public IActionResult TryRemoveSalePredicate(SalesModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            ResultWithValue<ConcurrentDictionary<int, string>> salesPredicatesDescription =
                systemController.getStoreSalesDescription((int)storeID);
            String pre = model.predicate;
            string[] s = pre.Split(": ");
            int predicateIDDD = int.Parse(s[s.Length - 1]);
            RegularResult res = systemController.removeSale(userName, (int)storeID, predicateIDDD);
            if (res.getTag())
            {
                return RedirectToAction("StoreActions");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("StoreActions");
            }
        }
        
        public int saleStringToEnum(string pred)
        {
            switch (pred)
            {
                case "XorComposition":
                    return 0;
                case "MaxComposition":
                    return 1;
                case "DoubleComposition":
                    return 2;
            }
            return -1;
        }
        
        public IActionResult ComposeSalePredicates(SalesModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            ResultWithValue<ConcurrentDictionary<int, string>> salePredicatesDescription =
                systemController.getStoreSalesDescription((int)storeID);
            int predicate1ID = 0;
            int predicate2ID = 0;
            foreach (string purpre in salePredicatesDescription.getValue().Values)
            {
                if (purpre.Equals(model.firstPred))
                {
                    predicate1ID = salePredicatesDescription.getValue().FirstOrDefault(x => x.Value == purpre).Key;
                    break;
                }
            }
            foreach (string purpre in salePredicatesDescription.getValue().Values)
            {
                if (purpre.Equals(model.secondPred))
                {
                    predicate2ID = salePredicatesDescription.getValue().FirstOrDefault(x => x.Value == purpre).Key;
                    break;
                }
            }
            int composetype = saleStringToEnum(model.compositionType);
            ResultWithValue<int> res = systemController.composeSales(userName, (int)storeID, predicate1ID,predicate2ID,composetype, null);
            if (res.getTag())
            {
                return RedirectToAction("StoreActions");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("StoreActions");
            }
        }
        
        public int stringToEnumPT(string pred)
        {
            switch (pred)
            {
                case "ImmediatePurchase":
                    return 0;
                case "SubmitOfferPurchase":
                    return 1;
            }
            return -1;
        }
        
        public IActionResult TryRemovePurchaseType(PurchaseTypesModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int storeID = (int)HttpContext.Session.GetInt32(SessionStoreID);
            RegularResult res = systemController.unsupportPurchaseType(userName, storeID, stringToEnumPT(model.purchaseType));
            if (res.getTag())
            {
                return RedirectToAction("StoreActions");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("StoreActions");
            }
        }
        
        public IActionResult TryAddPurchaseType(PurchaseTypesModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int storeID = (int)HttpContext.Session.GetInt32(SessionStoreID);
            RegularResult res = systemController.supportPurchaseType(userName, storeID, stringToEnumPT(model.purchaseType2));
            if (res.getTag())
            {
                return RedirectToAction("StoreActions");
            }
            else
            {
                TempData["alert"] = res.getMessage();
                return RedirectToAction("StoreActions");
            }
        }
        public IActionResult PurchasePredicate(StoreModel model)
        {
            if (model.storeInfo != null)
            {
                model.storeID = int.Parse(model.storeInfo.Split(",")[0].Substring(10));
                HttpContext.Session.SetInt32(SessionStoreID, model.storeID);
            }
            SystemController systemController = SystemController.Instance;
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> info =
                systemController.getItemsInStoresInformation();
            Store store = null;
            foreach (var storeid in info.Keys)
            {
                if (storeid.storeID == storeID)
                {
                    store = storeid;
                    break;
                }
            }
            if (store != null)
            {
                ResultWithValue<ConcurrentDictionary<int, string>> storePredicatesDescription =
                    systemController.getStorePredicatesDescription((int)storeID);
                LinkedList<string> purchasepredicateList = new LinkedList<string>();
                foreach (string purpre in storePredicatesDescription.getValue().Values)
                {
                    string purchasepredicate = purpre;
                    purchasepredicateList.AddLast(purchasepredicate);
                }
                
                string[] strs = purchasepredicateList.ToArray();
                HttpContext.Session.SetObject("PurchasePredicate", strs);
            }
            return View();
        }
        public IActionResult TryRemovePurchasePredicate(PredicateModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            ResultWithValue<ConcurrentDictionary<int, string>> storePredicatesDescription =
                systemController.getStorePredicatesDescription((int)storeID);
            int predicateID = 0;
            foreach (string purpre in storePredicatesDescription.getValue().Values)
            {
                if (purpre.Equals(model.predicate))
                {
                    predicateID = storePredicatesDescription.getValue().FirstOrDefault(x => x.Value == purpre).Key;
                    break;
                }
            }
            RegularResult res = systemController.removePurchasePredicate(userName, (int)storeID, predicateID);
                if (res.getTag())
                {
                    return RedirectToAction("PurchasePredicate");
                }
                else
                {
                    ViewBag.Alert = res.getMessage();
                    return RedirectToAction("PurchasePredicate");
                }
        }

        public IActionResult TryAddPurchasePredicate(PredicateModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            LocalPredicate<PurchaseDetails> pred = null;
            //Predicate<PurchaseDetails> newPred = null;
            string description = "";
            if (model.numbersOfProducts != 0)
            {
                Expression<Func<PurchaseDetails, double>> exp = pd => pd.numOfItemsInPurchase();
                //newPred = pd => pd.numOfItemsInPurchase() >= model.numbersOfProducts;
                pred = new LocalPredicate<PurchaseDetails>(exp, model.numbersOfProducts);
                description = $"number of products is bigger than: {model.numbersOfProducts.ToString()}";
            }
            if (model.priceOfShoppingBag != 0)
            {
                Expression<Func<PurchaseDetails, double>> exp = pd => pd.totalPurchasePrice();
                //newPred = pd => pd.numOfItemsInPurchase() >= model.numbersOfProducts;
                pred = new LocalPredicate<PurchaseDetails>(exp, model.priceOfShoppingBag);
                //newPred = pd => pd.totalPurchasePrice() >= model.priceOfShoppingBag;
                description = $"price of shopping bag is bigger than: {model.priceOfShoppingBag.ToString()}";
            }
            if (model.ageOfUser != 0)
            {
                Expression<Func<PurchaseDetails, double>> exp = pd => pd.userAge();
                //newPred = pd => pd.numOfItemsInPurchase() >= model.numbersOfProducts;
                pred = new LocalPredicate<PurchaseDetails>(exp, model.ageOfUser);
                //newPred = pd => pd.userAge() >= model.ageOfUser;
                description = $"age of user is bigger than: {model.ageOfUser.ToString()}";
            }
            if (pred != null)
            {
                ResultWithValue<int> res = systemController.addPurchasePredicate(userName, (int) storeID, pred, description);
                if (res.getTag())
                {
                    return RedirectToAction("PurchasePredicate");
                }
                else
                {
                    ViewBag.Alert = res.getMessage();
                    return RedirectToAction("PurchasePredicate");
                }
            }
            else
            {
                return RedirectToAction("PurchasePredicate");
            }
        }
        
        public int composePurchasePredicateStringToEnum(string pred)
        {
            switch (pred)
            {
                case "AndComposition":
                    return 0;
                case "OrComposition":
                    return 1;
                case "ConditionalComposition":
                    return 2;
            }
            return -1;
        }
        
        public IActionResult TryComposePurchasePredicate(PredicateModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            ResultWithValue<ConcurrentDictionary<int, string>> storePredicatesDescription =
                systemController.getStorePredicatesDescription((int)storeID);
            int predicate1ID = 0;
            int predicate2ID = 0;
            foreach (string purpre in storePredicatesDescription.getValue().Values)
            {
                if (purpre.Equals(model.firstPred))
                {
                    predicate1ID = storePredicatesDescription.getValue().FirstOrDefault(x => x.Value == purpre).Key;
                    break;
                }
            }
            foreach (string purpre in storePredicatesDescription.getValue().Values)
            {
                if (purpre.Equals(model.secondPred))
                {
                    predicate2ID = storePredicatesDescription.getValue().FirstOrDefault(x => x.Value == purpre).Key;
                    break;
                }
            }

            int compositionTypee = composePurchasePredicateStringToEnum(model.compositionType);
            ResultWithValue<int> res = systemController.composePurchasePredicates(userName, (int)storeID, predicate1ID,predicate2ID,compositionTypee);
            if (res.getTag())
            {
                return RedirectToAction("PurchasePredicate");
            }
            else
            {
                ViewBag.Alert = res.getMessage();
                return RedirectToAction("PurchasePredicate");
            }
        }

        public IActionResult TryPriceAftereSale(ShoppingCartModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>>
                pricesAfterSale = systemController.getItemsAfterSalePrices(userName);
            double finalPriceAfterSale = 0;
            foreach (ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>> dict in pricesAfterSale.getValue().Values)
            {
                foreach (KeyValuePair<double, PriceStatus> keyValuePair in dict.Values)
                {
                    finalPriceAfterSale = finalPriceAfterSale + keyValuePair.Key;
                }
            }
            HttpContext.Session.SetObject("finalPriceAfterSale", finalPriceAfterSale);
            return RedirectToAction("TryPriceAftereSale");
        }
        public IActionResult TryPriceBeforeSale(ShoppingCartModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            ShoppingCart res = systemController.viewShoppingCart(HttpContext.Session.GetString(SessionName)).getValue();
            ConcurrentDictionary<int, ShoppingBag> shoppingBagss = res.shoppingBags;

            double finalPriceBeforeSale = 0;
            foreach (ShoppingBag shopBag in shoppingBagss.Values)
            {
                ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>> prices = shopBag.allItemsPricesAndStatus();
                foreach (KeyValuePair<double, PriceStatus> keyValuePair in prices.Values)
                {
                    finalPriceBeforeSale = finalPriceBeforeSale + keyValuePair.Key;
                }
            }
            HttpContext.Session.SetObject("finalPriceBeforeSale", finalPriceBeforeSale);
            return RedirectToAction("TryPriceBeforeSale");
        }
        
        public IActionResult GetStoreInformation(StoreModel model)
        {
            model.storeID = int.Parse(model.storeInfo.Split(",")[0].Substring(10));
            HttpContext.Session.SetInt32(SessionStoreID, model.storeID);
            return View();
        }

        public IActionResult AddPredicate()
        {
            return View();
        }

        public IActionResult AddSaleCondition()
        {
            TryAllSales();
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> itemsandstores = systemController.getItemsInStoresInformation();
            Store store = null;
            ConcurrentLinkedList<Item> items = null;
            foreach (var storeid in itemsandstores.Keys)
            {
                if (storeid.storeID == storeID)
                {
                    store = storeid;
                    items = itemsandstores.GetValueOrDefault(storeid);
                    break;
                }
            }
            if (items != null)
            {
                string[] arr = new string[items.size];
                int i = 0;
                Node<Item> node = items.First;
                int size = items.size;
                while(size > 0)
                {
                    arr[i] = node.Value.ToString();
                    node = node.Next;
                    i++;
                    size--;
                }
                HttpContext.Session.SetObject("itemsList", arr);
            }
            return View();
        }
        
        public IActionResult EditPurchasePredicates()
        {
            return View();
        }

        public IActionResult BidsReview()
        {
            return View();
        }
    }
}