using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class UserRepository
    {
        //singelton
        ConcurrentBag<User> users;
    }
}
