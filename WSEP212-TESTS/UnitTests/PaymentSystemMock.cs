using System;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS
{
    public class PaymentSystemMock : PaymentInterface
    {
        //singelton
        private static readonly Lazy<PaymentSystem> lazy
            = new Lazy<PaymentSystem>(() => new PaymentSystem());

        public static PaymentSystem Instance
            => lazy.Value;

        public double paymentCharge(double price)
        {
            return 0; //charging in the listed price
        }
    }
}