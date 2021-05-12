using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer.SalePolicy.SaleOn
{
    public class SaleOnCategory : ApplySaleOn
    {
        public String category { get; set; }

        public SaleOnCategory(String category)
        {
            this.category = category;
        }

        public String saleOnName()
        {
            return "The Category " + category;
        }

        public bool shouldApplySale(Item item)
        {
            return item.category.Equals(this.category);
        }
    }
}
