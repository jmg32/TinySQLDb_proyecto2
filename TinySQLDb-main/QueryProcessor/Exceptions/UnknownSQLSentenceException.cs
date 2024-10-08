using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Exceptions //aca si esta definida la excepcion correctamente,puede servir de ejemplo para otras.
{
    public class UnknownSQLSentenceException : Exception
    {
        public UnknownSQLSentenceException(string sentence)
            : base($"Sentencia SQL Desconocida: {sentence}")
        {
        }
    }

}
