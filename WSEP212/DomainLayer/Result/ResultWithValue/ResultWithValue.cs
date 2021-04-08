using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer.Result
{
    public interface ResultWithValue<T>
    {
        public bool getTag();
        public String getMessage();
        public T getValue();
    }
}
