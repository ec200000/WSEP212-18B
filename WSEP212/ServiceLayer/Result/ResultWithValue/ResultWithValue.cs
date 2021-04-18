using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.ServiceLayer.Result
{
    public interface ResultWithValue<T>
    {
        public bool getTag();
        public String getMessage();
        public T getValue();
    }
}
