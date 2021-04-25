using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WSEP212.DomainLayer
{
    public class ThreadParameters
    {
        public EventWaitHandle eventWaitHandle = new ManualResetEvent(false); //the handler (notification if finished)

        public Object result; //the result of the action

        public Object[] parameters; //the parameters of the function
    }
}
