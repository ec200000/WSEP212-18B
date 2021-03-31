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
        public ConcurrentBag<User> users { get; set; }
        public ConcurrentDictionary<String,String> usersInfo { get; set; }

        public bool insertNewUser(User newUser,String password)
        {
            users.Add(newUser);
            return true;
        }

        public bool removeUser(User userToRemove)
        {
            return users.TryTake(out userToRemove);
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
