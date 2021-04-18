using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class PaymentSystem : PaymentInterface
    {
        //singelton
        private static readonly Lazy<PaymentSystem> lazy
       = new Lazy<PaymentSystem>(() => new PaymentSystem());

        public static PaymentSystem Instance
            => lazy.Value;

        public double paymentCharge(double price)
        {
            return price; //charging in the listed price
        }
    }
}
