using System;
using WSEP212.DomainLayer.ExternalPaymentSystem;

namespace WSEP212.DomainLayer
{
    public class PaymentSystemMock : PaymentInterface
    {
        //singelton
        private static readonly Lazy<PaymentSystemMock> lazy
            = new Lazy<PaymentSystemMock>(() => new PaymentSystemMock());

        public static PaymentSystemMock Instance
            => lazy.Value;

        private PaymentSystemMock() { }

        // returns valid transaction id
        public int paymentCharge(string cardNumber, string month, string year, string holder, string ccv, string id, double price)
        {
            return 10000;
        }

        public bool cancelPaymentCharge(int transactionID)
        {
            return true;
        }
    }
}
