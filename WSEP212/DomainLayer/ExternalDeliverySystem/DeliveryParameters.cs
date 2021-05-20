using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212.DomainLayer.ExternalDeliverySystem
{
    public class DeliveryParameters
    {
        public string sendToName { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string zip { get; set; }

        public DeliveryParameters(string sendToName, string address, string city, string country, string zip)
        {
            this.sendToName = sendToName;
            this.address = address;
            this.city = city;
            this.country = country;
            this.zip = zip;
        }

        public DeliveryParameters(DeliveryParametersDTO deliveryParametersDTO)
        {
            this.sendToName = deliveryParametersDTO.sendToName;
            this.address = deliveryParametersDTO.address;
            this.city = deliveryParametersDTO.city;
            this.country = deliveryParametersDTO.country;
            this.zip = deliveryParametersDTO.zip;
        }
    }
}
