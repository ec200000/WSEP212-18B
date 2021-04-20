using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;

namespace WebApplication.Views.Home
{
    public class StoresInfo : PageModel
    {
        public ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> stores { get; set; }
        
        public void OnGet()
        {
            SystemController systemController = SystemController.Instance;
            ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> res = systemController.getItemsInStoresInformation();
            this.stores = res;
        }
    }
}