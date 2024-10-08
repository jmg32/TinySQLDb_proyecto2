using Entities;
using QueryProcessor.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCatalog;

namespace QueryProcessor.Operations
{
    internal class CreateDatabase
    {
        public OperationStatus Execute(ParsedQuery parsedQuery)
        {
            // Verificar si la base de datos ya existe
            if (SystemCatalogManager.DatabaseExists(parsedQuery.DatabaseName))
            {
                return OperationStatus.DatabaseAlreadyExists;
            }

            // Crear la nueva base de datos
            SystemCatalogManager.CreateDatabase(parsedQuery.DatabaseName);
            return OperationStatus.Success;
        }
    }
}
