using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class SaleOnAllStore : ApplySaleOn
    {
        public SaleOnAllStore() { }

        public string saleOnName()
        {
            return "All The Store's Items";
        }

        public bool shouldApplySale(Item item)
        {
            return true;
        }
    }
}
