using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Entities;

namespace StoreDataManager
{

    public class ColumnDefinition
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public ColumnDefinition(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }
    }

    public sealed class Store
    {
        private static Store? instance = null;
        private static readonly object _lock = new object();

        private const string DatabaseBasePath = @"C:\TinySql\";
        private const string DataPath = $@"{DatabaseBasePath}\Data";
        private const string SystemCatalogPath = $@"{DataPath}\SystemCatalog";
        private const string SystemDatabasesFile = $@"{SystemCatalogPath}\SystemDatabases.table";
        private const string SystemTablesFile = $@"{SystemCatalogPath}\SystemTables.table";

        public static Store GetInstance()
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = new Store();
                }
                return instance;
            }
        }

        public Store()
        {
            this.InitializeSystemCatalog();
        }

        private void InitializeSystemCatalog()
        {
            Directory.CreateDirectory(SystemCatalogPath);
        }

        public OperationStatus CreateDatabase(string databaseName)
        {
            var databasePath = $@"{DataPath}\{databaseName}";
            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(databasePath);

                // Actualiza SystemCatalog
                using (FileStream stream = File.Open(SystemDatabasesFile, FileMode.Append))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(databaseName);
                }

                return OperationStatus.Success;
            }
            return OperationStatus.DatabaseAlreadyExists;
        }

        public List<Dictionary<string, object>> Select(string databaseName, string tableName, List<string> requestedColumns = null)
        {
            var tablePath = $@"{DataPath}\{databaseName}\{tableName}.Table"; // Se especifica la base de datos y la tabla.

            if (!File.Exists(tablePath)) // Si la tabla no existe, se devuelve un error.
            {
                throw new Exception("Table not found");
            }

            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            try
            {
                using (FileStream stream = File.Open(tablePath, FileMode.Open)) // Abrir el archivo binario de la tabla.
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    // Leer el esquema de la tabla (asumiendo que el esquema está al inicio del archivo)
                    List<ColumnDefinition> tableSchema = new List<ColumnDefinition>();
                    while (stream.Position < stream.Length)
                    {
                        string columnName = reader.ReadString();
                        string columnType = reader.ReadString();
                        tableSchema.Add(new ColumnDefinition(columnName, columnType));

                        // Salir del bucle cuando todas las columnas del esquema se hayan leído.
                        if (columnName == null || columnType == null) break;
                    }

                    // Leer los registros de la tabla
                    while (stream.Position < stream.Length)
                    {
                        Dictionary<string, object> row = new Dictionary<string, object>();

                        foreach (var column in tableSchema)
                        {
                            // Leer el valor de la columna dependiendo del tipo
                            if (column.Type == "int")
                            {
                                row[column.Name] = reader.ReadInt32();
                            }
                            else if (column.Type == "string")
                            {
                                row[column.Name] = reader.ReadString();
                            }
                        }

                        // Si hay columnas solicitadas, filtrarlas
                        if (requestedColumns != null && requestedColumns.Count > 0)
                        {
                            var filteredRow = new Dictionary<string, object>();
                            foreach (var col in requestedColumns)
                            {
                                if (row.ContainsKey(col))
                                {
                                    filteredRow[col] = row[col];
                                }
                            }
                            results.Add(filteredRow);
                        }
                        else
                        {
                            // Si no se solicitaron columnas específicas, agregar la fila completa
                            results.Add(row);
                        }
                    }
                }

                return results;
            }

            catch (Exception ex)
            {
                // Manejar el error si ocurre algún problema
                Console.WriteLine($"Error al seleccionar desde la tabla: {ex.Message}");
                throw new Exception("Error al seleccionar desde la tabla");
            }
        }
        public OperationStatus InsertIntoTable(string databaseName, string tableName, Dictionary<string, object> rowData)
        {
            var tablePath = $@"{DataPath}\{databaseName}\{tableName}.Table"; // Se especifica la base de datos.

            if (!File.Exists(tablePath)) // Si la tabla no existe, se devuelve un error.
            {
                return OperationStatus.Error;
            }

            try
            {
                using (FileStream stream = File.Open(tablePath, FileMode.Append)) // Se abre el archivo binario de la tabla en modo Append.
                using (BinaryWriter writer = new BinaryWriter(stream)) // Se crea un BinaryWriter para escribir en el archivo.
                {
                    // Escribe cada columna de la fila en el archivo.
                    foreach (var column in rowData)
                    {
                        if (column.Value is int)
                        {
                            writer.Write((int)column.Value);
                        }
                        else if (column.Value is string)
                        {
                            string value = (string)column.Value;

                            if (column.Key == "Nombre")
                            {
                                value = value.PadRight(30); // Se asegura de que el string tenga 30 caracteres.
                            }
                            else if (column.Key == "Apellidos")
                            {
                                value = value.PadRight(50); // Se asegura de que el string tenga 50 caracteres.
                            }

                            writer.Write(value);
                        }
                    }
                }
                return OperationStatus.Success;
            }
            catch (Exception ex)
            {
                // Manejar el error y devolver el estado de error
                Console.WriteLine($"Error al insertar en la tabla: {ex.Message}");
                return OperationStatus.Error;
            }
        }




        public OperationStatus CreateTable(string databaseName, string tableName, List<ColumnDefinition> columns)
        {
            var databasePath = $@"{DataPath}\{databaseName}";
            if (!Directory.Exists(databasePath))
            {
                return OperationStatus.DatabaseNotFound;
            }

            var tablePath = $@"{databasePath}\{tableName}.Table";
            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Escribir el esquema de la tabla en el archivo
                foreach (var column in columns)
                {
                    writer.Write(column.Name);
                    writer.Write(column.Type);
                }
            }

            // Actualiza el SystemCatalog
            using (FileStream stream = File.Open(SystemTablesFile, FileMode.Append))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(databaseName);
                writer.Write(tableName);
            }

            return OperationStatus.Success;
        }
        
    }
}
