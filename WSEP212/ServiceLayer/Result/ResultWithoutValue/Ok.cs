using System;

namespace WSEP212.ServiceLayer.Result
{
    public class Ok : RegularResult
    {
        private String message;

        public Ok(String message)
        {
            this.message = message;
        }

        public bool getTag()
        {
            return true;
        }
        public String getMessage()
        {
            return this.message;
        }
    }
}
