using System.Collections.Concurrent;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ConcurrentLinkedList;

namespace WSEP212.ServiceLayer.ServiceObjectsDTO
{
    public class NotificationDTO
    {
        public ConcurrentLinkedList<string> usersToSend;
        public string msgToSend;

        public NotificationDTO(ConcurrentLinkedList<string> usersToSend, string msgToSend)
        {
            this.usersToSend = usersToSend;
            this.msgToSend = msgToSend;
        }
    }
}