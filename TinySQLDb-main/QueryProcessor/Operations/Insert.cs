using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using QueryProcessor.Parser;

namespace QueryProcessor.Operations
{
    internal class Insert
    {
        internal OperationStatus Execute(ParsedQuery parsedQuery)
        {
            // Asegúrate de que parsedQuery.Values sea un Dictionary<string, object>
            if (parsedQuery.Values == null || !parsedQuery.Values.Any())
            {
                throw new ArgumentException("No hay valores para insertar.");
            }

            // Puedes usar directamente parsedQuery.Values como rowData
            return Store.GetInstance().InsertIntoTable(parsedQuery.DatabaseName, parsedQuery.TableName, parsedQuery.Values);
        }
    }
}

