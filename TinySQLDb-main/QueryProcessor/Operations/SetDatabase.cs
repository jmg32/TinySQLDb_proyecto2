using QueryProcessor.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCatalog;
using Entities;

namespace QueryProcessor.Operations
{
    internal class SetDatabase
    {
        public Entities.OperationStatus Execute(ParsedQuery parsedQuery)
        {
            // Verificar si la base de datos existe en el SystemCatalogManager
            if (!SystemCatalogManager.DatabaseExists(parsedQuery.DatabaseName))
            {
                return OperationStatus.DatabaseNotFound;
            }

            // Cambiar la base de datos activa en el SystemCatalogManager
            SystemCatalogManager.SetActiveDatabase(parsedQuery.DatabaseName);
            return OperationStatus.Success;
        }
    }
}
