using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.ExternalDeliverySystem
{
    public interface DeliveryInterface
    {
        public static bool mock = false;
        public int deliverItems(string sendToName, string address, string city, string country, string zip);
        public bool cancelDelivery(int transactionID);
    }
}
