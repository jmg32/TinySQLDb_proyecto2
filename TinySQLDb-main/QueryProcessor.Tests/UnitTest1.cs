
using QueryProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryProcessor.Exceptions;
using QueryProcessor.Parser;
using SystemCatalog;
using StoreDataManager;

namespace QueryProcessor.Tests
{
    [TestClass]
    public class SQLQueryProcessorTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            // Simulamos la inicialización del SystemCatalog y las bases de datos para las pruebas.
            SystemCatalogManager.Reset(); // Asume que Reset limpia las bases de datos y tablas.

            // Crea una base de datos para las pruebas
            SystemCatalogManager.CreateDatabase("TestDatabase");
            SystemCatalogManager.SetActiveDatabase("TestDatabase");
        }

        [TestMethod]
        public void Execute_CreateTable_Success()
        {
            // Arrange
            string query = "CREATE TABLE TestTable (ID INT, Name VARCHAR(50));";

            // Act
            var result = SQLQueryProcessor.Execute(query);

            // Assert
            Assert.AreEqual(Entities.OperationStatus.Success, result);

            // Verificar si la tabla fue creada en el SystemCatalog
            Assert.IsTrue(SystemCatalogManager.TableExists("TestDatabase", "TestTable"));
        }

        [TestMethod]
        public void Execute_Insert_Success()
        {
            // Arrange
            // Crear la tabla primero
            SQLQueryProcessor.Execute("CREATE TABLE TestTable (ID INT, Name VARCHAR(50));");
            string query = "INSERT INTO TestTable (ID, Name) VALUES (1, 'John Doe');";

            // Act
            var result = SQLQueryProcessor.Execute(query);

            // Assert
            Assert.AreEqual(Entities.OperationStatus.Success, result);

            // Verificar si el dato fue "insertado" en el SystemCatalog (simulación)
            var rows = SystemCatalogManager.GetRows("TestDatabase", "TestTable", null);
            Assert.AreEqual(1, rows.Count);
        }

        [TestMethod]
        public void Execute_Select_Success()
        {
            // Arrange
            // Crear tabla e insertar datos
            SQLQueryProcessor.Execute("CREATE TABLE TestTable (ID INT, Name VARCHAR(50));");
            SQLQueryProcessor.Execute("INSERT INTO TestTable (ID, Name) VALUES (1, 'John Doe');");
            string query = "SELECT ID, Name FROM TestTable;";

            // Act
            var result = SQLQueryProcessor.Execute(query);

            // Assert
            Assert.AreEqual(Entities.OperationStatus.Success, result);

            // Simulamos que el Select retorne los datos correctos.
            var rows = SystemCatalogManager.GetRows("TestDatabase", "TestTable", null);
            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual("John Doe", rows[0].GetValue("Name"));
        }

        [TestMethod]
        public void Execute_Select_TableNotFound()
        {
            // Arrange
            string query = "SELECT ID, Name FROM NonExistentTable;";

            // Act
            var result = SQLQueryProcessor.Execute(query);

            // Assert
            Assert.AreEqual(Entities.OperationStatus.TableNotFound, result);
        }

        [TestMethod]
        public void Execute_CreateDatabase_DatabaseAlreadyExists()
        {
            // Arrange
            string query = "CREATE DATABASE TestDatabase;"; // Ya se creó en TestInitialize

            // Act
            var result = SQLQueryProcessor.Execute(query);

            // Assert
            Assert.AreEqual(Entities.OperationStatus.DatabaseAlreadyExists, result);
        }

        [TestMethod]
        public void Execute_Insert_InvalidColumn()
        {
            // Arrange
            // Crear tabla
            SQLQueryProcessor.Execute("CREATE TABLE TestTable (ID INT, Name VARCHAR(50));");
            string query = "INSERT INTO TestTable (InvalidColumn) VALUES (1);";

            // Act
            var result = SQLQueryProcessor.Execute(query);

            // Assert
            Assert.AreEqual(Entities.OperationStatus.InvalidColumn, result);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Limpieza después de cada prueba
            SystemCatalogManager.Reset(); // Asume que Reset limpia los datos del SystemCatalog
        }
    }
}
