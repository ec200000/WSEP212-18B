using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class UserRepository
    {
        private readonly object insertLock = new object();
        private readonly object changeStatusLock = new object();
        private static readonly Lazy<UserRepository> lazy
        = new Lazy<UserRepository>(() => new UserRepository());

        public static UserRepository Instance
            => lazy.Value;

        private UserRepository() {
            users = new ConcurrentDictionary<User, bool>();
            users = new ConcurrentDictionary<User, bool>(SystemDBAccess.Instance.Users.ToDictionary(g => g, g=>g!=null));
        }
        public ConcurrentDictionary<User,bool> users { get; set; }

        public void createSystemManager()
        {
            User systemManager = new User("big manager", 25, true);
            systemManager.changeState(new SystemManagerState(systemManager));
            RegularResult res = insertNewUser(systemManager, "123456");
            if (!res.getTag())
                throw new SystemException("Couldn't create a system manager");
        }
        
        public RegularResult insertNewUser(User newUser, String password)
        {
            lock (insertLock)
            {
                if(checkIfUserExists(newUser.userName))
                {
                    return new Failure("User Name Already Exists In The System");
                }
                users.TryAdd(newUser, false);
                Authentication.Instance.insertUserInfo(newUser.userName, password);
                return new Ok("Registration To The System Was Successful");
            }
        }
        
        public RegularResult addLoginUser(User newUser)
        {
            lock (insertLock)
            {
                if(checkIfUserExists(newUser.userName))
                {
                    return new Failure("User Name Already Exists In The System");
                }
                users.TryAdd(newUser, true);
                return new Ok("Registration To The System Was Successful");
            }
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
                    bool res = Authentication.Instance.validatePassword(userPasswordRes.getValue(),passwordToValidate);
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
            return users.TryRemove(userToRemove, out _) && Authentication.Instance.removeUserInfo(userToRemove.userName);
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
           // if (SystemDBAccess.Instance.Users.Find(userName) != null)
            //    return new OkWithValue<User>("The User Was Found In The User Repository Successfully",SystemDBAccess.Instance.Users.Find(userName));
            return new FailureWithValue<User>("The User Name Not Exist In The User Repository", null);
        }

        public ResultWithValue<String> getUserPassword(string userName)
        {
            String password;
            if ((password = Authentication.Instance.getUserPassword(userName)) != null)
                return new OkWithValue<String>("User Password Successfully Found", password);
            return new FailureWithValue<String>("User Password Not Found", null);
        }

        public bool checkIfUserExists(string userName)
        {
            foreach( KeyValuePair<User,bool> pair in users)
            {
                if (pair.Key.userName.Equals(userName))
                    return true;
            }
            //if (SystemDBAccess.Instance.Users.Find(userName) != null)
            //    return true;
            return false;
        }

        public ConcurrentDictionary<String, ConcurrentBag<PurchaseInvoice>> getAllUsersPurchaseHistory()
        {
            ConcurrentDictionary<String, ConcurrentBag<PurchaseInvoice>> purchaseHistory = new ConcurrentDictionary<string, ConcurrentBag<PurchaseInvoice>>();
            foreach(KeyValuePair<User,bool> user in users)
            {
                if (!purchaseHistory.TryAdd(user.Key.userName, user.Key.purchases))
                    return null;
            }
            return purchaseHistory;
        }

        public ResultWithValue<ConcurrentBag<PurchaseInvoice>> getUserPurchaseHistory(string userName)
        {
            User u = findUserByUserName(userName).getValue();
            if (u.state is LoggedBuyerState)
            {
                return new OkWithValue<ConcurrentBag<PurchaseInvoice>>("ok", u.purchases);
            }

            return new FailureWithValue<ConcurrentBag<PurchaseInvoice>>("the user is not registered to the system", null);
        }
    }
}
