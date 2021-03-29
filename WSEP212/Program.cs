using System;
using WSEP212.DomainLayer;

namespace WSEP212
{
    class Program
    {
        static void Main(string[] args)
        {
            Store shoppingBagStore;
            Item storeItem;
            ShoppingBag shoppingBag;

            shoppingBagStore = new Store(new SalesPolicy(), new PurchasePolicy(), new User("admin"));
            Item item = new Item(500, "black masks", "protects against infection of covid-19", 10, "health");
            storeItem = item;
            shoppingBagStore.addItemToStorage(item);

            StoreRepository.getInstance().addStore(shoppingBagStore);

            shoppingBag = new ShoppingBag(shoppingBagStore);
            Console.WriteLine(shoppingBag.isEmpty());
        }
    }
}
