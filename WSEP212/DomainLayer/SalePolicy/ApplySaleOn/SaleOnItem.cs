using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class SaleOnItem : ApplySaleOn
    {
        public int itemID { get; set; }

        public SaleOnItem(int itemID)
        {
            this.itemID = itemID;
        }

        public String saleOnName()
        {
            return "The Item ID " + this.itemID;
        }

        public bool shouldApplySale(Item item)
        {
            return item.itemID == this.itemID;
        }
    }
}
