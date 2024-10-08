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
    internal class Delete
    {
        public OperationStatus Execute(ParsedQuery parsedQuery)
        {
            // Buscar las filas que cumplen la condición WHERE (si la hay)
            var rowsToDelete = SystemCatalogManager.GetRows(parsedQuery.DatabaseName, parsedQuery.TableName, parsedQuery.WhereClause);

            // Eliminar las filas y actualizar los índices si corresponde
            foreach (var row in rowsToDelete)
            {
                SystemCatalogManager.DeleteRow(parsedQuery.DatabaseName, parsedQuery.TableName, row);

                // Si la columna es indizada, actualizar el índice
                if (SystemCatalogManager.ColumnIsIndexed(parsedQuery.DatabaseName, parsedQuery.TableName, parsedQuery.ColumnName))
                {
                    SystemCatalogManager.UpdateIndexAfterDelete(parsedQuery.DatabaseName, parsedQuery.TableName, parsedQuery.ColumnName, row);
                }
            }
            return OperationStatus.Success;
        }
    }
}

