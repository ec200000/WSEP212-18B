using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer.Result;

namespace WSEP212.DomainLayer
{
    public interface PolicyRule
    {
        public Result<Object> applyRule(User user, ConcurrentDictionary<Item, int> items);
    }
}
