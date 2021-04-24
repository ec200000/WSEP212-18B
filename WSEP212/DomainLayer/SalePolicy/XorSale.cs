using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class XorSale : Sale
    {
        public Sale firstSale { get; set; }
        public Sale secondSale { get; set; }
        public 

        public XorSale()
        {

        }

        public double applySaleOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            throw new NotImplementedException();
        }
    }
}
