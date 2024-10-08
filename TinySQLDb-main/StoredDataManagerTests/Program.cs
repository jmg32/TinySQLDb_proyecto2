// See https://aka.ms/new-console-template for more information
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

            // Prueba para crear una base de datos
            var status = store.CreateDatabase("Universidad");
            Console.WriteLine($"Estado de la creación de la base de datos: {status}"); // Success o DatabaseAlreadyExists

            // Prueba para crear una tabla en la base de datos
            var columns = new List<ColumnDefinition>
            {
                new ColumnDefinition("ID", "INTEGER"),
                new ColumnDefinition("Nombre", "VARCHAR(30)"),
                new ColumnDefinition("Apellido", "VARCHAR(50)")
            };
            status = store.CreateTable("Universidad", "Estudiante", columns);
            Console.WriteLine($"Estado de la creación de la tabla: {status}"); // Success o DatabaseNotFound
        }
    }
}
