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
        public ConcurrentDictionary<User, bool> users { get; set; } //<user, if he is logged>
        public ConcurrentDictionary<String,String> usersInfo { get; set; }

        public bool insertNewUser(User newUser,String password)
        {
            users.TryAdd(newUser, false);
            return true;
        }

        public bool changeUserLoginStatus(User user, bool status, String password)
        {
            bool oldStatus;
            if (users.TryGetValue(user, out oldStatus))
                return users.TryUpdate(user, status, oldStatus);
            return false;
        }

        public bool removeUser(User userToRemove)
        {
            return users.TryRemove(userToRemove, out _);
        }

        public bool updateUser(User userToUpdate)
        {
            return true;
        }

        //<param>: String userName
        //<returns>: If found -> user returned, otherwise null is returned
        public User findUserByUserName(String userName)
        {
            foreach(User user in users)
            {
                if(user.userName.Equals(userName))
                {
                    return user;
                }
            }
            return null;
        }
    }
}
