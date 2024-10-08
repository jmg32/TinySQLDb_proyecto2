using System.Linq;

namespace SystemCatalog
{
    public static class SystemCatalogManager
    {
        private static Dictionary<string, Database> databases = new Dictionary<string, Database>();
        private static string activeDatabase;

        // Crear una base de datos
        public static void CreateDatabase(string dbName)
        {
            if (!databases.ContainsKey(dbName))
            {
                databases.Add(dbName, new Database(dbName));
            }
            else
            {
                throw new InvalidOperationException("Database already exists.");
            }
        }

        // Método para establecer la base de datos activa
        public static void SetActiveDatabase(string databaseName)
        {
            activeDatabase = databaseName;
        }

        // Método para obtener la base de datos activa
        public static string GetActiveDatabase()
        {
            return activeDatabase;
        }

        public static List<Row> GetRows(string dbName, string tableName, string whereClause)
        {
            if (TableExists(dbName, tableName))
            {
                var table = databases[dbName].GetTable(tableName);

                // Filtrar las filas según la cláusula WHERE
                // Aquí llamamos a un método que evalúa el whereClause
                return table.Rows.Where(row => EvaluateWhereClause(row, whereClause)).ToList();
            }
            throw new InvalidOperationException("Table not found.");
        }

        private static bool EvaluateWhereClause(Row row, string whereClause)
        {
            // Ejemplo de evaluación simple. Aquí puedes implementar lógica más compleja según tus necesidades.
            // Supongamos que `whereClause` tiene formato "columna = valor".

            var parts = whereClause.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                var columnName = parts[0].Trim();
                var value = parts[1].Trim();

                // Aquí deberías manejar la comparación según el tipo de datos de la columna
                // Este es un ejemplo muy básico y solo funciona para strings
                if (row.TryGetValue(columnName, out var rowValue))
                {
                    return rowValue.ToString() == value; // Comparación simple
                }
            }

            return false; // Si no se puede evaluar, devolver false
        }
        // Verificar si la base de datos existe
        public static bool DatabaseExists(string dbName)
        {
            return databases.ContainsKey(dbName);
        }

        // Crear una tabla en una base de datos específica
        public static void CreateTable(string dbName, string tableName)
        {
            if (DatabaseExists(dbName))
            {
                databases[dbName].AddTable(tableName);
            }
            else
            {
                throw new InvalidOperationException("Database not found.");
            }
        }

        // Verificar si una tabla existe dentro de una base de datos
        public static bool TableExists(string dbName, string tableName)
        {
            return DatabaseExists(dbName) && databases[dbName].TableExists(tableName);
        }

        // Obtener las columnas de una tabla
        public static List<string> GetColumns(string dbName, string tableName)
        {
            if (TableExists(dbName, tableName))
            {
                return databases[dbName].GetTable(tableName).Columns.Select(c => c.Name).ToList();
            }
            throw new InvalidOperationException("Table not found.");
        }


        // Agregar una columna a una tabla
        public static void AddColumnToTable(string dbName, string tableName, string columnName, string dataType, bool isPrimaryKey = false)
        {
            if (TableExists(dbName, tableName))
            {
                databases[dbName].GetTable(tableName).AddColumn(columnName, dataType, isPrimaryKey);
            }
            else
            {
                throw new InvalidOperationException("Table not found.");
            }
        }

        // Verificar si una tabla está vacía
        public static bool IsTableEmpty(string dbName, string tableName)
        {
            if (TableExists(dbName, tableName))
            {
                return databases[dbName].GetTable(tableName).Rows.Count == 0;
            }
            throw new InvalidOperationException("Table not found.");
        }

        // Eliminar una tabla de una base de datos
        public static void DropTable(string dbName, string tableName)
        {
            if (TableExists(dbName, tableName))
            {
                // Verificar si la tabla está vacía antes de eliminarla
                if (!IsTableEmpty(dbName, tableName))
                {
                    throw new InvalidOperationException("Table is not empty.");
                }
                databases[dbName].RemoveTable(tableName);
            }
            else
            {
                throw new InvalidOperationException("Table not found.");
            }
        }

        // Eliminar una fila de una tabla con una clave primaria específica
        public static void DeleteRow(string dbName, string tableName, Row row)
        {
            if (TableExists(dbName, tableName))
            {
                var table = databases[dbName].GetTable(tableName);
                table.Rows.Remove(row); // Remover la fila directamente
            }
            else
            {
                throw new InvalidOperationException("Table not found.");
            }
        }
        // Método para actualizar el índice después de eliminar una fila
        public static void UpdateIndexAfterDelete(string dbName, string tableName, string columnName, Row deletedRow)
        {
            if (TableExists(dbName, tableName))
            {
                var table = databases[dbName].GetTable(tableName);
                if (table.HasIndexOnColumn(columnName))
                {
                    table.UpdateIndexAfterDelete(columnName, deletedRow);
                }
            }
            else
            {
                throw new InvalidOperationException("Table not found.");
            }
        }
        // Método para verificar si una columna está indizada
        public static bool ColumnIsIndexed(string dbName, string tableName, string columnName)
        {
            if (TableExists(dbName, tableName))
            {
                var table = databases[dbName].GetTable(tableName);
                return table.HasIndexOnColumn(columnName);
            }
            throw new InvalidOperationException("Table not found.");
        }

        public static bool IndexExists(string dbName, string tableName, string indexName)
        {
            if (TableExists(dbName, tableName))
            {
                return databases[dbName].GetTable(tableName).IndexExists(indexName);
            }
            throw new InvalidOperationException("Table not found.");
        }
        public static void CreateIndex(string dbName, string tableName, string indexName, string columnName, Index.IndexType indexType)
        {
            if (TableExists(dbName, tableName))
            {
                databases[dbName].GetTable(tableName).CreateIndex(indexName, columnName, indexType);
            }
            else
            {
                throw new InvalidOperationException("Table not found.");
            }
        }
        public static void Reset()
        {
            databases.Clear(); // Limpia todas las bases de datos
            activeDatabase = null; // Resetea la base de datos activa
        }


    }
}
