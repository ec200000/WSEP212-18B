using System;

namespace WSEP212.DomainLayer.Result
{
    class Ok<T> : Result<T>
    {
        private String message;
        private T value;

        public Ok(String message, T value)
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
