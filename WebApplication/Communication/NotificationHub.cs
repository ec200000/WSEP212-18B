using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using WSEP212.DomainLayer;

namespace WebApplication.Communication
{
    public class NotificationHub : Hub
    {
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
        
        public async void SendToSpecificUser(String userName, String msg)
        {
            var connections = UserConnectionManager.Instance.GetUserConnections(userName);
            if (connections != null && connections.Count > 0) //the user is logged in
            {
                foreach (var connectionId in connections)
                {
                    await Clients.Client(connectionId).SendAsync("sendToUser", msg);
                }
            }
            else
            {
                UserConnectionManager.Instance.KeepNotification(userName,msg);
            }
        }
        
        public async Task Send(string message)
        {
            await Clients
                .Caller
                .SendAsync("OnMessageRecieved", message);
        }
    }
}