using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSEP212.ServiceLayer.ServiceObjectsDTO
{
    public class PaymentParametersDTO
    {
        public string cardNumber { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string holder { get; set; }
        public string ccv { get; set; }
        public string id { get; set; }

        public PaymentParametersDTO(string cardNumber, string month, string year, string holder, string ccv, string id)
        {
            this.cardNumber = cardNumber;
            this.month = month;
            this.year = year;
            this.holder = holder;
            this.ccv = ccv;
            this.id = id;
        }
    }
}
