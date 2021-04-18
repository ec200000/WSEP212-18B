﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.Result;

namespace WSEP212.DomainLayer
{
    public class UserRepository
    {
        
        private readonly object changeStatusLock = new object();
        private static readonly Lazy<UserRepository> lazy
        = new Lazy<UserRepository>(() => new UserRepository());

        public static UserRepository Instance
            => lazy.Value;

        private UserRepository() {
            users = new ConcurrentDictionary<User, bool>();
           
        }
        public ConcurrentDictionary<User,bool> users { get; set; }
        
        
        public ConcurrentDictionary<User, bool> systemManagers { get; set; }

        public RegularResult changeSystemManagerStatus(string userName, string password, bool status)
        {
            foreach (var u in users)
            {
                if (u.Key.userName.Equals(userName) && u.Key.isSystemManager)
                {
                    return changeUserLoginStatus(u.Key, status, password);
                }
            }
            return new Failure($"The user {userName} is not a system manager!");
        }
        

        //status is true: register -> login, otherwise: login -> logout
        public RegularResult changeUserLoginStatus(User user, bool status, String passwordToValidate)
        {
            bool oldStatus;
            if(status) //need to verify it's password
            {
                ResultWithValue<String> userPasswordRes = getUserPassword(user.userName);
                if(userPasswordRes.getTag()) //found password in DB
                {
                    bool res = Authentication.Instance.validatePassword(passwordToValidate, userPasswordRes.getValue());
                    if (!res)  //the password that we are validating does not match to the password in the DB
                    {
                        return new Failure("The Password Entered Is Incorrect, Please Try Again");
                    }
                }
            }

            lock (changeStatusLock)
            {
                if(users.TryGetValue(user, out oldStatus))
                {
                    if(oldStatus != status)
                    {
                        users.TryUpdate(user, status, oldStatus);
                        return new Ok("User Change Login Status Successfully");
                    }
                    return new Failure("The User Is Already In The Same Login Status");
                }
                return new Failure("System Fails To Find The Login Status Of The User");
            }
            
        }

        //removing completely from the system
        public bool removeUser(User userToRemove)
        {
            return users.TryRemove(userToRemove, out _) && Authentication.Instance.usersInfo.TryRemove(userToRemove.userName, out _);
        }

        public bool updateUser(User userToUpdate)
        {
            bool userStatus;
            if(users.TryRemove(userToUpdate, out userStatus))
            {
                return users.TryAdd(userToUpdate, userStatus);
            }
            return false;
        }

        //<param>: String userName
        //<returns>: If found -> user returned, otherwise null is returned
        public ResultWithValue<User> findUserByUserName(String userName)
        {
            foreach (KeyValuePair<User,bool> user in users)
            {
                if(user.Key.userName.Equals(userName))
                {
                    return new OkWithValue<User>("The User Was Found In The User Repository Successfully", user.Key);
                }
            }
            return new FailureWithValue<User>("The User Name Not Exist In The User Repository", null);
        }

        public ResultWithValue<String> getUserPassword(string userName)
        {
            String password;
            if (Authentication.Instance.usersInfo.TryGetValue(userName, out password))
                return new OkWithValue<String>("User Password Successfully Found", password);
            return new FailureWithValue<String>("User Password Not Found", null);
        }

        public ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>> getAllUsersPurchaseHistory()
        {
            ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>> purchaseHistory = new ConcurrentDictionary<string, ConcurrentBag<PurchaseInfo>>();
            foreach(KeyValuePair<User,bool> user in users)
            {
                if (!purchaseHistory.TryAdd(user.Key.userName, user.Key.purchases))
                    return null;
            }
            return purchaseHistory;
        }

        public ResultWithValue<ConcurrentBag<PurchaseInfo>> getUserPurchaseHistory(string userName)
        {
            User u = findUserByUserName(userName).getValue();
            if (u.state is LoggedBuyerState)
            {
                return new OkWithValue<ConcurrentBag<PurchaseInfo>>("ok", u.purchases);
            }

            return new FailureWithValue<ConcurrentBag<PurchaseInfo>>("the user is not registered to the system", null);
        }
    }
}
