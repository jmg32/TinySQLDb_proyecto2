using System;
using System.Collections.Generic;
using StoreDataManager;

namespace StoredDataManagerTests
{
    class Program
    {
        static void Main(string[] args)
        {
            // Crear una instancia del Store (almacén de datos)
            var store = Store.GetInstance();

            // Prueba para crear una base de datos (solo se crea si no existe)
            var status = store.CreateDatabase("Universidad");
            Console.WriteLine($"Estado de la creación de la base de datos: {status}"); // Success o DatabaseAlreadyExists

            // Definir columnas para la tabla
            var columns = new List<ColumnDefinition>
            {
                new ColumnDefinition("ID", "int"),
                new ColumnDefinition("Nombre", "string"),
                new ColumnDefinition("Apellidos", "string")
            };

            // Prueba para crear una tabla en la base de datos (solo se crea si no existe)
            status = store.CreateTable("Universidad", "Estudiante", columns);
            Console.WriteLine($"Estado de la creación de la tabla: {status}"); // Success, TableAlreadyExists o DatabaseNotFound

            // Insertar datos en la tabla solo si fue creada o ya existe
            
            
                // Prueba para insertar registros en la tabla
                var row1 = new Dictionary<string, object>
                {
                    { "ID", 1 },
                    { "Nombre", "Isaac" },
                    { "Apellidos", "Ramirez" }
                };

                var row2 = new Dictionary<string, object>
                {
                    { "ID", 2 },
                    { "Nombre", "Maria" },
                    { "Apellidos", "Perez" }
                };

                status = store.InsertIntoTable("Universidad", "Estudiante", row1);
                Console.WriteLine($"Estado de la inserción de la fila 1: {status}"); // Success o Error

                status = store.InsertIntoTable("Universidad", "Estudiante", row2);
                Console.WriteLine($"Estado de la inserción de la fila 2: {status}"); // Success o Error

                // Prueba para seleccionar registros de la tabla
                Console.WriteLine("\n--- Registros en la tabla Estudiante antes de eliminar ---");
                status = store.SelectFromTable("Universidad", "Estudiante");

                // Prueba para eliminar un registro de la tabla
                status = store.DeleteFromTable("Universidad", "Estudiante", 1); // Eliminar registro con ID = 1
                Console.WriteLine($"Estado de la eliminación del registro con ID 1: {status}"); // Success o Error

                // Prueba para seleccionar registros de la tabla después de la eliminación
                Console.WriteLine("\n--- Registros en la tabla Estudiante después de eliminar ---");
                status = store.SelectFromTable("Universidad", "Estudiante");
            
          
        }
    }
}
