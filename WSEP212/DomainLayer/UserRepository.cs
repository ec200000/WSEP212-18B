using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class UserRepository
    {
        //singelton
        public ConcurrentBag<User> users { get; set; }
    }
}
