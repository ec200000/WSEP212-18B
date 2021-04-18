using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class Authentication
    {
        private readonly object insertLock = new object();
        private static readonly Lazy<Authentication> lazy
        = new Lazy<Authentication>(() => new Authentication());

        public static Authentication Instance
            => lazy.Value;

        public ConcurrentDictionary<String,String> usersInfo { get; set; }

        private Authentication()
        {
            usersInfo = new ConcurrentDictionary<string, string>();
        }

        public String encryptPassword(String password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            String hash = System.Text.Encoding.ASCII.GetString(data);
            return hash;
        }

        public bool validatePassword(String passwordToValidate, String userPassword) //the user password is already encrypted
        {
            string passwordToValidateHash = encryptPassword(passwordToValidate);

            return passwordToValidateHash.Equals(userPassword);
        }
        
        public RegularResult insertNewUser(User newUser,String password)
        {
            lock (insertLock)
            {
                if(checkIfUserExists(newUser.userName))
                {
                    return new Failure("User Name Already Exists In The System");
                }
                UserRepository.Instance.users.TryAdd(newUser, false);
                usersInfo.TryAdd(newUser.userName, Authentication.Instance.encryptPassword(password));
                return new Ok("Registration To The System Was Successful");
            }
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
