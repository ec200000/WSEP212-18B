using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.ServiceLayer
{
    public class FilterItemsDTO
    {
        public double minPrice { get; set; }
        public double maxPrice { get; set; }
        public String category { get; set; }

        public FilterItemsDTO(double minPrice = Double.MinValue, double maxPrice = Double.MaxValue, String category = null)
        {
            this.minPrice = minPrice;
            this.maxPrice = maxPrice;
            this.category = category;
        }
    }
}
