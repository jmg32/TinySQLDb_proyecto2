using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryProcessor.Exceptions;

namespace QueryProcessor.Parser
{
    public class SQLParser
    {
        public static ParsedQuery Parse(string sentence)
        {
            // Asegúrate de que el SQL no tenga espacios innecesarios.
            sentence = sentence.Trim();

            // Identificamos qué tipo de comando es.
            if (sentence.StartsWith("CREATE DATABASE", StringComparison.OrdinalIgnoreCase))
            {
                return ParseCreateDatabase(sentence);
            }
            else if (sentence.StartsWith("SET DATABASE", StringComparison.OrdinalIgnoreCase))
            {
                return ParseSetDatabase(sentence);
            }
            else if (sentence.StartsWith("CREATE TABLE", StringComparison.OrdinalIgnoreCase))
            {
                return ParseCreateTable(sentence);
            }
            else if (sentence.StartsWith("INSERT INTO", StringComparison.OrdinalIgnoreCase))
            {
                return ParseInsert(sentence);
            }
            else if (sentence.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return ParseSelect(sentence);
            }
            else if (sentence.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase))
            {
                return ParseUpdate(sentence);
            }
            else if (sentence.StartsWith("DELETE FROM", StringComparison.OrdinalIgnoreCase))
            {
                return ParseDelete(sentence);
            }
            else if (sentence.StartsWith("CREATE INDEX", StringComparison.OrdinalIgnoreCase))
            {
                return ParseCreateIndex(sentence);
            }
            else if (sentence.StartsWith("DROP TABLE", StringComparison.OrdinalIgnoreCase))
            {
                return ParseDropTable(sentence);
            }
            else
            {
                throw new SQLParserException("Unknown or unsupported SQL command.");
            }
        }

        public static ParsedQuery ParseCreateDatabase(string sentence)
        {
            string[] parts = sentence.Split(' ');
            if (parts.Length != 3 || parts[0].ToUpper() != "CREATE" || parts[1].ToUpper() != "DATABASE")
            {
                throw new SQLParserException("Syntax error in CREATE DATABASE statement.");
            }
            string databaseName = parts[2].Trim();

            return new ParsedQuery
            {
                CommandType = CommandType.CreateDatabase,
                DatabaseName = databaseName
            };
        }

        public static ParsedQuery ParseDropTable(string sentence)
        {
            string[] parts = sentence.Split(' ');

            // Verifica que la sentencia sea correcta
            if (parts.Length != 3 || parts[0].ToUpper() != "DROP" || parts[1].ToUpper() != "TABLE")
            {
                throw new SQLParserException("Syntax error in DROP TABLE statement.");
            }

            string tableName = parts[2].Trim();

            return new ParsedQuery
            {
                CommandType = CommandType.DropTable,
                TableName = tableName
            };
        }

        public static ParsedQuery ParseSetDatabase(string sentence)
        {
            string[] parts = sentence.Split(' ');
            if (parts.Length != 3 || parts[0].ToUpper() != "SET" || parts[1].ToUpper() != "DATABASE")
            {
                throw new SQLParserException("Syntax error in SET DATABASE statement.");
            }
            string databaseName = parts[2].Trim();

            return new ParsedQuery
            {
                CommandType = CommandType.SetDatabase,
                DatabaseName = databaseName
            };
        }

        public static ParsedQuery ParseCreateTable(string sentence)
        {
            string withoutCreateTable = sentence.Substring("CREATE TABLE".Length).Trim();

            int openParenIndex = withoutCreateTable.IndexOf('(');
            int closeParenIndex = withoutCreateTable.LastIndexOf(')');

            if (openParenIndex == -1 || closeParenIndex == -1)
            {
                throw new SQLParserException("Syntax error in CREATE TABLE statement.");
            }

            string tableName = withoutCreateTable.Substring(0, openParenIndex).Trim();
            string columnsPart = withoutCreateTable.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1).Trim();
            var columns = new Dictionary<string, object>();

            var columnDefinitions = columnsPart.Split(',');

            foreach (var columnDefinition in columnDefinitions)
            {
                var parts = columnDefinition.Trim().Split(' ');
                if (parts.Length < 2)
                {
                    throw new SQLParserException("Syntax error in column definition.");
                }

                string columnName = parts[0].Trim();
                string columnType = parts[1].Trim();

                columns[columnName] = columnType;
            }

