using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer.ExternalPaymentSystem
{
    public class PaymentSystem : PaymentInterface
    {
        //singelton
        private static readonly Lazy<PaymentSystem> lazy
       = new Lazy<PaymentSystem>(() => new PaymentSystem());

        public static PaymentSystem Instance
            => lazy.Value;
        
        private PaymentSystem() {
           
        }

        public double paymentCharge(double price)
        {
            return price; //charging in the listed price
        }
    }
}
