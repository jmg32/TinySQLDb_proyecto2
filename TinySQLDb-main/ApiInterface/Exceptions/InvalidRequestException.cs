using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiInterface.Exceptions
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(string message) : base(message)
        {
        }
    }
}
