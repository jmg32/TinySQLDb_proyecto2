using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiInterface.Exceptions
{
    public class UnknownRequestTypeException : Exception
    {
        public UnknownRequestTypeException(string message) : base(message)
        {
        }
    }
}
