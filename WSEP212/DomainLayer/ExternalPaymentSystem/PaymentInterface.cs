using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WSEP212.DomainLayer.ExternalPaymentSystem
{
    public interface PaymentInterface
    {
        public int paymentCharge(string cardNumber, string month, string year, string holder, string ccv, string id, double price);
        public bool cancelPaymentCharge(int transactionID);
    }
}
