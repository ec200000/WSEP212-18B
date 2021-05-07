using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WSEP212.ConcurrentLinkedList;
using System.Collections.Generic;

namespace WebApplication.Communication
{
    public class NotificationHub : Hub
    {
        private ConcurrentDictionary<String, List<String>> usersConn = new ConcurrentDictionary<string, List<string>>();
        private readonly object listlock = new object();

        public void SaveConnection(String name)
        {
            if (usersConn.ContainsKey(name))
                usersConn.TryAdd(name, new List<string>());
            lock (listlock)
            { 
                usersConn[name].Add(Context.ConnectionId);
            }
        }
        
        public Task Send(string message)
        {
            var connection = Context.ConnectionId;
            return Clients.All.SendAsync("Send", message);
        }
        
    }
}