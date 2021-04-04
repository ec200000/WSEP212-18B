using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public interface PolicyRule
    {
        public bool applyRule(User user, ConcurrentDictionary<Item, int> items);
    }
}
