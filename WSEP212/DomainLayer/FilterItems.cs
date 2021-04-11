using System;
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

        public void filterItems(ConcurrentLinkedList<Item> items)
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

        private void filterItemsByPriceRange(ConcurrentLinkedList<Item> items)
        {
            Node<Item> itemNode = items.First;
            while (itemNode.Value != null)
            {
                if (itemNode.Value.price < minPrice)
                {
                    items.Remove(itemNode.Value, out _);
                }
                else if(itemNode.Value.price > maxPrice)
                {
                    items.Remove(itemNode.Value, out _);
                }
                itemNode = itemNode.Next;
            }
        }

        private void filterItemsByCategory(ConcurrentLinkedList<Item> items)
        {
            Node<Item> itemNode = items.First;
            while (itemNode.Value != null)
            {
                if (!itemNode.Value.category.Equals(category))
                {
                    items.Remove(itemNode.Value, out _);
                }
                itemNode = itemNode.Next;
            }
        }
    }
}
