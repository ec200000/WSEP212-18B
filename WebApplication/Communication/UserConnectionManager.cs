using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WSEP212.DomainLayer;

namespace WebApplication.Communication
{
    public class UserConnectionManager : IUserConnectionManager
    {
        private static Dictionary<string, List<string>> userConnectionMap;
        private static ConcurrentDictionary<string, List<string>> delayedNotifications;
        private static string userConnectionMapLocker = string.Empty;
        private static string delayedNotificationsLock = string.Empty;
        
        private static readonly Lazy<UserConnectionManager> lazy
            = new Lazy<UserConnectionManager>(() => new UserConnectionManager());

        public static UserConnectionManager Instance
            => lazy.Value;

        private UserConnectionManager()
        {
            userConnectionMap = new Dictionary<string, List<string>>();
            delayedNotifications = new ConcurrentDictionary<string, List<string>>();
        }
        public void KeepUserConnection(string userId, string connectionId)
        {
            lock (userConnectionMapLocker)
            {
                if (!userConnectionMap.ContainsKey(userId))
                {
                    userConnectionMap[userId] = new List<string>();
                }
                userConnectionMap[userId].Add(connectionId);
            }
        }
        public void RemoveUserConnection(string connectionId)
        {
            //This method will remove the connectionId of user
            lock (userConnectionMapLocker)
            {
                foreach (var userId in userConnectionMap.Keys)
                {
                    if (userConnectionMap.ContainsKey(userId))
                    {
                        if (userConnectionMap[userId].Contains(connectionId))
                        {
                            userConnectionMap[userId].Remove(connectionId);
                            break;
                        }
                    }
                }
            }
        }
        public List<string> GetUserConnections(string userId)
        {
            var conn = new List<string>();
            lock (userConnectionMapLocker)
            {
                conn = userConnectionMap[userId];
            }
            return conn;
        }

        public void KeepNotification(string userName, string msg)
        {
            lock (delayedNotificationsLock)
            {
                if (!delayedNotifications.ContainsKey(userName))
                    delayedNotifications[userName] = new List<string>();
                delayedNotifications[userName].Add(msg);
            }
        }

        public List<string> GetUserDelayedNotifications(string userName)
        {
            lock (delayedNotificationsLock)
            {
                if(delayedNotifications.ContainsKey(userName))
                    return delayedNotifications[userName];
                return null;
            }
        }
    }
}