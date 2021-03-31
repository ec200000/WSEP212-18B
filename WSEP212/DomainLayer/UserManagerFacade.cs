using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WSEP212.DomainLayer
{
    public class UserManagerFacade : IUserManagerFacade
    {
        private static readonly Lazy<UserManagerFacade> lazy
        = new Lazy<UserManagerFacade>(() => new UserManagerFacade());

        public static UserManagerFacade Instance
            => lazy.Value;

        private UserManagerFacade() { }

        public bool register(string userName, string password) //the result will be held in the ThreadParameters Object
        {
            Object[] paramsList = { userName, password};
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.register, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if(typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool login(string userName, string password)
        {
            Object[] paramsList = { userName, password };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.login, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool logout(string userName)
        {
            Object[] paramsList = { userName};
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.logout, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool addItemToShoppingCart(string userName, int storeID, int itemID)
        {
            Object[] paramsList = { userName, storeID, itemID };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.addItemToShoppingCart, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool removeItemFromShoppingCart(string userName, int storeID, int itemID)
        {
            Object[] paramsList = { userName, storeID, itemID };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.removeItemFromShoppingCart, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool purchaseItems(string userName)
        {
            Object[] paramsList = { userName };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.purchaseItems, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool openStore(string userName, string storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            Object[] paramsList = { userName, storeName, purchasePolicy, salesPolicy };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.openStore, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool itemReview(string userName, string review, int itemID, int storeID)
        {
            Object[] paramsList = { userName, review, itemID, storeID };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.itemReview, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool addItemToStorage(string userName, int storeID, Item item, int quantity)
        {
            Object[] paramsList = { userName, storeID, item, quantity };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.addItemToStorage, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool removeItemFromStorage(string userName, int storeID, Item item)
        {
            Object[] paramsList = { userName, storeID, item};
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.removeItemFromStorage, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool editItemDetails(string userName, int storeID, Item item)
        {
            Object[] paramsList = { userName, storeID, item };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.editItemDetails, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool appointStoreManager(string userName, string managerName, int storeID)
        {
            Object[] paramsList = { userName, managerName, storeID };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.appointStoreManager, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool appointStoreOwner(string userName, string storeOwnerName, int storeID)
        {
            Object[] paramsList = { userName, storeOwnerName, storeID };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.appointStoreOwner, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool editManagerPermissions(string userName, string managerName, ConcurrentBag<Permissions> permissions, int storeID)
        {
            Object[] paramsList = { userName, managerName, permissions, storeID };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.editManagerPermissions, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public bool removeStoreManager(string userName, string managerName, int storeID)
        {
            Object[] paramsList = { userName, managerName, storeID };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.removeStoreManager, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (bool)threadParameters.result;
        }

        public ConcurrentDictionary<string, ConcurrentBag<Permissions>> getOfficialsInformation(string userName, int storeID)
        {
            Object[] paramsList = { userName, storeID };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.getOfficialsInformation, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (ConcurrentDictionary<string, ConcurrentBag<Permissions>>)threadParameters.result;
        }

        public ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(string userName, int storeID)
        {
            Object[] paramsList = { userName, storeID };
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadParameters threadParameters = new ThreadParameters();
            threadParameters.parameters = paramsList;
            ThreadPool.QueueUserWorkItem(user.getStorePurchaseHistory, threadParameters); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (ConcurrentBag<PurchaseInfo>)threadParameters.result;
        }

        public ConcurrentDictionary<string, ConcurrentBag<PurchaseInfo>> getUsersPurchaseHistory(String userName)
        {
            ThreadParameters threadParameters = new ThreadParameters();
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadPool.QueueUserWorkItem(user.getUsersPurchaseHistory); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (ConcurrentDictionary<string, ConcurrentBag<PurchaseInfo>>)threadParameters.result;
        }

        public ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getStoresPurchaseHistory(String userName)
        {
            ThreadParameters threadParameters = new ThreadParameters();
            User user = UserRepository.Instance.findUserByUserName(userName);
            ThreadPool.QueueUserWorkItem(user.getStoresPurchaseHistory); //creating the job
            threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
            if (typeof(NotImplementedException).IsInstanceOfType(threadParameters.result))
            {
                throw new NotImplementedException(); //there is no permission to perform this task
            }
            return (ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>>)threadParameters.result;
        }
    }
}
