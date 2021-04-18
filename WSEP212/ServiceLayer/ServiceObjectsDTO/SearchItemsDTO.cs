﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.ServiceLayer.ServiceObjectsDTO
{
    public class SearchItemsDTO
    {
        public String itemName { get; set; }
        public String keyWords { get; set; }
        public double minPrice { get; set; }
        public double maxPrice { get; set; }
        public String category { get; set; }

        public SearchItemsDTO(String itemName, String keyWords, double minPrice, double maxPrice, String category)
        {
            this.itemName = itemName;
            this.keyWords = keyWords;
            this.minPrice = minPrice;
            this.maxPrice = maxPrice;
            this.category = category;
        }
    }
}