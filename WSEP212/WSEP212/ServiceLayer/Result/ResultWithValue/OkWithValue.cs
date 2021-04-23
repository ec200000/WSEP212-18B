using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.ServiceLayer.Result
{
    public class OkWithValue<T> : ResultWithValue<T>
    {
        private String message;
        private T value;

        public OkWithValue(String message, T value)
        {
            this.message = message;
            this.value = value;
        }

        public bool getTag()
        {
            return true;
        }
        public String getMessage()
        {
            return this.message;
        }

        public T getValue()
        {
            return this.value;
        }
    }
}
