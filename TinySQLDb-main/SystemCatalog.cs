using System;

public static class SystemCatalog
{
    private static Dictionary<string, Database> databases = new Dictionary<string, Database>();

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
}
