using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WSEP212.ConcurrentLinkedList;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers;
namespace WebApplication.Communication
{
    public class NotificationHub : Hub
    {
        private ConcurrentDictionary<String, List<String>> usersConn = new ConcurrentDictionary<string, List<string>>();
        private readonly object listlock = new object();
        //private readonly IUserConnectionManager _userConnectionManager;
        private static int i = 0;
        /*public NotificationHub(IUserConnectionManager userConnectionManager)
        {
            _userConnectionManager = userConnectionManager;
        }*/
        public string GetConnectionId()
        {
            var httpContext = this.Context.GetHttpContext();
            //var userId = httpContext.Request.Query["userId"];
            if(i % 2 == 0) //TODO: just for now
                UserConnectionManager.Instance.KeepUserConnection("iris", Context.ConnectionId);
            if(i % 2 == 1)
                UserConnectionManager.Instance.KeepUserConnection("i", Context.ConnectionId);
            i++;
            UserConnectionManager.Instance.KeepUserConnection(Context.GetHttpContext().Session.GetString("SessionName"), Context.ConnectionId);
            return Context.ConnectionId;
        }
        //Called when a connection with the hub is terminated.
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //get the connectionId
            var connectionId = Context.ConnectionId;
            UserConnectionManager.Instance.RemoveUserConnection(connectionId);
            var value = await Task.FromResult(0);
        }
        
        public Task Send(string message)
        {
            var connection = Context.ConnectionId;
            return Clients.All.SendAsync("Send", message);
        }
        
    }
}