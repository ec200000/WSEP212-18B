using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer.SalePolicy.SaleOn
{
    public class SaleOnCategory : ApplySaleOn
    {
        public ItemCategory category { get; set; }

        public SaleOnCategory(ItemCategory category)
        {
            this.category = category;
        }

        public String saleOnName()
        {
            return "The Category " + category.ToString();
        }

        public bool shouldApplySale(Item item)
        {
            return item.category == this.category;
        }
    }
}
