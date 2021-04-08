using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer.Result
{
    class Failure<T> : Result<T>
    {
        private String message;
        private T value;

        public Failure(String message, T value)
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
