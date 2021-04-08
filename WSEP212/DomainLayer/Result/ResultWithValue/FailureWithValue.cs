using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer.Result
{
    public class FailureWithValue<T> : ResultWithValue<T>
    {
        private String message;
        private T value;

        public FailureWithValue(String message, T value)
        {
            this.message = message;
            this.value = value;
        }

        public bool getTag()
        {
            return false;
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
