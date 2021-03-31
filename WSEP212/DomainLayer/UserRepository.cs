using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class UserRepository
    {
        private static readonly Lazy<UserRepository> lazy
        = new Lazy<UserRepository>(() => new UserRepository());

        public static UserRepository Instance
            => lazy.Value;

        private UserRepository() { }
        public ConcurrentDictionary<User,bool> users { get; set; } //<user,if he is logged>
        public ConcurrentDictionary<String,String> usersInfo { get; set; } //<userName, password> password will be encrypted

        public bool insertNewUser(User newUser,String password)
        {
            if(checkIfUserExists(newUser.userName))
            {
                return false; //user is already in the system
            }
            users.TryAdd(newUser,false);
            usersInfo.TryAdd(newUser.userName, Authentication.Instance.encryptPassword(password));
            return true;
        }

        //status is true: register -> login, otherwise: login -> logout
        public bool changeUserLoginStatus(User user, bool status, String passwordToValidate)
        {
            bool oldStatus;
            if(status) //need to verify it's password
            {
                string userPassword = getUserPassword(user.userName);
                if(userPassword != null) //found password in DB
                {
                    bool res = Authentication.Instance.validatePassword(passwordToValidate, userPassword);
                    if (!res) //the password that we are validating does not match to the password in the DB
                        return false;
                }
            }
            if(users.TryGetValue(user, out oldStatus))
                return users.TryUpdate(user,status, oldStatus);
            return false;
        }

        //removing completly from the system
        public bool removeUser(User userToRemove)
        {
            return users.TryRemove(userToRemove, out _) && usersInfo.TryRemove(userToRemove.userName, out _);
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
        public User findUserByUserName(String userName)
        {
            foreach (KeyValuePair<User,bool> user in users)
            {
                if(user.Key.userName.Equals(userName))
                {
                    return user.Key;
                }
            }
            return null;
        }

        public string getUserPassword(string userName)
        {
            string password;
            if (usersInfo.TryGetValue(userName, out password))
                return password;
            return null;
        }

        public bool checkIfUserExists(string userName)
        {
            foreach( KeyValuePair<String,String> userInfo in usersInfo)
            {
                if (userInfo.Key.Equals(userName))
                    return true;
            }
            return false;
        }
    }
}
