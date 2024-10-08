using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using QueryProcessor.Parser;
using StoreDataManager;
using SystemCatalog; 


namespace QueryProcessor //De momento solo se han insertado los metodos createtable,insert y select.
{
    public class SQLQueryProcessor //Clase que procesa las consultas SQL y le dice que hacer al store data manager
    {
        public static OperationStatus Execute(string sentence) //Metodo execute que recibe una sentencia SQL en forma de texto y luego ejecuta acciones necesarias para procesarla.
        {
            //La Entrada recibida se pasa al parser para que la convierta en un objeto ParsedQuery que contiene la información de la consulta mas manejable,como en "partes".
            var parsedQuery = SQLParser.Parse(sentence);

            //Ahora se valida la consulta con el SystemCatalog,validateQuery es el metodo que utiliza el system catalog como tal, se asegura que los datos y tablas de la consulta existan.
            var validationStatus = ValidateQuery(parsedQuery);
            if (validationStatus != OperationStatus.Success)
            {
                return validationStatus; //si la validacion falla, se devuelve error.
            }

            //Si la validacion es correcta , se ejecuta la operacion correspondiente a la consulta.
            switch (parsedQuery.CommandType) //aca usa como las partes separadas anteriormente en el parser y lo trata en casos
            {
                case CommandType.CreateTable:
                    return new CreateTable().Execute(parsedQuery);
                case CommandType.Insert:
                    return new Insert().Execute(parsedQuery);
                case CommandType.Select:
                    return new Select().Execute(parsedQuery);
                case CommandType.DropTable: //Nuevo caso para DROP TABLE
                    return new DropTable().Execute(parsedQuery);
                case CommandType.CreateIndex: //Nuevo caso para CREATE INDEX
                    return new CreateIndex().Execute(parsedQuery);
                case CommandType.Update: //Nuevo caso para UPDATE
                    return new Update().Execute(parsedQuery);
                case CommandType.Delete: //Nuevo caso para DELETE
                    return new Delete().Execute(parsedQuery);
                case CommandType.SetDatabase: //Nuevo caso para SET DATABASE
                    return new SetDatabase().Execute(parsedQuery);
                case CommandType.CreateDatabase: //Nuevo caso para CREATE DATABASE
                    return new CreateDatabase().Execute(parsedQuery);
                default:
                    throw new InvalidOperationException("Unsupported command type.");
            }
        }

        //Este seria el metodo para validar la consulta con el SystemCatalog
        private static OperationStatus ValidateQuery(ParsedQuery parsedQuery)
        {
            // Validar base de datos existente (excepto para CREATE DATABASE)
            if (parsedQuery.CommandType != CommandType.CreateDatabase &&
                !SystemCatalogManager.DatabaseExists(parsedQuery.DatabaseName))
            {
                return OperationStatus.DatabaseNotFound;
            }

            // Validar tabla existente (excepto para CREATE TABLE y CREATE DATABASE)
            if (parsedQuery.CommandType != CommandType.CreateTable &&
                parsedQuery.CommandType != CommandType.CreateDatabase &&
                !SystemCatalogManager.TableExists(parsedQuery.DatabaseName, parsedQuery.TableName))
            {
                return OperationStatus.TableNotFound;
            }

            // Validar columnas para INSERT, SELECT, UPDATE
            if (parsedQuery.CommandType == CommandType.Insert ||
                parsedQuery.CommandType == CommandType.Select ||
                parsedQuery.CommandType == CommandType.Update)
            {
                var validColumns = SystemCatalogManager.GetColumns(parsedQuery.DatabaseName, parsedQuery.TableName);
                foreach (var column in parsedQuery.Columns.Keys)
                {
                    if (!validColumns.Contains(column))
                    {
                        return OperationStatus.InvalidColumn;
                    }
                }
            }

            // Validar índice para CREATE INDEX
            if (parsedQuery.CommandType == CommandType.CreateIndex)
            {
                if (SystemCatalogManager.IndexExists(parsedQuery.DatabaseName, parsedQuery.TableName, parsedQuery.IndexName))
                {
                    return OperationStatus.IndexAlreadyExists;
                }
            }

            // Validar que la base de datos no exista para CREATE DATABASE
            if (parsedQuery.CommandType == CommandType.CreateDatabase)
            {
                if (SystemCatalogManager.DatabaseExists(parsedQuery.DatabaseName))
                {
                    return OperationStatus.DatabaseAlreadyExists;
                }
            }

            // Validar que la base de datos exista para SET DATABASE
            if (parsedQuery.CommandType == CommandType.SetDatabase)
            {
                if (!SystemCatalogManager.DatabaseExists(parsedQuery.DatabaseName))
                {
                    return OperationStatus.DatabaseNotFound;
                }
            }

            // Si todo está bien
            return OperationStatus.Success;
        }
    }
}