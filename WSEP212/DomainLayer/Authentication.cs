using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class Authentication
    {
        private static readonly Lazy<Authentication> lazy
        = new Lazy<Authentication>(() => new Authentication());

        public static Authentication Instance
            => lazy.Value;

        private Authentication() { }

        public String encryptPassword(String password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            String hash = System.Text.Encoding.ASCII.GetString(data);
            return hash;
        }
    }
}
