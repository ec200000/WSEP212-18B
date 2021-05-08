using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WSEP212.ConcurrentLinkedList;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers;
namespace WebApplication.Communication
{
    public class NotificationHub : Hub
    {
        //private ConcurrentDictionary<String, List<String>> usersConn = new ConcurrentDictionary<string, List<string>>();
        //private readonly object listlock = new object();
        //private readonly IUserConnectionManager _userConnectionManager;
        //private static int i = 0;
        /*public NotificationHub(IUserConnectionManager userConnectionManager)
        {
            _userConnectionManager = userConnectionManager;
        }*/
        public string GetConnectionId()
        {
            UserConnectionManager.Instance.KeepUserConnection(Context.GetHttpContext().Session.GetString("_Name"), Context.ConnectionId);
            return Context.ConnectionId;
        }
        
        public string GetConnectionIdWithUserName(string userName)
        {
            UserConnectionManager.Instance.KeepUserConnection(userName, Context.ConnectionId);
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
        
        public async void SendDelayedNotificationsToUser(String userName)
        {
            var delayedNot = UserConnectionManager.Instance.GetUserDelayedNotifications(userName);
            var connections = UserConnectionManager.Instance.GetUserConnections(userName);
            Thread.Sleep(1000);
            if (delayedNot != null && connections != null && connections.Count > 0)
            {
                foreach (var noti in delayedNot)
                {
                    foreach (var connectionId in connections)
                    {
                        await Clients.Client(connectionId).SendAsync("sendToUser", noti);
                    }
                }
            }
        }
    }
}