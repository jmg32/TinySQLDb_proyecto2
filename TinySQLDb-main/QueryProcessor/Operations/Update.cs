using QueryProcessor.Parser;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCatalog;
using Entities;

namespace QueryProcessor.Operations
{
    public class Update
    {
        public Entities.OperationStatus Execute(ParsedQuery parsedQuery)
        {
            // Buscar las filas que cumplen la condición WHERE (si la hay)
            var rowsToUpdate = SystemCatalogManager.GetRows(parsedQuery.DatabaseName, parsedQuery.TableName, parsedQuery.WhereClause);

            // Actualizar los valores de las columnas especificadas
            foreach (var row in rowsToUpdate)
            {
                // Usar SetValue en lugar de la indexación directa
                row.SetValue(parsedQuery.ColumnName, parsedQuery.NewValue);

                // Si la columna es indizada, actualizar el índice
                if (SystemCatalogManager.ColumnIsIndexed(parsedQuery.DatabaseName, parsedQuery.TableName, parsedQuery.ColumnName))
                {
                    SystemCatalogManager.UpdateIndexAfterDelete(parsedQuery.DatabaseName, parsedQuery.TableName, parsedQuery.ColumnName, row);
                }
            }

            return Entities.OperationStatus.Success;
        }
    }
}

    