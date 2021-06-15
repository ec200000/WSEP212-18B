using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer.AuthenticationSystem;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.SystemLoggers;
using WSEP212.ServiceLayer;

namespace WSEP212.DomainLayer
{
    public class PieChart
    {
        [JsonProperty("value")]    
        private int[] pieChartData;

        private List<UserConnectionLog> logs;
        public int[] GetPieChartData()
        {
            return this.pieChartData;
        }

        public void SetPieChartData()
        {
            pieChartData = new int[5];
            pieChartData[0] = GuestsCounter();  
            pieChartData[1] = BuyerCounter();
            pieChartData[2] = ManagerCounter();
            pieChartData[3] = OwnerCounter();
            pieChartData[4] = SystemManagerCounter();
        }
        public PieChart()
        {
            logs = new List<UserConnectionLog>();
            var temp = SystemDBAccess.Instance.UserConnection;
            foreach(var a in temp.ToArray())
            {
                logs.Add(a);
            }
        }

        private UserConnectionLog contains(string username)
        {
            foreach (var log in logs)
            {
                if (log.userName.Equals(username))
                    return log;
            }

            return null;
        }
        private bool isConnectedToday(string username)
        {
            var l = contains(username);
            if (l != null)
            {
                if (l.loggedIn.Date == DateTime.Now.Date)
                {
                    return true;
                }
            }

            foreach (var user in UserRepository.Instance.users)
            {
                if (user.Key.userName.Equals(username))
                {
                    if (user.Value.Value.Date == DateTime.Now.Date)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private int GuestsCounter()
        {
            int counter = 0;
            List<string> names = new List<string>();
            foreach (var log in logs)
            {
               
                if (!Authentication.Instance.usersInfo.ContainsKey(log.userName) && log.loggedIn.Date == DateTime.Now.Date)
                {
                    counter++;
                    names.Add(log.userName); //to avoid duplications
                }
            }

            foreach (var user in UserRepository.Instance.users.Keys) //the guests are also stored there
            {
                if (!names.Contains(user.userName) && isConnectedToday(user.userName) && !Authentication.Instance.usersInfo.ContainsKey(user.userName))
                {
                    counter++;
                }
            }
            return counter;
        }
        
        private int BuyerCounter()
        {
            int counter = 0;
            foreach (var userKey in UserRepository.Instance.users.Keys)
            {
                if (isConnectedToday(userKey.userName) && !userKey.isSystemManager && Authentication.Instance.usersInfo.ContainsKey(userKey.userName) && userKey.sellerPermissions.Count == 0)
                    counter++;
            }
            return counter;
        }
        
        private int ManagerCounter()
        {
            int counter = 0;
            bool flag = false;
            foreach (var userKey in UserRepository.Instance.users.Keys)
            {
                if (isConnectedToday(userKey.userName) && Authentication.Instance.usersInfo.ContainsKey(userKey.userName))
                {
                    foreach (var perm in userKey.sellerPermissions)
                    {
                        var perms = persListToArray(perm.permissionsInStore);
                        if (perms.Contains(Permissions.AllPermissions))
                            flag = true;
                    }

                    if (!flag && userKey.sellerPermissions.Count > 0)
                        counter++;
                    flag = false;
                }
            }
            return counter;
        }
        
        private int OwnerCounter()
        {
            int counter = 0; 
            foreach (var userKey in UserRepository.Instance.users.Keys)
            {
                if (isConnectedToday(userKey.userName) && Authentication.Instance.usersInfo.ContainsKey(userKey.userName))
                {
                    foreach (var perm in userKey.sellerPermissions)
                    {
                        var perms = persListToArray(perm.permissionsInStore);
                        if (perms.Contains(Permissions.AllPermissions))
                        {
                            counter++;
                            break;
                        }
                    }
                }
            }
            return counter;
        }
        
        private int SystemManagerCounter()
        {
            int counter = 0; 
            foreach (var userKey in UserRepository.Instance.users)
            {
                if (isConnectedToday(userKey.Key.userName) && userKey.Key.isSystemManager)
                {
                    counter++;
                }
            }
            return counter;
        }
        
        private Permissions[] persListToArray(ConcurrentLinkedList<Permissions> lst)
        {
            try
            {
                Permissions[] arr = new Permissions[lst.size];
                int i = 0;
                Node<Permissions> node = lst.First;
                while(node.Next != null)
                {
                    arr[i] = node.Value;
                    node = node.Next;
                    i++;
                }
                return arr;
            }
            catch (SystemException e)
            {
                var m = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    m += inner.Message;
                Logger.Instance.writeErrorEventToLog(m);
                System.Environment.Exit(-1);
            }
            return null;
        }
    }
}