            return new ParsedQuery
            {
                CommandType = CommandType.CreateTable,
                TableName = tableName,
                Columns = columns
            };
        }
        public static ParsedQuery ParseDelete(string sentence)
        {
            string withoutDelete = sentence.Substring("DELETE FROM".Length).Trim();
            string[] parts = withoutDelete.Split(new string[] { "WHERE" }, StringSplitOptions.None);

            if (parts.Length == 0)
            {
                throw new SQLParserException("Syntax error in DELETE statement.");
            }

            string tableName = parts[0].Trim();
            var conditions = ParseConditions(parts.Length == 2 ? parts[1].Trim() : null);

            return new ParsedQuery
            {
                CommandType = CommandType.Delete,
                TableName = tableName,
                Conditions = conditions
            };
        }
        public static ParsedQuery ParseCreateIndex(string sentence)
        {
            string[] parts = sentence.Split(' ');

            if (parts.Length != 6 || parts[0].ToUpper() != "CREATE" || parts[1].ToUpper() != "INDEX")
            {
                throw new SQLParserException("Syntax error in CREATE INDEX statement.");
            }

            string indexName = parts[2].Trim();
            string tableName = parts[4].Trim();
            string columnName = parts[5].Trim('(', ')');
            string indexTypeString = parts.Length == 7 ? parts[6].ToUpper() : "BTREE"; // Default BTREE

            SystemCatalog.Index.IndexType indexType;
            if (!Enum.TryParse(indexTypeString, true, out indexType))
            {
                throw new SQLParserException("Invalid index type.");
            }

            return new ParsedQuery
            {
                CommandType = CommandType.CreateIndex,
                IndexName = indexName,
                TableName = tableName,
                ColumnName = columnName,
                IndexType = indexType.ToString() // Convertimos el enum a string
            };
        }

        public static Dictionary<string, object> ParseConditions(string whereClause)
        {
            if (string.IsNullOrEmpty(whereClause)) return null;

            var conditions = new Dictionary<string, object>();
            string[] conditionParts = whereClause.Split(new string[] { "AND", "OR" }, StringSplitOptions.None);

            foreach (var condition in conditionParts)
            {
                string[] parts = condition.Split('=');
                if (parts.Length == 2)
                {
                    string column = parts[0].Trim();
                    object value = parts[1].Trim().Trim('\''); // Remove quotes
                    conditions[column] = value;
                }
            }

            return conditions;
        }
        public static ParsedQuery ParseUpdate(string sentence)
        {
            string withoutUpdate = sentence.Substring("UPDATE".Length).Trim();
            string[] parts = withoutUpdate.Split(new string[] { "SET", "WHERE" }, StringSplitOptions.None);

            if (parts.Length < 2)
            {
                throw new SQLParserException("Syntax error in UPDATE statement.");
            }

            string tableName = parts[0].Trim();
            string[] setParts = parts[1].Split('=');
            if (setParts.Length != 2)
            {
                throw new SQLParserException("Syntax error in SET clause of UPDATE.");
            }

            string columnName = setParts[0].Trim();
            object newValue = setParts[1].Trim().Trim('\'');

            var conditions = ParseConditions(parts.Length == 3 ? parts[2].Trim() : null);

            return new ParsedQuery
            {
                CommandType = CommandType.Update,
                TableName = tableName,
                ColumnName = columnName,
                NewValue = newValue,
                Conditions = conditions
            };
        }

        public static ParsedQuery ParseInsert(string sentence)
        {
            string withoutInsertInto = sentence.Substring("INSERT INTO".Length).Trim();

            int openParenIndex = withoutInsertInto.IndexOf('(');
            int closeParenIndex = withoutInsertInto.IndexOf(')', openParenIndex);

            if (openParenIndex == -1 || closeParenIndex == -1)
            {
                throw new SQLParserException("Syntax error in INSERT INTO statement.");
            }

            string tableName = withoutInsertInto.Substring(0, openParenIndex).Trim();
            string columnsPart = withoutInsertInto.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1).Trim();
            var columns = columnsPart.Split(',').Select(c => c.Trim()).ToList();

            int valuesIndex = withoutInsertInto.IndexOf("VALUES", closeParenIndex);
            if (valuesIndex == -1)
            {
                throw new SQLParserException("Missing VALUES keyword in INSERT INTO statement.");
            }

            string valuesPart = withoutInsertInto.Substring(valuesIndex + "VALUES".Length).Trim();
            valuesPart = valuesPart.Trim('(', ')');
            var values = valuesPart.Split(',').Select(v => v.Trim('\'', ' ')).ToList();

            if (columns.Count != values.Count)
            {
                throw new SQLParserException("Number of columns and values do not match.");
            }

            var rowData = new Dictionary<string, object>();
            for (int i = 0; i < columns.Count; i++)
            {
                rowData[columns[i]] = values[i];
            }

            return new ParsedQuery
            {
                CommandType = CommandType.Insert,
                TableName = tableName,
                Values = rowData
            };
        }

        public static ParsedQuery ParseSelect(string sentence)
        {
            string withoutSelect = sentence.Substring("SELECT".Length).Trim();
            string[] parts = withoutSelect.Split(new string[] { "FROM" }, StringSplitOptions.None);

            if (parts.Length != 2)
            {
                throw new SQLParserException("Syntax error in SELECT statement.");
            }

            string columnsPart = parts[0].Trim();
            string tableName = parts[1].Trim();

            var selectedColumns = columnsPart == "*" ? new List<string>() : columnsPart.Split(',').Select(c => c.Trim()).ToList();

            return new ParsedQuery
            {
                CommandType = CommandType.Select,
                TableName = tableName,
                SelectedColumns = selectedColumns
            };
        }
    }

    public class ParsedQuery
    {
        public CommandType CommandType { get; set; }
        public string DatabaseName { get; set; }   // Para sentencias relacionadas con la base de datos
        public string TableName { get; set; }      // Para sentencias que operan sobre tablas
        public Dictionary<string, object> Columns { get; set; }  // Columnas para CREATE TABLE
        public Dictionary<string, object> Values { get; set; }   // Valores para INSERT y UPDATE
        public List<string> SelectedColumns { get; set; }        // Columnas seleccionadas en SELECT
        public Dictionary<string, object> Conditions { get; set; } // Condiciones WHERE para SELECT, DELETE, UPDATE
        public string ColumnName { get; set; }    // Nombre de columna en operaciones como UPDATE
        public object NewValue { get; set; }      // Nuevo valor para UPDATE
        public string IndexName { get; set; }     // Nombre del índice para CREATE INDEX
        public string IndexType { get; set; }     // Tipo de índice (BTREE o BST)
        public string WhereClause { get; set; } // Simplemente un string
    }

    public enum CommandType
    {
        CreateTable,
        Insert,
        Select,
        CreateDatabase,
        SetDatabase,
        Update,
        Delete,
        CreateIndex,
        DropTable
    }
}

