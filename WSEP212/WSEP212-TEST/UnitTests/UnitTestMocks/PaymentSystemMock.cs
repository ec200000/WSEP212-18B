using System;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS
{
    public class PaymentSystemMock : PaymentInterface
    {
        //singelton
        private static readonly Lazy<PaymentSystemMock> lazy
            = new Lazy<PaymentSystemMock>(() => new PaymentSystemMock());

        public static PaymentSystemMock Instance
            => lazy.Value;

        public double paymentCharge(double price)
        {
            return 30000; //charging in the listed price
        }
    }
}