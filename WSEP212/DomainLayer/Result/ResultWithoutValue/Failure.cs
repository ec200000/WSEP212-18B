using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer.Result
{
    public class Failure : RegularResult
    {
        private String message;

        public Failure(String message)
        {
            this.message = message;
        }

        public bool getTag()
        {
            return false;
        }

        public String getMessage()
        {
            return this.message;
        }
    }
}
