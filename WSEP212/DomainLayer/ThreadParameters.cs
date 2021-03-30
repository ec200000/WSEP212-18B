using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WSEP212.DomainLayer
{
    class ThreadParameters
    {
        public EventWaitHandle eventWaitHandle = new ManualResetEvent(false);

        public Object result;

        public Object[] parameters;
    }
}
