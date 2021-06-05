using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSEP212.ServiceLayer.ServiceObjectsDTO
{
    public class DeliveryParametersDTO
    {
        public string sendToName { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string zip { get; set; }

        public DeliveryParametersDTO(string sendToName, string address, string city, string country, string zip)
        {
            this.sendToName = sendToName;
            this.address = address;
            this.city = city;
            this.country = country;
            this.zip = zip;
        }
    }
}
