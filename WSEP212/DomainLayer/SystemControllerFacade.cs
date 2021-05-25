using System;
using System.Collections.Concurrent;
using WSEP212.ConcurrentLinkedList;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.ServiceLayer;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer.ServiceObjectsDTO;
using WSEP212.DomainLayer.SystemLoggers;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.ExternalPaymentSystem;
using WSEP212.DomainLayer.ExternalDeliverySystem;

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

                
                User newUser = new User(userName, userAge);
                Object[] paramsList = { newUser, password };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
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

        private ItemPurchaseType createItemPurchaseType(PurchaseType purchaseType, double startPrice)
        {
            ItemPurchaseType itemPurchaseType = null;
            switch(purchaseType)
            {
                case PurchaseType.ImmediatePurchase:
                    itemPurchaseType = new ItemImmediatePurchase(startPrice);
                    break;
                case PurchaseType.SubmitOfferPurchase:
                    itemPurchaseType = new ItemSubmitOfferPurchase(startPrice);
                    break;
            }
            return itemPurchaseType;
        }

        public RegularResult addItemToShoppingCart(string userName, int storeID, int itemID, int quantity, PurchaseType purchaseType, double startPrice)
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

                ItemPurchaseType itemPurchaseType = createItemPurchaseType(purchaseType, startPrice);
                Object[] paramsList = { storeID, itemID, quantity, itemPurchaseType };
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

        public RegularResult changeItemQuantityInShoppingCart(string userName, int storeID, int itemID, int updatedQuantity)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                }
                else user = userRes.getValue();

                Object[] paramsList = { storeID, itemID, updatedQuantity };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.changeItemQuantityInShoppingCart, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the changeItemQuantityInShoppingCart action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In changeItemQuantityInShoppingCart function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult changeItemPurchaseType(string userName, int storeID, int itemID, PurchaseType purchaseType, double startPrice)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                }
                else user = userRes.getValue();

                ItemPurchaseType itemPurchaseType = createItemPurchaseType(purchaseType, startPrice);
                Object[] paramsList = { storeID, itemID, itemPurchaseType };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.changeItemPurchaseType, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the changeItemPurchaseType action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In changeItemPurchaseType function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public ResultWithValue<ConcurrentLinkedList<string>> submitPriceOffer(string userName, int storeID, int itemID, double offerItemPrice)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                }
                else user = userRes.getValue();

                Object[] paramsList = { storeID, itemID, offerItemPrice };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.submitPriceOffer, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the submitPriceOffer action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (ResultWithValue<ConcurrentLinkedList<string>>)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In submitPriceOffer function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentLinkedList<string>>(e.Message, null);
            }
        }

        public ResultWithValue<string> itemCounterOffer(String storeManager, String userName, int storeID, int itemID, double counterOffer)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(storeManager);
                if (!userRes.getTag())
                {
                    user = new User(storeManager);
                    UserRepository.Instance.addLoginUser(user);
                }
                else user = userRes.getValue();

                Object[] paramsList = { userName, storeID, itemID, counterOffer };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.itemCounterOffer, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + storeManager + " cannot perform the itemCounterOffer action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (ResultWithValue<string>)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In itemCounterOffer function, the error is: {e.Message}");
                return new FailureWithValue<string>(e.Message, userName);
            }
        }

        public ResultWithValue<string> confirmPriceStatus(string storeManager, string userToConfirm, int storeID, int itemID, PriceStatus priceStatus)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(storeManager);
                if (!userRes.getTag())
                {
                    user = new User(storeManager);
                    UserRepository.Instance.addLoginUser(user);
                }
                else user = userRes.getValue();

                Object[] paramsList = { userToConfirm, storeID, itemID, priceStatus };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.confirmPriceStatus, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + storeManager + " cannot perform the confirmPriceStatus action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (ResultWithValue<string>)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In confirmPriceStatus function, the error is: {e.Message}");
                return new FailureWithValue<string>(e.Message, userToConfirm);
            }
        }

        public RegularResult supportPurchaseType(string userName, int storeID, PurchaseType purchaseType)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                }
                else user = userRes.getValue();

                Object[] paramsList = { storeID, purchaseType };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.supportPurchaseType, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the supportPurchaseType action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In supportPurchaseType function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public RegularResult unsupportPurchaseType(string userName, int storeID, PurchaseType purchaseType)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    user = new User(userName);
                    UserRepository.Instance.addLoginUser(user);
                }
                else user = userRes.getValue();

                Object[] paramsList = { storeID, purchaseType };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.unsupportPurchaseType, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the unsupportPurchaseType action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In unsupportPurchaseType function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public ResultWithValue<ConcurrentLinkedList<string>> purchaseItems(string userName, DeliveryParameters deliveryParameters, PaymentParameters paymentParameters)
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

                Object[] paramsList = { deliveryParameters, paymentParameters };
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
                return (ResultWithValue<ConcurrentLinkedList<string>>)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In PurchaseItems function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentLinkedList<string>>(e.Message, null);
            }
        }

        public ResultWithValue<int> openStore(String userName, String storeName, String storeAddress, PurchasePolicyInterface purchasePolicy, SalePolicyInterface salesPolicy)
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

        public ResultWithValue<ConcurrentLinkedList<string>> itemReview(string userName, string review, int itemID, int storeID)
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
                return (ResultWithValue<ConcurrentLinkedList<string>>)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In ItemReview function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentLinkedList<string>>(e.Message,null);
            }
        }

        public ResultWithValue<int> addItemToStorage(string userName, int storeID, int quantity, String itemName, String description, double price, ItemCategory category)
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

        public RegularResult editItemDetails(string userName, int storeID, int itemID, int quantity, String itemName, String description, double price, ItemCategory category)
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
                ThreadPool.QueueUserWorkItem(user.removeStoreOwner, threadParameters); //creating the job
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

        public ResultWithValue<int> addPurchasePredicate(string userName, int storeID, Predicate<PurchaseDetails> newPredicate, String predDescription)
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

                Object[] paramsList = { storeID, newPredicate, predDescription };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.addPurchasePredicate, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the addPurchasePredicate action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (ResultWithValue<int>)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In addPurchasePredicate function, the error is: {e.Message}");
                return new FailureWithValue<int>(e.Message, -1);
            }
        }

        public RegularResult removePurchasePredicate(string userName, int storeID, int predicateID)
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

                Object[] paramsList = { storeID, predicateID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.removePurchasePredicate, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the removePurchasePredicate action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In removePurchasePredicate function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public ResultWithValue<int> composePurchasePredicates(string userName, int storeID, int firstPredicateID, int secondPredicateID, PurchasePredicateCompositionType typeOfComposition)
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

                Object[] paramsList = { storeID, firstPredicateID, secondPredicateID, typeOfComposition };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.composePurchasePredicates, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the composePurchasePredicates action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (ResultWithValue<int>)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In composePurchasePredicates function, the error is: {e.Message}");
                return new FailureWithValue<int>(e.Message, -1);
            }
        }

        public ResultWithValue<int> addSale(string userName, int storeID, int salePercentage, ApplySaleOn saleOn, String saleDescription)
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

                Object[] paramsList = { storeID, salePercentage, saleOn, saleDescription };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.addSale, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the addSale action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (ResultWithValue<int>)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In addSale function, the error is: {e.Message}");
                return new FailureWithValue<int>(e.Message, -1);
            }
        }

        public RegularResult removeSale(string userName, int storeID, int saleID)
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

                Object[] paramsList = { storeID, saleID };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.removeSale, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the removeSale action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In removeSale function, the error is: {e.Message}");
                return new Failure(e.Message);
            }
        }

        public ResultWithValue<int> addSaleCondition(string userName, int storeID, int saleID, SimplePredicate condition, SalePredicateCompositionType compositionType)
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

                Object[] paramsList = { storeID, saleID, condition, compositionType };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.addSaleCondition, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the addSaleCondition action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (ResultWithValue<int>)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In addSaleCondition function, the error is: {e.Message}");
                return new FailureWithValue<int>(e.Message, -1);
            }
        }

        public ResultWithValue<int> composeSales(string userName, int storeID, int firstSaleID, int secondSaleID, SaleCompositionType typeOfComposition, SimplePredicate selectionRule)
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

                Object[] paramsList = { storeID, firstSaleID, secondSaleID, typeOfComposition, selectionRule };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.composeSales, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the composeSales action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (ResultWithValue<int>)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In composeSales function, the error is: {e.Message}");
                return new FailureWithValue<int>(e.Message, -1);
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

                if (threadParameters.result == null || ((ConcurrentDictionary<string, ConcurrentLinkedList<Permissions>>)threadParameters.result).IsEmpty)
                {
                    return new FailureWithValue<ConcurrentDictionary<string, ConcurrentLinkedList<Permissions>>>("No officials found", null);
                }
                return new OkWithValue<ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>>("Get Officials Information Successfully", (ConcurrentDictionary<string, ConcurrentLinkedList<Permissions>>)threadParameters.result);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetOfficialsInformation function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentDictionary<string, ConcurrentLinkedList<Permissions>>>(e.Message, null);
            }
        }

        public ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> getStorePurchaseHistory(string userName, int storeID)
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
                    return new FailureWithValue<ConcurrentDictionary<int, PurchaseInvoice>>("Cannot perform this action!", null);
                }
                return new OkWithValue<ConcurrentDictionary<int, PurchaseInvoice>>("Get Store Purchase History Successfully", (ConcurrentDictionary<int, PurchaseInvoice>)threadParameters.result);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetStorePurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentDictionary<int, PurchaseInvoice>>(e.Message, null);
            }
        }

        public ResultWithValue<ConcurrentDictionary<string, ConcurrentDictionary<int, PurchaseInvoice>>> getUsersPurchaseHistory(String userName)
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
                return new OkWithValue<ConcurrentDictionary<string, ConcurrentDictionary<int, PurchaseInvoice>>>("Get Users Purchase History Successfully", (ConcurrentDictionary<string, ConcurrentDictionary<int, PurchaseInvoice>>)threadParameters.result);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetUsersPurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentDictionary<string, ConcurrentDictionary<int, PurchaseInvoice>>>(e.Message, null);
            }
        }

        public ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>>> getStoresPurchaseHistory(String userName)
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
                return new OkWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>>>("Get Stores Purchase History Successfully", (ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>>)threadParameters.result);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetStoresPurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>>>(e.Message, null);
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
                    String errorMsg = "The user " + userName + " cannot perform the login as system manager action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    //throw new NotImplementedException(); //there is no permission to perform this task
                    return new Failure(errorMsg);
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

        public ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> getUserPurchaseHistory(string userName)
        {
            try
            {
                return UserRepository.Instance.getUserPurchaseHistory(userName);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetUserPurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentDictionary<int, PurchaseInvoice>>(e.Message, null);
            }
        }

        public ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>> getItemsAfterSalePrices(String userName)
        {
            try
            {
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (!userRes.getTag())
                {
                    return new FailureWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>>(userRes.getMessage(), null);
                }
                ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>> itemsPrices = userRes.getValue().shoppingCart.getItemsAfterSalePrices();
                return new OkWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>>("Return User Items Prices Afetr Sales", itemsPrices);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetUserPurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>>(e.Message, null);
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

        public ResultWithValue<ConcurrentLinkedList<int>> getUsersStores(String userName)
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

                Object[] paramsList = { };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.getUsersStores, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the getUsersStores action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                if (threadParameters.result == null)
                {
                    return new FailureWithValue<ConcurrentLinkedList<int>>("Cannot perform this action!", null);
                }
                return new OkWithValue<ConcurrentLinkedList<int>>("Get users store Successfully", (ConcurrentLinkedList<int>)threadParameters.result);
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In GetStorePurchaseHistory function, the error is: {e.Message}");
                return new FailureWithValue<ConcurrentLinkedList<int>>(e.Message, null);
            }
        }

        public RegularResult continueAsGuest(String userName)
        {
            try
            {
                User user;
                ResultWithValue<User> userRes = UserRepository.Instance.findUserByUserName(userName);
                if (userRes.getTag())
                {
                    return new Failure("user already exist.");
                }
                user = new User(userName);
                Object[] paramsList = { userName };
                ThreadParameters threadParameters = new ThreadParameters();
                threadParameters.parameters = paramsList;
                ThreadPool.QueueUserWorkItem(user.continueAsGuest, threadParameters); //creating the job
                threadParameters.eventWaitHandle.WaitOne(); //after this line the result will be calculated in the ThreadParameters obj(waiting for the result)
                if (threadParameters.result is NotImplementedException)
                {
                    String errorMsg = "The user " + userName + " cannot perform the continue as guest action!";
                    Logger.Instance.writeWarningEventToLog(errorMsg);
                    throw new NotImplementedException(); //there is no permission to perform this task
                }
                return (RegularResult)threadParameters.result;
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                Logger.Instance.writeErrorEventToLog($"In ContinueAsGuest function, the error is: {e.Message}");
                return new Failure("user couldn't perform action.");
            }
        }

        public ResultWithValue<ConcurrentDictionary<int, string>> getStorePredicatesDescription(int storeID)
        {
            Store store;
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new FailureWithValue<ConcurrentDictionary<int, string>>("store not exist", null);
            }
            store = storeRes.getValue();
            return new OkWithValue<ConcurrentDictionary<int, String>>("Store Exist", store.getPurchasePredicatesDescriptions());
        }

        public ResultWithValue<ConcurrentDictionary<int, string>> getStoreSalesDescription(int storeID)
        {
            Store store;
            ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
            if (!storeRes.getTag())
            {
                return new FailureWithValue<ConcurrentDictionary<int, string>>("store not exist", null);
            }
            store = storeRes.getValue();
            return new OkWithValue<ConcurrentDictionary<int, String>>("Store Exist", store.getSalesDescriptions());
        }
    }
}
