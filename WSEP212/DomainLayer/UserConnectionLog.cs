using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WSEP212.DataAccessLayer;

namespace WSEP212.DomainLayer
{
    public class UserConnectionLog
    {
        [Key]
        public string userName { get; set; }

        public DateTime loggedIn { get; set; }

        public UserConnectionLog()
        {
        }

        public UserConnectionLog(string username, DateTime date)
        {
            this.userName = username;
            this.loggedIn = date;
        }

        public void addToDB()
        {
            lock (SystemDBAccess.savelock)
            {
                var result = SystemDBAccess.Instance.UserConnection.SingleOrDefault(c => c.userName == this.userName);
                if (result != null)
                {
                    result.loggedIn = this.loggedIn;
                    SystemDBAccess.Instance.SaveChanges();
                    
                }
                else
                {
                    SystemDBAccess.Instance.UserConnection.Add(this);
                    SystemDBAccess.Instance.SaveChanges();
                }
            
            }
        }
    }
}