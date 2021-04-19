using System;
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

        public RegularResult register(string userName, string password) //the result will be held in the ThreadParameters Object
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (userRes.getTag()) //found user
                {
                    return new Failure($"Cannot register with the user name: {userName}, it is already in the system!");
                }

                Object[] paramsList = { userName, password };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                User newUser = new User(userName);
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
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In Register function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult login(string userName, string password)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = { userName, password };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().login, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the login action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In Login function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult logout(string userName)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = { userName };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().logout, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the logout action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In Logout function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult addItemToShoppingCart(string userName, int storeID, int itemID, int quantity)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = { storeID, itemID, quantity };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().addItemToShoppingCart, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the addItemToShoppingCart action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In AddItemToShoppingCart function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult removeItemFromShoppingCart(string userName, int storeID, int itemID)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = { storeID, itemID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().removeItemFromShoppingCart, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the removeItemFromShoppingCart action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In RemoveItemFromShoppingCart function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult purchaseItems(String userName, String address)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = { address };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                User user = userRes.getValue();
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
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In PurchaseItems function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public ResultWithValue<int> openStore(String userName, String storeName, String storeAddress, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if(!userRes.getTag())
                {
                    return new FailureWithValue<int>(userRes.getMessage(), -1);
                }

                Object[] paramsList = { storeName, storeAddress, purchasePolicy, salesPolicy };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().openStore, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the openStore action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (ResultWithValue<int>)threadParameters.result;
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In OpenStore function, the error is: {e.Message}");
                return new FailureWithValue<int>(e.Message,-1);
            }
        }

        public RegularResult itemReview(string userName, string review, int itemID, int storeID)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = { review, itemID, storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().itemReview, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the itemReview action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In ItemReview function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public ResultWithValue<int> addItemToStorage(string userName, int storeID, int quantity, String itemName, String description, double price, String category)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new FailureWithValue<int>(userRes.getMessage(), -1);
                }

                Object[] paramsList = { storeID, quantity, itemName, description, price, category };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().addItemToStorage, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the addItemToStorage action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (ResultWithValue<int>)threadParameters.result;
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In AddItemToStorage function, the error is: {e.Message}");
                return new FailureWithValue<int>(e.Message,-1);
            }
        }

        public RegularResult removeItemFromStorage(string userName, int storeID, int itemID)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = { storeID, itemID};
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().removeItemFromStorage, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the removeItemFromStorage action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In RemoveItemFromStorage function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult editItemDetails(string userName, int storeID, int itemID, int quantity, String itemName, String description, double price, String category)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = {storeID, itemID, quantity, itemName, description, price, category};
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().editItemDetails, threadParameters); //creating the job
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
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In EditItemDetails function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult appointStoreManager(string userName, string managerName, int storeID)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = { managerName, storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().appointStoreManager, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the appointStoreManager action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In AppointStoreManager function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult appointStoreOwner(string userName, string storeOwnerName, int storeID)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = { storeOwnerName, storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().appointStoreOwner, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the appointStoreOwner action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In AppointStoreOwner function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult editManagerPermissions(string userName, string managerName, ConcurrentLinkedList<Permissions> permissions, int storeID)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = { managerName, permissions, storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().editManagerPermissions, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the editManagerPermissions action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In EditManagerPermissions function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult removeStoreManager(string userName, string managerName, int storeID)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = { managerName, storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().removeStoreManager, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the removeStoreManager action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In RemoveStoreManager function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public ResultWithValue<ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>> getOfficialsInformation(string userName, int storeID)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new FailureWithValue<ConcurrentDictionary<string, ConcurrentLinkedList<Permissions>>>(userRes.getMessage(), null);
                }

                Object[] paramsList = { storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().getOfficialsInformation, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the getOfficialsInformation action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return new OkWithValue<ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>>("Get Officials Information Successfully", (ConcurrentDictionary<string, ConcurrentLinkedList<Permissions>>)threadParameters.result);
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In GetOfficialsInformation function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentDictionary<string, ConcurrentLinkedList<Permissions>>>(e.Message, null);
            }
        }

        public ResultWithValue<ConcurrentBag<PurchaseInfo>> getStorePurchaseHistory(string userName, int storeID)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new FailureWithValue<ConcurrentBag<PurchaseInfo>>(userRes.getMessage(), null);
                }

                Object[] paramsList = { storeID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().getStorePurchaseHistory, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the getStorePurchaseHistory action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                if (threadParameters.result == null)
                {
                    return new FailureWithValue<ConcurrentBag<PurchaseInfo>>("Cannot perform this action!", null);
                }
                return new OkWithValue<ConcurrentBag<PurchaseInfo>>("Get Store Purchase History Successfully", (ConcurrentBag<PurchaseInfo>)threadParameters.result);
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In GetStorePurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentBag<PurchaseInfo>>(e.Message, null);
            }
        }

        public ResultWithValue<ConcurrentDictionary<string, ConcurrentBag<PurchaseInfo>>> getUsersPurchaseHistory(String userName)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new FailureWithValue<ConcurrentDictionary<string, ConcurrentBag<PurchaseInfo>>>(userRes.getMessage(), null);
                }

                ThreadParameters threadParameters = new ThreadParameters();
                ThreadPool.QueueUserWorkItem(userRes.getValue().getUsersPurchaseHistory, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the getUsersPurchaseHistory action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return new OkWithValue<ConcurrentDictionary<string, ConcurrentBag<PurchaseInfo>>>("Get Users Purchase History Successfully", (ConcurrentDictionary<string, ConcurrentBag<PurchaseInfo>>)threadParameters.result);
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In GetUsersPurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentDictionary<string, ConcurrentBag<PurchaseInfo>>>(e.Message, null);
            }
        }

        public ResultWithValue<ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>>> getStoresPurchaseHistory(String userName)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new FailureWithValue<ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>>>(userRes.getMessage(), null);
                }

                ThreadParameters threadParameters = new ThreadParameters();
                ThreadPool.QueueUserWorkItem(userRes.getValue().getStoresPurchaseHistory, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the getStoresPurchaseHistory action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return new OkWithValue<ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>>>("Get Stores Purchase History Successfully", (ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>>)threadParameters.result);
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In GetStoresPurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>>>(e.Message, null);
            }
        }

        public RegularResult loginAsSystemManager(string userName, string password)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new Failure(userRes.getMessage());
                }

                Object[] paramsList = { userName, password };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(userRes.getValue().loginAsSystemManager, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the login action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e)
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
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In ViewShoppingCart function, the error is: {e.Message}");
                return new FailureWithValue<ShoppingCart>(e.Message, null);
            }
        }

        public ResultWithValue<ConcurrentBag<PurchaseInfo>> getUserPurchaseHistory(string userName)
        {
            try
            {
                return UserRepository.Instance.getUserPurchaseHistory(userName);
            }
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In GetUserPurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentBag<PurchaseInfo>>(e.Message, null);
            }
            
        }
        
        public ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> getItemsInStoresInformation()
        {
            try
            {
                return StoreRepository.Instance.getStoresAndItemsInfo();
            }
            catch (Exception e)
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
            catch (Exception e)
            {
                Logger.Instance.writeErrorEventToLog($"In SearchItems function, the error is: {e.Message}");
                return null;
            }
        }
    }
}
