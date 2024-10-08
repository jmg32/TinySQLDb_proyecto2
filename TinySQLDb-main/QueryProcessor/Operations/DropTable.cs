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
    internal class DropTable
    {
        public OperationStatus Execute(ParsedQuery parsedQuery)
        {
            // Verificar si la tabla está vacía y eliminarla
            if (!SystemCatalogManager.IsTableEmpty(parsedQuery.DatabaseName, parsedQuery.TableName))
            {
                return OperationStatus.TableNotEmpty;
            }
            SystemCatalogManager.DropTable(parsedQuery.DatabaseName, parsedQuery.TableName);
            return OperationStatus.Success;
        }
    }
}
