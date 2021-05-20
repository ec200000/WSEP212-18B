using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer.SalePolicy.SaleOn
{
    public interface ApplySaleOn
    {
        public String saleOnName();
        public bool shouldApplySale(Item item);
    }
}
