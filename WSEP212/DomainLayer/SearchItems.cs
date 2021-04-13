using System;
using System.Collections.Concurrent;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer;

namespace WSEP212.DomainLayer
{
    public class SearchItems
    {
        public String itemName { get; set; }
        public String keyWords { get; set; }
        public double minPrice { get; set; }
        public double maxPrice { get; set; }
        public String category { get; set; }

        public SearchItems(SearchItemsDTO searchItemsDTO)
        {
            this.itemName = searchItemsDTO.itemName;
            this.keyWords = searchItemsDTO.keyWords;
            this.minPrice = searchItemsDTO.minPrice;
            this.maxPrice = searchItemsDTO.maxPrice;
            this.category = searchItemsDTO.category;
        }

        public bool matchSearchSettings(Item item)
        {
            return (item.itemName.Contains(itemName) && item.category.Contains(category) && matchKeyWordsSetting(item)
                && item.price >= minPrice && item.price <= maxPrice);
        }

        private bool matchKeyWordsSetting(Item item)
        {
            String[] words = keyWords.Split(' ');   // split key words by space
            foreach (String word in words)
            {
                if (item.category.Contains(word) || item.description.Contains(word) || item.itemName.Contains(word) || item.category.Contains(word))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
