using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class VisibleSale
    {
        public int salePercentageOff { get; set; }
        public DateTime saleExpireTime { get; set; }

        public VisibleSale(int salePercentageOff, DateTime saleExpireTime)
        {
            this.saleExpireTime = saleExpireTime;
            this.salePercentageOff = salePercentageOff;
        }

        // calculate the price after the sale if the sale not expired
        public double priceAfterSale(int price)
        {
            if(DateTime.Now < saleExpireTime)
            {
                return ((100 - salePercentageOff) / 100) * price;
            }
            return price;
        }

    }
}
