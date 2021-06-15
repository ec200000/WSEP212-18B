using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class UserConnectionManager : IUserConnectionManager
    {
        private static Dictionary<string, List<string>> userConnectionMap;
        private static ConcurrentDictionary<string, List<string>> delayedNotifications;
        [NotMapped]
        private static string userConnectionMapLocker = string.Empty;
        [Key]
        public string delayedNotificationsLock
        {
            get;
            set;
        }
        
        [NotMapped]
        private static string delayedlock = string.Empty;
        
        public string DelayedNotificationsAsJson
        {
            get => JsonConvert.SerializeObject(delayedNotifications);
            set => delayedNotifications = JsonConvert.DeserializeObject<ConcurrentDictionary<string, List<string>>>(value);
        }
        
        private static readonly Lazy<UserConnectionManager> lazy
            = new Lazy<UserConnectionManager>(() => new UserConnectionManager());

        public static UserConnectionManager Instance
            => lazy.Value;

        private UserConnectionManager()
        {
            delayedNotificationsLock = String.Empty;
            userConnectionMap = new Dictionary<string, List<string>>();
            delayedNotifications = new ConcurrentDictionary<string, List<string>>();
            List<UserConnectionManager> delayNot = new List<UserConnectionManager>();
            if (SystemDBAccess.Instance.DelayedNotifications.Count() > 0)
                delayNot = SystemDBAccess.Instance.DelayedNotifications.ToList();
            if (delayNot.Count > 0)
                delayedNotifications = JsonConvert.DeserializeObject<ConcurrentDictionary<string, List<string>>>(delayNot[0].DelayedNotificationsAsJson);
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

        public void RemoveUser(string userName)
        {
            lock (userConnectionMapLocker)
            {
                if (userConnectionMap.ContainsKey(userName))
                    userConnectionMap.Remove(userName);
            }
        }
        
        public List<string> GetUserConnections(string userId)
        {
            var conn = new List<string>();
            lock (userConnectionMapLocker)
            {
                if(userConnectionMap.ContainsKey(userId))
                    conn = userConnectionMap[userId];
            }
            return conn;
        }

        public void KeepNotification(string userName, string msg)
        {
            lock (delayedlock)
            {
                if (!delayedNotifications.ContainsKey(userName))
                    delayedNotifications[userName] = new List<string>();
                delayedNotifications[userName].Add(msg);
                DelayedNotificationsAsJson = JsonConvert.SerializeObject(delayedNotifications);
                var result = SystemDBAccess.Instance.DelayedNotifications.Find(string.Empty);
                if (result != null)
                {
                    if(!JToken.DeepEquals(result.DelayedNotificationsAsJson, this.DelayedNotificationsAsJson))
                        result.DelayedNotificationsAsJson = this.DelayedNotificationsAsJson;
                    lock(SystemDBAccess.savelock)
                        SystemDBAccess.Instance.SaveChanges();
                }
                else //first time - 
                {
                    this.DelayedNotificationsAsJson = JsonConvert.SerializeObject(delayedNotifications);
                    SystemDBAccess.Instance.DelayedNotifications.Add(this);
                }
            }
        }

        public List<string> GetUserDelayedNotifications(string userName)
        {
            lock (delayedlock)
            {
                if(delayedNotifications.ContainsKey(userName))
                    return delayedNotifications[userName];
                return null;
            }
        }
        
       
    }
}