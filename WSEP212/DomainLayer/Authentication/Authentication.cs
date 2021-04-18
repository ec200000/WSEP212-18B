using System;
using System.Collections.Concurrent;

namespace WSEP212.DomainLayer
{
    public class Authentication
    {
        private ConcurrentDictionary<String,String> usersInfo { get; set; }
        private static readonly Lazy<Authentication> lazy
            = new Lazy<Authentication>(() => new Authentication());

        public static Authentication Instance
            => lazy.Value;

        private Authentication() { usersInfo = new ConcurrentDictionary<string, string>();}

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

        public void insertUserInfo(string userName, string password)
        {
            usersInfo.TryAdd(userName, encryptPassword(password));
        }

        public bool removeUserInfo(string userName)
        {
            return usersInfo.TryRemove(userName, out _);
        }

        public String getUserPassword(string userName)
        {
            return usersInfo.TryGetValue(userName, out var password) ? password : null;
        }
    }
}