using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Models;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;

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
        public IActionResult Index()  
        {
            HttpContext.Session.SetString(SessionName, "");
            HttpContext.Session.SetInt32(SessionLogin, 0);
            return View();  
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
        
        public IActionResult TryLogin(UserModel model)
        {
            SystemController systemController = SystemController.Instance;
            RegularResult res = systemController.login(model.UserName, model.Password);
            if (res.getTag())
            {
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}