﻿using System;
using System.Collections.Concurrent;
using WSEP212.ConcurrentLinkedList;
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
using ShoppingCart = WebApplication.Models.ShoppingCartModel;

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

        private User user;
        
        public IActionResult Index()  
        {
            HttpContext.Session.SetString(SessionName, "");
            HttpContext.Session.SetInt32(SessionLogin, 0);
            return View();  
        }  

        public IActionResult ItemReview()
        {
            return View();
        }

        public IActionResult ShoppingCart(ShoppingCartModel model)
        {
            SystemController systemController = SystemController.Instance;
            WSEP212.DomainLayer.ShoppingCart res = systemController.viewShoppingCart(model.UserName).getValue();
            ShoppingCartItems(res);
            return View();
        }
        private void ShoppingCartItems(WSEP212.DomainLayer.ShoppingCart shoppingCart)
        {
            if (shoppingCart == null)
            {
                HttpContext.Session.SetObject("shoppingCart", new string[]{""});
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
                    storesAndItems.AddLast(storeAndItem+item);
                }
            }

            string[] strs = storesAndItems.ToArray();
            HttpContext.Session.SetObject("shoppingCart", storesAndItems);
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
        
        public IActionResult ItemActions(StoreModel model)
        {
            SystemController systemController = SystemController.Instance;
            ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> res = systemController.getItemsInStoresInformation();
            checkStoresItems(model.storeID, res);
            return View();
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

        public IActionResult TryReviewItem(ReviewModel model)
        {
            SystemController systemController = SystemController.Instance;
            RegularResult res = systemController.itemReview(SessionName, model.review, model.itemID, model.storeID);
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
        
        public IActionResult TryLogout(UserModel model)
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
                new ConcurrentDictionary<string, string>(), model.price, model.category);
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
        
        public IActionResult TryRemoveItemFromShoppingCart(ItemModel model)
        {
            SystemController systemController = SystemController.Instance;
            string userName = HttpContext.Session.GetString(SessionName);
            int? storeID = HttpContext.Session.GetInt32(SessionStoreID);
            RegularResult res = systemController.removeItemFromShoppingCart(userName, (int)storeID, model.itemID);
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
        public IActionResult TrypurchaseItems(ShoppingCart model)
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

/*
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
*/
        private int[] itemListToArray(ConcurrentLinkedList<Item> lst)
        {
            int[] arr = new int[lst.size];
            int i = 0;
            Node<Item> node = lst.First; // going over the user's permissions to check if he is a store manager or owner
            while(node.Next != null)
            {
                arr[i] = node.Value.itemID;
                node = node.Next;
                i++;
            }
            return arr;
        }
        
        private void checkStoresItems(int lst, ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> dict)
        {
            foreach (Store store in dict.Keys)
            {
                if (lst == store.storeID)
                {
                    int[] items = itemListToArray(dict[store]);
                    string itms = "items" + store.storeID;
                    HttpContext.Session.SetObject(itms, items);
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}