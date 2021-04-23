using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        private SubscribeModel model;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult Register()
        {
            return View();
        }
        
        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
        
        [HttpPost]
        public ActionResult Subscribe(SubscribeModel model)
        {
            if (ModelState.IsValid)
            {
                this.model = model;
                SystemController systemController = SystemController.Instance;
                RegularResult res = systemController.register(model.UserName, model.Password);
                if (res.getTag())
                {
                    return View("Login");
                }
                if (!res.getTag())
                {
                    ViewBag.Alert = res.getMessage();
                    return View("Register");
                }
            }
            return View("Privacy");
        }
    }
}