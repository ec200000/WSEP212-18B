using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public interface SalesPolicyRule
    {
        public bool applyRule(UserType userType, ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, SalesType> itemsSaleType);
    }
}
