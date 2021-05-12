using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.DomainLayer;

namespace WSEP212_TEST.UnitTests.UnitTestMocks
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
            return price;
        }
    }
}
