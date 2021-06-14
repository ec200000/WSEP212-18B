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
using PostSharp.Aspects;
using WebApplication.Communication;
using WebApplication.Publisher;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SystemLoggers;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<NotificationHub> _notificationUserHubContext;
        private readonly IHubContext<ChartNotifications> _chartHubContext;
        public HomeController(ILogger<HomeController> logger, IHubContext<NotificationHub> notificationUserHubContext, IHubContext<ChartNotifications> chartHubContext)
        {
            _logger = logger;
            _notificationUserHubContext = notificationUserHubContext;
            _chartHubContext = chartHubContext;
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
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
            }
        }

        public IActionResult ItemReview(PurchaseModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult AppointOfficials()
        {
            try
            {
                string[] users = SystemController.Instance.getAllSignedUpUsers();
                HttpContext.Session.SetObject("allUsers", users);
                return View();
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult PurchaseHistory()
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult ShoppingCart(ShoppingCartModel model)
        {
            try
            {
                SystemController systemController = SystemController.Instance;
                ShoppingCart res = systemController.viewShoppingCart(HttpContext.Session.GetString(SessionName)).getValue();
                ShoppingCartItems(res);
                TryPriceBeforeSale(model);
                TryPriceAftereSale(model);
                string[] types = {PurchaseType.ImmediatePurchase.ToString(), PurchaseType.SubmitOfferPurchase.ToString()};
                HttpContext.Session.SetObject("purchasetypes", types);
                //tryBidsInfo:
                string userName = HttpContext.Session.GetString(SessionName);
                ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>> dict =
                    systemController.offerItemsPricesAndStatus(userName).getValue();
                if (dict == null || dict.IsEmpty || dict.Count == 0)
                {
                    HttpContext.Session.SetObject("bidsInfo", new string[] {" "});
                    RedirectToAction("SearchItems");
                }
                else
                {
                    int size = dict.Count;
                    string[] bidsInfo = new string[size];
                    int i = 0;
                    foreach (int itemID in dict.Keys)
                    {
                        foreach (KeyValuePair<double, PriceStatus> priceAndStatus in dict.Values)
                        {
                            bidsInfo[i] = bidsInfo[i] + "itemID: " + itemID + ", price: " + priceAndStatus.Key +
                                          ", status: " + priceAndStatus.Value;
                        }
                        i++;
                    }
                    HttpContext.Session.SetObject("bidsInfo", bidsInfo);
                    RedirectToAction("SearchItems");
                }
                
                
                return View();
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult tryBidsInfo(ShoppingCartModel model)
        {
            return null;
        }

        private void ShoppingCartItems(ShoppingCart shoppingCart)
        {
            try
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
                    string storeAndItem = "StoreID:" + storeid+",";
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }
        }

        public IActionResult Privacy()
        {
            try
            {
                ViewBag.Name = HttpContext.Session.GetString(SessionName);  
                ViewBag.Age = HttpContext.Session.GetInt32(SessionAge);  
                ViewBag.StoreID = HttpContext.Session.GetInt32(SessionStoreID); 
                ViewData["Message"] = "Asp.Net Core !!!.";  
            
                return View();
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult Login()
        {
            try
            {
                ViewBag.Name = HttpContext.Session.GetString(SessionName);  
                ViewBag.Age = HttpContext.Session.GetInt32(SessionAge);  
                ViewData["Message"] = "Asp.Net Core !!!.";  
            
                return View();
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult Logout()
        {
            try
            {
                return View();
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult OpenStore()
        {
            try
            {
                return View();
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        private string[] listToArray(ConcurrentLinkedList<PurchaseType> lst)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        private string[] toStringArray(PurchaseType[] arr)
        {
            string[] array = new string[arr.Length];
            int i = 0;
            foreach (var v in arr)
            {
                if (v == PurchaseType.ImmediatePurchase)
                    array[i] = PurchaseType.ImmediatePurchase.ToString();
                if (v == PurchaseType.SubmitOfferPurchase)
                    array[i] = PurchaseType.SubmitOfferPurchase.ToString();
                i++;
            }
            return array;
        }
        
        public IActionResult PurchaseTypes(StoreModel model)
        {
            try
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
                ConcurrentDictionary<PurchaseType, int> lst = systemController.getStorePurchaseTypes(userName, storeID);
                HttpContext.Session.SetObject("storepurchasetypes", toStringArray(lst.Keys.ToArray()));
                HttpContext.Session.SetObject("purchasetypes", types);
                return View();
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult StoreActions()
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult ItemActions(StoreModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult EditItemDetails(ItemModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult SearchItems(SearchModel model)
        {
            //this is the first screen that the user sees after he logs in/continue as guest
            //SendDelayedNotificationsToUser(HttpContext.Session.GetString(SessionName));
            try
            {
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult ShowReviews(ShowReviewsModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        private string reviewsToString(ConcurrentDictionary<String, ItemReview> reviews)
        {
            try
            {
                string reviewsStr = "";
                foreach (ItemReview review in reviews.Values)
                {
                    reviewsStr += review + "\n";
                }
                return reviewsStr;
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult TryShowReviews(SearchModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult Subscribe(UserModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult ContinueAsGuest(UserModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult TryEditDetails(ItemModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult TrySearchItems(SearchModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult TryReviewItem(ReviewModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult TryAppointManager(AppointModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult TryAppointOwner(AppointModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult TryLogin(UserModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult LoginAsSystemManager(UserModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult TryLogout(UserModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
            
        }
        
        public IActionResult TryOpenStore(StoreModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        private int[] listToArray(ConcurrentLinkedList<int> lst)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult TryAddItem(ItemModel model)
        {
            try
            {
                TempData["alert"] = null;
                SystemController systemController = SystemController.Instance;
                string userName = HttpContext.Session.GetString(SessionName);
                int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
                ItemDTO item = new ItemDTO((int) storeID, model.quantity, model.itemName, model.description,
                    new ConcurrentDictionary<string, ItemReview>(), model.price, categorytoenum(model.category));
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult TryRemoveItem(ItemModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        private Item[] itemListToArray2(ConcurrentLinkedList<Item> lst)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        private Item findThisItem(ConcurrentLinkedList<Item> items, int itemID)
        {
            try
            {
                Item[] itms = itemListToArray2(items);
                for (int i = 0; i < itms.Length; i++)
                {
                    if (itms[i].itemID == itemID)
                        return itms[i];
                }
                return null;
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        private double findItem(ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> stores, int itemID, int storeID)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }
            return -1;
        }
        
        public IActionResult TryAddItemToShoppingCart(SearchModel model)
        {
            try
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
                        //HttpContext.Session.SetObject("allbids",res.getValue());
                        if (res.getValue() != null && purchaseType == 1)
                        {
                            Node<string> node = res.getValue().usersToSend.First;
                            while (node.Next != null)
                            {
                                SendToSpecificUser(node.Value, res.getValue().msgToSend);
                                systemController.addBidOffer(node.Value, (int) storeID, itemID, userName, price);
                                node = node.Next;
                            }
                        }
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult TryShowPurchaseHistory(PurchaseModel model)
        {
            try
            {
                TempData["alert"] = null;
                return RedirectToAction("ItemReview", model);
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult TryShowShoppingCart()
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        private int[] itemListToArray(ConcurrentLinkedList<Item> lst)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        private void allStoresItems(ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> dict)
        {
            try
            {
                int[] items;
                foreach (Store store in dict.Keys)
                {
                    items = itemListToArray(dict[store]);
                    string itms = "searchitems" + store.storeID;
                    HttpContext.Session.SetObject(itms, items);
                }
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }
        }
        
        private void getAllItems(ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> dict)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }
        }
        
        private void allStoresItemStrings(ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> dict)
        {
            try
            {
                string[] items;
                foreach (Store store in dict.Keys)
                {
                    items = itemListToStringArray(dict[store], store.storeID);
                    string itms = "searchitemsstr" + store.storeID;
                    HttpContext.Session.SetObject(itms, items);
                }
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }
        }
        
        private void allItemStrings(ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> dict)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }
        }
        
        private void itemsFromSearch(ConcurrentDictionary<Item,int> dict)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }
        }
        
        private void checkStoresItems(int lst, ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> dict)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }
        }
        
        private string[] itemListToStringArray(ConcurrentLinkedList<Item> lst, int storeID)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            try
            {
                return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        public IActionResult TryRemoveItemFromShoppingCart(ShoppingCartModel model)
        {
            try
            {
                TempData["alert"] = null;
                SystemController systemController = SystemController.Instance;
                string userName = HttpContext.Session.GetString(SessionName);
                //int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
                string itemID = model.itemID;
                if (itemID!=null && itemID != "")
                {
                    string[] strs = itemID.Split(",");
                    string[] itemstr = strs[1].Split(":");
                    int item = int.Parse(itemstr[itemstr.Length - 1]);
                    string[] storestr = strs[0].Split(":");
                    int store = int.Parse(storestr[storestr.Length - 1]);
                    RegularResult res = systemController.removeItemFromShoppingCart(userName, store, item);
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        public IActionResult TrypurchaseItems(ShoppingCartModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult UsersPurchaseHistory()
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult StoresPurchaseHistory()
        {
            try
            {
                TempData["alert"] = null;
                SystemController systemController = SystemController.Instance;
                ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>>> res = systemController.getStoresPurchaseHistory(HttpContext.Session.GetString(SessionName));
                if (res.getTag())
                {
                    double totalIncome = 0;
                    string value = "";
                    foreach (KeyValuePair<int, ConcurrentDictionary<int, PurchaseInvoice>> invs in res.getValue())
                    {
                        foreach (PurchaseInvoice inv in invs.Value.Values)
                        {
                            value += inv.ToString() + "\n" + ";";
                            if(inv.dateOfPurchase.Date.Equals(DateTime.Today))
                                totalIncome += inv.getPurchaseTotalPrice();
                        }
                    }
                    if(value!="")
                        value = value.Substring(0, value.Length - 1);
                    HttpContext.Session.SetString("Stores_history", value);
                    HttpContext.Session.SetString("stores_income", totalIncome.ToString());
                    return View();
                }
                else
                {
                    TempData["alert"] = res.getMessage();
                    return View();
                }
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult DailyVisitors()
        {
            try
            {
                PieChart data = new PieChart();
                //HttpContext.Session.SetObject("pieChartData", data.GetPieChartData());
                TempData["data"] = data.GetPieChartData();
                /**TempData["alert"] = null;
                SystemController systemController = SystemController.Instance;
                ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>>> res = systemController.getStoresPurchaseHistory(HttpContext.Session.GetString(SessionName));
                if (res.getTag())
                {
                    double totalIncome = 0;
                    string value = "";
                    foreach (KeyValuePair<int, ConcurrentDictionary<int, PurchaseInvoice>> invs in res.getValue())
                    {
                        foreach (PurchaseInvoice inv in invs.Value.Values)
                        {
                            value += inv.ToString() + "\n" + ";";
                            if(inv.dateOfPurchase.Date.Equals(DateTime.Today))
                                totalIncome += inv.getPurchaseTotalPrice();
                        }
                    }
                    if(value!="")
                        value = value.Substring(0, value.Length - 1);
                    HttpContext.Session.SetString("Stores_history", value);
                    HttpContext.Session.SetString("stores_income", totalIncome.ToString());
                    return View();
                }
                else
                {
                    TempData["alert"] = res.getMessage();
                    return View();
                }**/
                return View();
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult TryDailyVisitors(PurchaseModel model)
        {
            try
            {
                TempData["alert"] = null;
                return RedirectToAction("ItemReview", model);
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult StorePurchaseHistory()
        {
            try
            {
                TempData["alert"] = null;
                SystemController systemController = SystemController.Instance;
                ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> res = systemController.getStorePurchaseHistory(HttpContext.Session.GetString(SessionName), (int)HttpContext.Session.GetInt32(SessionStoreID));
                if (res.getTag())
                { 
                    double totalIncome = 0;
                    string value = "";
                    foreach (PurchaseInvoice inv in res.getValue().Values)
                    {
                        value += inv.ToString() + "\n" + ";";
                        if(inv.dateOfPurchase.Date.Equals(DateTime.Today))
                            totalIncome += inv.getPurchaseTotalPrice();
                    }
                    if(value!="")
                        value = value.Substring(0, value.Length - 1);
                    HttpContext.Session.SetString("store_history", value);
                    HttpContext.Session.SetString("store_income", totalIncome.ToString());
                    return View();
                }
                else
                {
                    TempData["alert"] = res.getMessage();
                    return RedirectToAction("StoreActions");
                }
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        private Permissions[] persListToArray(ConcurrentLinkedList<Permissions> lst)
        {
            try
            {
                Permissions[] arr = new Permissions[lst.size];
                int i = 0;
                Node<Permissions> node = lst.First;
                while(node.Next != null)
                {
                    arr[i] = node.Value;
                    node = node.Next;
                    i++;
                }
                return arr;
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult ViewOfficials()
        {
            try
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
                        Permissions[] pers = persListToArray(res.getValue()[name]);
                        if (pers.Contains(Permissions.AllPermissions))
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult RemoveManager(OfficialsModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult RemoveOwner(OfficialsModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public ConcurrentLinkedList<int> changeListType(ConcurrentLinkedList<Permissions> pers)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult AddManagerPermission(OfficialsModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public int stringToEnum(string pred)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return -1;
        }

        public IActionResult EditSalePredicates(StoreModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        public IActionResult AddSale()
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        public int categorytoenum(string pred)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return -1;
        }
        public IActionResult TryAddSale(SalesModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        private void TryAllSales()
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

        }

        public IActionResult TryAddSaleCondition(SalesModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult TryRemoveSalePredicate(SalesModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public int saleStringToEnum(string pred)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return -1;
        }
        
        public IActionResult ComposeSalePredicates(SalesModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public int stringToEnumPT(string pred)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return -1;
        }
        
        public IActionResult TryRemovePurchaseType(PurchaseTypesModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult TryAddPurchaseType(PurchaseTypesModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        public IActionResult PurchasePredicate(StoreModel model)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        public IActionResult TryRemovePurchasePredicate(PredicateModel model)
        {
            try
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
                    TempData["alert"] = res.getMessage();
                    return RedirectToAction("PurchasePredicate");
                }
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult TryAddPurchasePredicate(PredicateModel model)
        {
            try
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
                        TempData["alert"] = res.getMessage();
                        return RedirectToAction("PurchasePredicate");
                    }
                }
                else
                {
                    return RedirectToAction("PurchasePredicate");
                }
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public int composePurchasePredicateStringToEnum(string pred)
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return -1;
        }
        
        public IActionResult TryComposePurchasePredicate(PredicateModel model)
        {
            try
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
                    TempData["alert"] = res.getMessage();
                    return RedirectToAction("PurchasePredicate");
                }
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult TryPriceAftereSale(ShoppingCartModel model)
        {
            try
            {
                SystemController systemController = SystemController.Instance;
                string userName = HttpContext.Session.GetString(SessionName);
                ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>>
                    pricesAfterSale = systemController.getItemsAfterSalePrices(userName);
                ConcurrentDictionary<int, int> itemsQuantities;
                double finalPriceAfterSale = 0;
                foreach (KeyValuePair<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>> dict in
                    pricesAfterSale.getValue())
                {
                    itemsQuantities = systemController.bagItemsQuantities(userName, dict.Key).getValue();
                    foreach (KeyValuePair<int, KeyValuePair<double, PriceStatus>> itemPrice in dict.Value)
                    {
                        finalPriceAfterSale += itemPrice.Value.Key * itemsQuantities[itemPrice.Key];
                    }
                }

                HttpContext.Session.SetObject("finalPriceAfterSale", finalPriceAfterSale);
                return RedirectToAction("TryPriceAftereSale");
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult TryPriceBeforeSale(ShoppingCartModel model)
        {
            try
            {
                SystemController systemController = SystemController.Instance;
                string userName = HttpContext.Session.GetString(SessionName);
                ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>>
                    pricesBeforeSale = systemController.getItemsBeforeSalePrices(userName);
                ConcurrentDictionary<int, int> itemsQuantities;
                double finalPriceBeforeSale = 0;
                foreach (KeyValuePair<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>> dict in
                    pricesBeforeSale.getValue())
                {
                    itemsQuantities = systemController.bagItemsQuantities(userName, dict.Key).getValue();
                    foreach (KeyValuePair<int, KeyValuePair<double, PriceStatus>> itemPrice in dict.Value)
                    {
                        finalPriceBeforeSale += itemPrice.Value.Key * itemsQuantities[itemPrice.Key];
                    }
                }

                HttpContext.Session.SetObject("finalPriceBeforeSale", finalPriceBeforeSale);
                return RedirectToAction("TryPriceBeforeSale");
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult GetStoreInformation(StoreModel model)
        {
            try
            {
                model.storeID = int.Parse(model.storeInfo.Split(",")[0].Substring(10));
                HttpContext.Session.SetInt32(SessionStoreID, model.storeID);
                return View();
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult AddPredicate()
        {
            try
            {
                return View();
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult AddSaleCondition()
        {
            try
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
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult EditPurchasePredicates()
        {
            try
            {
                return View();
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }

        public IActionResult BidsReview(StoreModel model)
        {
            try
            {
                if (model.storeInfo != null)
                {
                    model.storeID = int.Parse(model.storeInfo.Split(",")[0].Substring(10));
                    HttpContext.Session.SetInt32(SessionStoreID, model.storeID);
                }
                string userName = HttpContext.Session.GetString(SessionName);
                int storeID = (int)HttpContext.Session.GetInt32(SessionStoreID);
                LinkedList<SellerPermissions> storeFounderPermissions =
                    UserRepository.Instance.findUserByUserName(userName).getValue().sellerPermissions;
                SellerPermissions per = null;
                foreach (var perm in storeFounderPermissions)
                {
                    if (perm.StoreID == storeID)
                    {
                        per = perm;
                        break;
                    }
                }

                BidInfo[] bids = per.bids.Values.ToArray();
                string[] bidsstr = new string[bids.Length];
                for (int i = 0; i < bids.Length; i++)
                {
                    bidsstr[i] = bids[i].ToString();
                }
                HttpContext.Session.SetObject("bids", bidsstr);
                return View();
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult ChangeOffer(ShoppingCartModel model)
        {
            try
            {
                SystemController systemController = SystemController.Instance;
                string userName = HttpContext.Session.GetString(SessionName);
                int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
                string itemID = model.itemID;
                if (itemID != null && itemID != "")
                {
                    string[] strs = itemID.Split(":");
                    int item = int.Parse(strs[strs.Length - 1]);
                    ResultWithValue<NotificationDTO> res = systemController.submitPriceOffer(userName, (int) storeID, item, model.newOffer);
                    if (res.getValue() != null)
                    {
                        Node<string> node = res.getValue().usersToSend.First;
                        while (node.Next != null)
                        {
                            SendToSpecificUser(node.Value, res.getValue().msgToSend);
                            systemController.removeBidOffer(node.Value, (int)storeID, item, userName);
                            systemController.addBidOffer(node.Value, (int) storeID, item, userName, model.newOffer);
                            node = node.Next;
                        }
                    }
                }
                return RedirectToAction("StoreActions");
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult DiclineBid(BidsModel model)
        {
            try
            {
                SystemController systemController = SystemController.Instance;
                string userName = HttpContext.Session.GetString(SessionName);
                int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
                string itemID = model.bid;
                if (itemID != null && itemID != "")
                {
                    string[] strs = itemID.Split(";");
                    string[] strs2 = strs[0].Split(":");
                    int item = int.Parse(strs2[1]);
                    string[] strs5 = strs[0].Split(",");
                    string[] strs6 = strs5[0].Split("-");
                    string user = strs6[1].Trim();
                    ResultWithValue<NotificationDTO> res = systemController.confirmPriceStatus(userName, user, (int) storeID, item, 2);
                    if (res.getValue() != null)
                    {
                        systemController.removeBidOffer(userName, (int)storeID, item, user);
                        ConcurrentLinkedList<string> owners = StoreRepository.Instance.getStoreOwners((int) storeID);
                        Node<string> node = res.getValue().usersToSend.First;
                        while (node.Next != null)
                        {
                            SendToSpecificUser(node.Value, res.getValue().msgToSend);
                            node = node.Next;
                        }
                        node = owners.First;
                        while (node != null) 
                        {
                            systemController.removeBidOffer(node.Value, (int)storeID, item, user); 
                            node = node.Next;
                        }
                    }
                }
                return RedirectToAction("StoreActions");
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult ApproveBid(BidsModel model)
        {
            try
            {
                SystemController systemController = SystemController.Instance;
                string userName = HttpContext.Session.GetString(SessionName);
                int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
                string itemID = model.bid;
                if (itemID != null && itemID != "")
                {
                    string[] strs = itemID.Split(";");
                    string[] strs2 = strs[0].Split(":");
                    int item = int.Parse(strs2[1]);
                    string[] strs5 = strs[0].Split(",");
                    string[] strs6 = strs5[0].Split("-");
                    string user = strs6[1].Trim();
                    ResultWithValue<NotificationDTO> res = systemController.confirmPriceStatus( userName, user, (int) storeID, item, 0);
                    if (res.getValue() != null)
                    {
                        systemController.removeBidOffer(userName, (int)storeID, item, user);
                        Node<string> node = res.getValue().usersToSend.First;
                        while (node.Next != null)
                        {
                            SendToSpecificUser(node.Value, res.getValue().msgToSend);
                            node = node.Next;
                        }
                    }
                }
                return RedirectToAction("StoreActions");
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult CounterOffer(BidsModel model)
        {
            try
            {
                SystemController systemController = SystemController.Instance;
                string userName = HttpContext.Session.GetString(SessionName);
                int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
                string itemID = model.bid;
                if (itemID != null && itemID != "")
                {
                    string[] strs = itemID.Split(";");
                    string[] strs2 = strs[0].Split(":");
                    int item = int.Parse(strs2[1]);
                    string[] strs3 = strs[1].Split("!");
                    string[] strs4 = strs3[0].Split(":");
                    string[] strs5 = strs[0].Split(",");
                    string[] strs6 = strs5[0].Split("-");
                    string user = strs6[1].Trim();
                    ResultWithValue<NotificationDTO> res = systemController.itemCounterOffer(userName,user, (int) storeID, item, Double.Parse(model.newBid));
                    if (res.getValue() != null)
                    {
                        Node<string> node = res.getValue().usersToSend.First;
                        while (node.Next != null)
                        {
                            SendToSpecificUser(node.Value, res.getValue().msgToSend);
                            node = node.Next;
                        }
                    }
                    ConcurrentLinkedList<string> storeOwners = StoreRepository.Instance.getStoreOwners((int)storeID);
                    Node<string> node2 = storeOwners.First;
                    while (node2.Next != null)
                    {
                        systemController.removeBidOffer(node2.Value, (int)storeID, item, user);
                        node2 = node2.Next;
                    }
                }
                return RedirectToAction("StoreActions");
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
        
        public IActionResult ChangePurchaseType(ShoppingCartModel model)
        {
            try
            {
                SystemController systemController = SystemController.Instance;
                string userName = HttpContext.Session.GetString(SessionName);
                int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
                string itemID = model.itemID;
                if (itemID != null && itemID != "")
                {
                    string[] strs = itemID.Split(":");
                    int item = int.Parse(strs[strs.Length - 1]);
                    RegularResult res = systemController.changeItemPurchaseType(userName, (int) storeID, item, stringToEnumPT(model.purchaseType), model.newOffer);
                    if (res.getTag())
                    {
                        return RedirectToAction("StoreActions");
                    }
                    TempData["alert"] = res.getMessage();
                    return RedirectToAction("StoreActions");
                }
                return RedirectToAction("StoreActions");
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }

            return null;
        }
    }
}