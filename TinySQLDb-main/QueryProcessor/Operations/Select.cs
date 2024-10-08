using Entities;
using QueryProcessor.Parser;
using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Operations
{
    internal class Select
    {
        internal OperationStatus Execute(ParsedQuery parsedQuery)
        {
            // Asegúrate de que parsedQuery contenga el nombre de la base de datos
            if (string.IsNullOrEmpty(parsedQuery.DatabaseName) || string.IsNullOrEmpty(parsedQuery.TableName))
            {
                return OperationStatus.Error;
            }

            // Verificar si hay columnas solicitadas, de lo contrario pasar null
            var requestedColumns = parsedQuery.Columns?.Keys.ToList();

            // Llamar al método Select de Store con el nombre de la base de datos, nombre de la tabla y las columnas opcionales
            var results = Store.GetInstance().Select(parsedQuery.DatabaseName, parsedQuery.TableName, requestedColumns);

            // Procesar los resultados (esto es un ejemplo, puedes adaptarlo según el formato que necesites)
            foreach (var row in results)
            {
                foreach (var column in row)
                {
                    Console.WriteLine($"{column.Key}: {column.Value}");
                }
                Console.WriteLine("----------");
            }

            return OperationStatus.Success;
        }
    }
}

