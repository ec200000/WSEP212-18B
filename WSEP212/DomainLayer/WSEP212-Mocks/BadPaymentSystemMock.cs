using System;
using WSEP212.DomainLayer.ExternalPaymentSystem;

namespace WSEP212.DomainLayer
{
    public class BadPaymentSystemMock : PaymentInterface
    {
        //singelton
        private static readonly Lazy<BadPaymentSystemMock> lazy
            = new Lazy<BadPaymentSystemMock>(() => new BadPaymentSystemMock());

        public static BadPaymentSystemMock Instance
            => lazy.Value;

        // bad payment system, always reject the payment 
        public int paymentCharge(string cardNumber, string month, string year, string holder, string ccv, string id, double price)
        {
            return -1;
        }

        public bool cancelPaymentCharge(int transactionID)
        {
            return false;
        }
    }
}