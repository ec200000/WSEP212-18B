using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212.DomainLayer.ExternalPaymentSystem
{
    public class PaymentParameters
    {
        public string cardNumber { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string holder { get; set; }
        public string ccv { get; set; }
        public string id { get; set; }

        public PaymentParameters(string cardNumber, string month, string year, string holder, string ccv, string id)
        {
            this.cardNumber = cardNumber;
            this.month = month;
            this.year = year;
            this.holder = holder;
            this.ccv = ccv;
            this.id = id;
        }

        public PaymentParameters(PaymentParametersDTO paymentParametersDTO)
        {
            this.cardNumber = paymentParametersDTO.cardNumber;
            this.month = paymentParametersDTO.month;
            this.year = paymentParametersDTO.year;
            this.holder = paymentParametersDTO.holder;
            this.ccv = paymentParametersDTO.ccv;
            this.id = paymentParametersDTO.id;
        }
    }
}
