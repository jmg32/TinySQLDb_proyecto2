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
    internal class CreateIndex
    {
        public OperationStatus Execute(ParsedQuery parsedQuery)
        {
            // Validar si el índice ya existe en esa columna
            if (SystemCatalogManager.IndexExists(parsedQuery.DatabaseName, parsedQuery.TableName, parsedQuery.IndexName))
            {
                return OperationStatus.IndexAlreadyExists;
            }

            // Verificar si el tipo de índice es válido (por ejemplo, BTREE o BST)
            if (parsedQuery.IndexType != SystemCatalog.Index.IndexType.BTREE.ToString() && parsedQuery.IndexType != SystemCatalog.Index.IndexType.BST.ToString())
            {
                return OperationStatus.InvalidIndexType; // Asumiendo que tienes un estado para índice inválido
            }

            // Crear el índice con el tipo como string
            SystemCatalog.Index.IndexType indexType;
            if (Enum.TryParse(parsedQuery.IndexType.ToString(), out indexType))
            {
                SystemCatalogManager.CreateIndex(
                    parsedQuery.DatabaseName,
                    parsedQuery.TableName,
                    parsedQuery.IndexName,
                    parsedQuery.ColumnName,
                    indexType
                );
            }
            else
            {
                return OperationStatus.InvalidIndexType;
            }

            return OperationStatus.Success;
        }
    }
}
