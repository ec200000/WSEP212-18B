﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            while(node.Next != null)
            {
                arr[i] = node.Value.ToString();
                node = node.Next;
                i++;
            }
            return arr;
        }
        
        public IActionResult PurchaseTypes()
        {
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
        
        public IActionResult GetStoreInformation(StoreModel model)
        {
            model.storeID = int.Parse(model.storeInfo.Split(",")[0].Substring(10));
            HttpContext.Session.SetInt32(SessionStoreID, model.storeID);
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
            // !!! TODO: fix category to work with ItemCategory !!!
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
            // !!! TODO: fix category to work with ItemCategory !!!
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
                // !!! TODO: ADD CHOOSE PURCHASE TYPE, AND PRICE TO OFFER (FOR IMMIDIATE INSERT THE REAL PRICE) !!!
                RegularResult res = systemController.addItemToShoppingCart(userName, storeID, itemID, model.quantity, 0, model.maxPrice);
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

        public IActionResult EditSalePredicates()
        {
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
                    preds[i] = pred.Value+ "; " + pred.Key.ToString();
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
        
        public IActionResult TryRemoveSalePredicate(SalesModel model)
        {
            TempData["alert"] = null;
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            string pred = model.predicate;
            string[] predparts = pred.Split(";");
            int predicate = int.Parse(predparts[1]); 
            RegularResult res = systemController.removeSale(userName, (int)storeID, predicate);
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
            string firstpred = model.firstPred;
            string[] predparts1 = firstpred.Split(";");
            int predicate1 = int.Parse(predparts1[1]);
            string secondpred = model.secondPred;
            string[] predparts2 = secondpred.Split(";");
            int predicate2 = int.Parse(predparts2[1]);
            int composetype = saleStringToEnum(model.compositionType);
            ResultWithValue<int> res = systemController.composeSales(userName, (int)storeID, predicate1,predicate2,composetype, null);
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
    }
}