using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class Authentication
    {
        [NotMapped]
        public ConcurrentDictionary<String,String> usersInfo { get; set; }

        [Key] 
        public int field { get; set; }
        public string UserInfoJson
        {
            get => JsonConvert.SerializeObject(usersInfo);
            set => usersInfo = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(value);
        }
        private static readonly Lazy<Authentication> lazy
            = new Lazy<Authentication>(() => new Authentication());

        public static Authentication Instance
            => lazy.Value;

        private Authentication()
        {
            usersInfo = new ConcurrentDictionary<string, string>();
            if(SystemDBAccess.Instance.UsersInfo.Find(1)!=null)
                usersInfo = new ConcurrentDictionary<string, string>(SystemDBAccess.Instance.UsersInfo.Find(1).usersInfo);

            field = 0;
        }

        public string encryptPassword(string password)
        {
            var prf = KeyDerivationPrf.HMACSHA256;
            var rng = RandomNumberGenerator.Create();
            const int iterCount = 10000;
            const int saltSize = 128 / 8;
            const int numBytesRequested = 256 / 8;

            // Produce a version 3 (see comment above) text hash.
            var salt = new byte[saltSize];
            rng.GetBytes(salt);
            var subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, iterCount);
            WriteNetworkByteOrder(outputBytes, 9, saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return Convert.ToBase64String(outputBytes);
        }

        public bool validatePassword(string hashedPassword, string providedPassword)
        {
            var decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            // Wrong version
            if (decodedHashedPassword[0] != 0x01)
                return false;

            // Read header information
            var prf = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHashedPassword, 1);
            var iterCount = (int)ReadNetworkByteOrder(decodedHashedPassword, 5);
            var saltLength = (int)ReadNetworkByteOrder(decodedHashedPassword, 9);

            // Read the salt: must be >= 128 bits
            if (saltLength < 128 / 8)
            {
                return false;
            }
            var salt = new byte[saltLength];
            Buffer.BlockCopy(decodedHashedPassword, 13, salt, 0, salt.Length);

            // Read the subkey (the rest of the payload): must be >= 128 bits
            var subkeyLength = decodedHashedPassword.Length - 13 - salt.Length;
            if (subkeyLength < 128 / 8)
            {
                return false;
            }
            var expectedSubkey = new byte[subkeyLength];
            Buffer.BlockCopy(decodedHashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the incoming password and verify it
            var actualSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, prf, iterCount, subkeyLength);
            return actualSubkey.SequenceEqual(expectedSubkey);
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }

        public void insertUserInfo(string userName, string password)
        {
            //var result = SystemDBAccess.Instance.UsersInfo.SingleOrDefault(a => a.usersInfo == this.usersInfo);
            var result = SystemDBAccess.Instance.UsersInfo.Find(1);
            if (result != null)
            {
                usersInfo.TryAdd(userName, encryptPassword(password));
                result.UserInfoJson = this.UserInfoJson;
                SystemDBAccess.Instance.SaveChanges();
            }
            else //first time - no passwords are saved
            {
                usersInfo.TryAdd(userName, encryptPassword(password));
                this.UserInfoJson = JsonConvert.SerializeObject(usersInfo);
                SystemDBAccess.Instance.UsersInfo.Add(this);
            }
        }

        public bool removeUserInfo(string userName)
        {
            return usersInfo.TryRemove(userName, out _);
        }

        public String getUserPassword(string userName)
        {
            return usersInfo.TryGetValue(userName, out var password) ? password : null;
        }

        public String[] getAllUsers()
        {
            return usersInfo.Keys.ToArray();
        }

    }
}