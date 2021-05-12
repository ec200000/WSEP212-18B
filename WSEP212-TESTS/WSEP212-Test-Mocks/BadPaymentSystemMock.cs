using System;
using WSEP212.DomainLayer.ExternalPaymentSystem;

namespace WSEP212_TEST.UnitTests.UnitTestMocks
{
    public class BadPaymentSystemMock : PaymentInterface
    {
        //singelton
        private static readonly Lazy<BadPaymentSystemMock> lazy
            = new Lazy<BadPaymentSystemMock>(() => new BadPaymentSystemMock());

        public static BadPaymentSystemMock Instance
            => lazy.Value;

        public double paymentCharge(double price)
        {
            return 30000; //charging in the listed price
        }
    }
}