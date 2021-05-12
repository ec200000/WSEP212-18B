using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer.ExternalPaymentSystem
{
    public interface PaymentInterface
    {
        public double paymentCharge(double price);
    }
}
