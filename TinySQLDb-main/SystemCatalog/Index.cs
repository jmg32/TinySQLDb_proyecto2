using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemCatalog
{
    public class Index
    {
        public string IndexName { get; private set; }
        public string ColumnName { get; private set; }
        public IndexType indexType { get; private set; }

        // Estructura de datos para almacenar el índice
        private SortedDictionary<object, Row> indexEntries;

        public Index(string indexName, string columnName, IndexType indexType)
        {
            IndexName = indexName;
            ColumnName = columnName;
            indexType = indexType;
            indexEntries = new SortedDictionary<object, Row>(); // Inicializamos el diccionario ordenado
        }

        // Insertar una fila en el índice
        public void Add(Row row)
        {
            object columnValue = row.GetValue(ColumnName);
            if (!indexEntries.ContainsKey(columnValue))
            {
                indexEntries.Add(columnValue, row);
            }
        }

        // Eliminar una fila del índice
        public void Remove(Row row)
        {
            object columnValue = row.GetValue(ColumnName);
            if (indexEntries.ContainsKey(columnValue))
            {
                indexEntries.Remove(columnValue);
            }
        }

        // Ejemplo de búsqueda en el índice (puede expandirse)
        public Row Search(object columnValue)
        {
            if (indexEntries.ContainsKey(columnValue))
            {
                return indexEntries[columnValue];
            }
            return null;
        }

        public enum IndexType
        {
            BTREE,
            BST
        }
    }
}


