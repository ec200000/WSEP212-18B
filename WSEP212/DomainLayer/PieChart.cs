﻿using System;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.AuthenticationSystem;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.SystemLoggers;

namespace WSEP212.DomainLayer
{
    public class PieChart
    {
        [JsonProperty("value")]    
        private int[] pieChartData;

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
        }

        private int GuestsCounter()
        {
            int counter = 0;
            foreach (var userKey in UserRepository.Instance.users.Keys)
            {
                if (!Authentication.Instance.usersInfo.ContainsKey(userKey.userName) && !userKey.sellerPermissions.Any())
                    counter++;
            }
            return counter;
        }
        
        private int BuyerCounter()
        {
            int counter = 0;
            foreach (var userKey in UserRepository.Instance.users.Keys)
            {
                if (!userKey.isSystemManager && Authentication.Instance.usersInfo.ContainsKey(userKey.userName) && userKey.sellerPermissions.Count == 0)
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
                if (Authentication.Instance.usersInfo.ContainsKey(userKey.userName))
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
                if (Authentication.Instance.usersInfo.ContainsKey(userKey.userName))
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
                if (userKey.Key.isSystemManager)
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