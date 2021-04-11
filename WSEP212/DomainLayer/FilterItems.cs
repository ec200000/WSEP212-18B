using System;
using System.Collections.Concurrent;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer;

namespace WSEP212.DomainLayer
{
    public class FilterItems
    {
        public double minPrice { get; set; }
        public double maxPrice { get; set; }
        public String category { get; set; }

        public FilterItems(FilterItemsDTO filterItemsDTO)
        {
            this.minPrice = filterItemsDTO.minPrice;
            this.maxPrice = filterItemsDTO.maxPrice;
            this.category = filterItemsDTO.category;
        }
        
        public FilterItems(double minPrice, double maxPrice, String category)
        {
            this.minPrice = minPrice;
            this.maxPrice = maxPrice;
            this.category = category;
        }

        public void filterItems(ConcurrentDictionary<Item, int> items)
        {
            if(minPrice != Double.MinValue || maxPrice != Double.MaxValue)
            {
                filterItemsByPriceRange(items);
            }
            if(category != null)
            {
                filterItemsByCategory(items);
            }
        }

        private void filterItemsByPriceRange(ConcurrentDictionary<Item, int> items)
        {
            foreach (Item item in items.Keys)
            {
                if (item.price < minPrice)
                {
                    items.TryRemove(item, out _);
                }
                else if(item.price > maxPrice)
                {
                    items.TryRemove(item, out _);
                }
            }
        }

        private void filterItemsByCategory(ConcurrentDictionary<Item, int> items)
        {
            foreach (Item item in items.Keys)
            {
                if (!item.category.Equals(category))
                {
                    items.TryRemove(item, out _);
                }
            }
        }
    }
}
