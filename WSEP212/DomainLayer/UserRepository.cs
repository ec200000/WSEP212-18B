using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer.AuthenticationSystem;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WSEP212.DataAccessLayer;
using WSEP212.ServiceLayer;
using WSEP212.DomainLayer.SystemLoggers;
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

        private UserRepository()
        {
            initRepo();
        }
        public ConcurrentDictionary<User,bool> users { get; set; }

        public void initRepo()
        {
            users = new ConcurrentDictionary<User, bool>(); 
            var usersList = SystemDBAccess.Instance.Users.ToList();
            var cartsList = SystemDBAccess.Instance.Carts.ToList();
            foreach (var user in usersList)
            {
                foreach (var cart in cartsList)
                {
                    if (cart.cartOwner.Equals(user.userName))
                    {
                        user.shoppingCart = cart;
                        break;
                    }
                }
                users.TryAdd(user, false); //when the system is starting no user is logged in
            }
        }
        public void createSystemManager()
        {
            User systemManager = new User("big manager", 25, true);
            systemManager.changeState(new SystemManagerState(systemManager));
            RegularResult res = insertNewUser(systemManager, "123456");
            if (!res.getTag())
                throw new SystemException("Could not create a system manager");
        }
        
        public class JsonUsers
        {
            public IList<JsonUser> users;
        }
        public class JsonUser
        {
            public string username;
            public int userAge;
            public bool isSystemManager;
        }
        
        public void Init()
        {
            string jsonFilePath = "init.json";
            string json = File.ReadAllText(jsonFilePath);
            dynamic array = JsonConvert.DeserializeObject(json);
            // CREATE USERS
            string loggedUser = array.loggedUser;
            foreach (var item in array.users)
            {
                string username = item.username;
                string userAge = item.userAge;
                string isSystemManager = item.isSystemManager;
                if (SystemDBAccess.Instance.Users.SingleOrDefault(u => u.userName == username) == null)
                {
                    if (isSystemManager.Equals("true"))
                    //User user = new User(username, int.Parse(userAge), isSystemManager.Equals("true"));
                        SystemController.Instance.registerAsSystemManager(username, 18,"123456");
                    else
                    {
                        SystemController.Instance.register(username, 18,"123456");
                    }
                    User user = UserRepository.Instance.findUserByUserName(username).getValue();
                    if (isSystemManager.Equals("true"))
                        SystemController.Instance.loginAsSystemManager(username, "123456");
                    else if (loggedUser.Equals(username))
                        SystemController.Instance.login(username, "123456");
                    else
                        user.changeState(new GuestBuyerState(user));
                    
                }
            }
        }
        
        public RegularResult insertNewUser(User newUser, String password)
        {
            try
            {
                lock (insertLock)
                {
                    if(checkIfUserExists(newUser.userName))
                    {
                        return new Failure("User Name Already Exists In The System");
                    }
                    users.TryAdd(newUser, false);
                    Authentication.Instance.insertUserInfo(newUser.userName, password);
                    newUser.addToDB();
                    return new Ok("Registration To The System Was Successful");
                }
            }
            catch (Exception e)
            {
                var msg = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    msg += inner.Message;
                Logger.Instance.writeErrorEventToLog(msg);
            }
            return new Failure("failed in user repo");
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
        public RegularResult changeUserLoginStatus(String userName, bool status, String passwordToValidate)
        {
            bool oldStatus;
            if(status) //need to verify it's password
            {
                ResultWithValue<String> userPasswordRes = getUserPassword(userName);
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
                ResultWithValue<User> userRes = findUserByUserName(userName);
                if (userRes.getTag())
                {
                    if(users.TryGetValue(userRes.getValue(), out oldStatus))
                    {
                        if(oldStatus != status)
                        {
                            users.TryUpdate(userRes.getValue(), status, oldStatus);
                            return new Ok("User Change Login Status Successfully");
                        }
                        return new Failure("The User Is Already In The Same Login Status");
                    }
                }
                return new Failure("System Fails To Find The Login User");
            }
            
        }

        //removing completely from the system
        public bool removeUser(User userToRemove)
        {
            return users.TryRemove(userToRemove, out _) && Authentication.Instance.removeUserInfo(userToRemove.userName);
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
            return false;
        }

        public ConcurrentDictionary<String, ConcurrentDictionary<int, PurchaseInvoice>> getAllUsersPurchaseHistory()
        {
            ConcurrentDictionary<String, ConcurrentDictionary<int, PurchaseInvoice>> purchaseHistory = new ConcurrentDictionary<String, ConcurrentDictionary<int, PurchaseInvoice>>();
            foreach(KeyValuePair<User,bool> user in users)
            {
                if (!purchaseHistory.TryAdd(user.Key.userName, user.Key.purchases))
                    return null;
            }
            return purchaseHistory;
        }

        public ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> getUserPurchaseHistory(string userName)
        {
            User u = findUserByUserName(userName).getValue();
            if (u.state is LoggedBuyerState)
            {
                return new OkWithValue<ConcurrentDictionary<int, PurchaseInvoice>>("ok", u.purchases);
            }

            return new FailureWithValue<ConcurrentDictionary<int, PurchaseInvoice>>("the user is not registered to the system", null);
        }
    }
}
