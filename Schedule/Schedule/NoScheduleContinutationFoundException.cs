using System;
using System.Collections.Generic;
using System.Text;

namespace Schedule
{
    public class NoScheduleContinutationFoundException : Exception
    {
        public NoScheduleContinutationFoundException(string msg) : base(msg)
        {

        }
    }
}
