using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSEP212.DomainLayer.ExternalPaymentSystem
{
    public class PaymentSystemAdapter : PaymentInterface
    {
        //singelton
        private static readonly Lazy<PaymentSystemAdapter> lazy
       = new Lazy<PaymentSystemAdapter>(() => new PaymentSystemAdapter());

        public static PaymentSystemAdapter Instance
            => lazy.Value;

        private PaymentSystemAPI paymentSystemAPI { get; set; }

        private PaymentSystemAdapter() 
        {
            this.paymentSystemAPI = PaymentSystemAPI.Instance;
        }

        public int paymentCharge(string cardNumber, string month, string year, string holder, string ccv, string id, double price)
        {
            if(!PaymentInterface.mock)
                return this.paymentSystemAPI.paymentChargeAsync(cardNumber, month, year, holder, ccv, id, price).Result;
            else
            {
                return BadPaymentSystemMock.Instance.paymentCharge(cardNumber,month,year,holder,ccv,id,price);
            }
        }

        public bool cancelPaymentCharge(int transactionID)
        {
            if(!PaymentInterface.mock)
                return this.paymentSystemAPI.cancelPaymentChargeAsync(transactionID).Result;
            else
            {
                return BadPaymentSystemMock.Instance.cancelPaymentCharge(transactionID);
            }
        }
    }
}
