using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public interface PaymentInterface
    {
        public double paymentCharge(User user, double price);
    }
}
