using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class PurchaseInfo
    {
        public int storeID { get; set; }
        public String userName { get; set; }
        public double price { get; set; }
        public DateTime dateOfPurchase { get; set; }
    }
}
