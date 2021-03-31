using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class StoreRepository
    {
        //singelton
        ConcurrentBag<Store> stores;
    }
}
