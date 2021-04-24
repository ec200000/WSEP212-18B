﻿using System;
using System.Collections.Concurrent;
using WSEP212.ConcurrentLinkedList;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212.DomainLayer
{
    public class SystemControllerFacade : ISystemControllerFacade
    {
        private static readonly Lazy<SystemControllerFacade> lazy
        = new Lazy<SystemControllerFacade>(() => new SystemControllerFacade());

        public static SystemControllerFacade Instance
            => lazy.Value;

        private SystemControllerFacade() { }

        public RegularResult register(string userName, int userAge, string password) //the result will be held in the ThreadParameters Object
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (userRes.getTag()) //found user
                {
                    return new Failure($"Cannot register with the user name: {userName}, it is already in the system!");
                }

                Object[] paramsList = { userName, userAge, password };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                User newUser = new User(userName, userAge);
                ThreadPool.QueueUserWorkItem(newUser.register, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if(threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the register action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In Register function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult login(string userName, string password)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }
                user = userRes.getValue();

                Object[] paramsList = { userName, password };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.login, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the login action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In Login function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult logout(string userName)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { userName };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.logout, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the logout action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In Logout function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult addItemToShoppingCart(string userName, int storeID, int itemID, int quantity)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { storeID, itemID, quantity };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.addItemToShoppingCart, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the addItemToShoppingCart action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In AddItemToShoppingCart function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult removeItemFromShoppingCart(string userName, int storeID, int itemID)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { storeID, itemID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.removeItemFromShoppingCart, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the removeItemFromShoppingCart action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In RemoveItemFromShoppingCart function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult purchaseItems(String userName, String address)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { address };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.purchaseItems, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the purchaseItems action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In PurchaseItems function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public ResultWithValue<int> openStore(String userName, String storeName, String storeAddress, PurchasePolicy purchasePolicy, SalePolicy salesPolicy)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();
                Object[] paramsList = { storeName, storeAddress, purchasePolicy, salesPolicy };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.openStore, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the openStore action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (ResultWithValue<int>)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In OpenStore function, the error is: {e.Message}");
                return new FailureWithValue<int>(e.Message, -1);
            }
        }

        public RegularResult itemReview(string userName, string review, int itemID, int storeID)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { review, itemID, storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.itemReview, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the itemReview action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In ItemReview function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public ResultWithValue<int> addItemToStorage(string userName, int storeID, int quantity, String itemName, String description, double price, String category)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { storeID, quantity, itemName, description, price, category };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.addItemToStorage, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the addItemToStorage action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (ResultWithValue<int>)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In AddItemToStorage function, the error is: {e.Message}");
                return new FailureWithValue<int>(e.Message,-1);
            }
        }

        public RegularResult removeItemFromStorage(string userName, int storeID, int itemID)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { storeID, itemID};
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.removeItemFromStorage, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the removeItemFromStorage action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In RemoveItemFromStorage function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult editItemDetails(string userName, int storeID, int itemID, int quantity, String itemName, String description, double price, String category)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = {storeID, itemID, quantity, itemName, description, price, category};
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.editItemDetails, threadParameters); //creating the job
                threadParameters.eventWaitHandle
                    .WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the editItemDetails action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }

                return (RegularResult) threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In EditItemDetails function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult appointStoreManager(string userName, string managerName, int storeID)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { managerName, storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.appointStoreManager, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the appointStoreManager action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In AppointStoreManager function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult appointStoreOwner(string userName, string storeOwnerName, int storeID)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { storeOwnerName, storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.appointStoreOwner, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the appointStoreOwner action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In AppointStoreOwner function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult editManagerPermissions(string userName, string managerName, ConcurrentLinkedList<Permissions> permissions, int storeID)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { managerName, permissions, storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.editManagerPermissions, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the editManagerPermissions action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In EditManagerPermissions function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult removeStoreManager(string userName, string managerName, int storeID)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { managerName, storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.removeStoreManager, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the removeStoreManager action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In RemoveStoreManager function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }
        
        public RegularResult removeStoreOwner(string userName, string ownerName, int storeID)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { ownerName, storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.removeStoreManager, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the removeStoreManager action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In RemoveStoreManager function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public ResultWithValue<ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>> getOfficialsInformation(string userName, int storeID)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.getOfficialsInformation, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the getOfficialsInformation action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return new OkWithValue<ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>>("Get Officials Information Successfully", (ConcurrentDictionary<string, ConcurrentLinkedList<Permissions>>)threadParameters.result);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetOfficialsInformation function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentDictionary<string, ConcurrentLinkedList<Permissions>>>(e.Message, null);
            }
        }

        public ResultWithValue<ConcurrentBag<PurchaseInvoice>> getStorePurchaseHistory(string userName, int storeID)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.getStorePurchaseHistory, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the getStorePurchaseHistory action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                if (threadParameters.result == null)
                {
                    return new FailureWithValue<ConcurrentBag<PurchaseInvoice>>("Cannot perform this action!", null);
                }
                return new OkWithValue<ConcurrentBag<PurchaseInvoice>>("Get Store Purchase History Successfully", (ConcurrentBag<PurchaseInvoice>)threadParameters.result);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetStorePurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentBag<PurchaseInvoice>>(e.Message, null);
            }
        }

        public ResultWithValue<ConcurrentDictionary<string, ConcurrentBag<PurchaseInvoice>>> getUsersPurchaseHistory(String userName)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                ThreadParameters threadParameters = new ThreadParameters();
                ThreadPool.QueueUserWorkItem(user.getUsersPurchaseHistory, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the getUsersPurchaseHistory action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return new OkWithValue<ConcurrentDictionary<string, ConcurrentBag<PurchaseInvoice>>>("Get Users Purchase History Successfully", (ConcurrentDictionary<string, ConcurrentBag<PurchaseInvoice>>)threadParameters.result);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetUsersPurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentDictionary<string, ConcurrentBag<PurchaseInvoice>>>(e.Message, null);
            }
        }

        public ResultWithValue<ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>>> getStoresPurchaseHistory(String userName)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                ThreadParameters threadParameters = new ThreadParameters();
                ThreadPool.QueueUserWorkItem(user.getStoresPurchaseHistory, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the getStoresPurchaseHistory action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return new OkWithValue<ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>>>("Get Stores Purchase History Successfully", (ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>>)threadParameters.result);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetStoresPurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>>>(e.Message, null);
            }
        }

        public RegularResult loginAsSystemManager(string userName, string password)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                    //   return new FailureWithValue<int>(userRes.getMessage(), -1);
                }
                else user = userRes.getValue();

                Object[] paramsList = { userName, password };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.loginAsSystemManager, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the login action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In LoginAsSystemManager function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public ResultWithValue<ShoppingCart> viewShoppingCart(string userName)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new FailureWithValue<ShoppingCart>(userRes.getMessage(), null);
                }

                return new OkWithValue<ShoppingCart>("Found user with shopping cart",userRes.getValue().shoppingCart);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In ViewShoppingCart function, the error is: {e.Message}");
                return new FailureWithValue<ShoppingCart>(e.Message, null);
            }
        }

        public ResultWithValue<ConcurrentBag<PurchaseInvoice>> getUserPurchaseHistory(string userName)
        {
            try
            {
                return UserRepository.Instance.getUserPurchaseHistory(userName);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetUserPurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentBag<PurchaseInvoice>>(e.Message, null);
            }
            
        }
        
        public ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> getItemsInStoresInformation()
        {
            try
            {
                return StoreRepository.Instance.getStoresAndItemsInfo();
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetItemsInStoresInformation function, the error is: {e.Message}");
                return null;
            }
            
        }

        public ConcurrentDictionary<Item, int> searchItems(SearchItemsDTO searchItemsDTO)
        {
            try
            {
                SearchItems search = new SearchItems(searchItemsDTO);
                return StoreRepository.Instance.searchItem(search);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In SearchItems function, the error is: {e.Message}");
                return null;
            }
        }
    }
}
