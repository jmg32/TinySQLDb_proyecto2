using Entities;
using QueryProcessor.Parser;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {

        internal OperationStatus Execute(ParsedQuery parsedQuery)
        {
            // Convertimos el diccionario de columnas en una lista de ColumnDefinition
            var columns = parsedQuery.Columns
                .Select(column => new ColumnDefinition(column.Key, column.Value.ToString())) // Aquí pasamos los argumentos
                .ToList();

            // Llamamos al Store con la lista de columnas
            return Store.GetInstance().CreateTable("DefaultDatabase", parsedQuery.TableName, columns);
        }

    }
}

