using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class UserState
    {
        public User user { get; set; }

        public UserState(User user)
        {
            this.user = user;
        }
    }
}
