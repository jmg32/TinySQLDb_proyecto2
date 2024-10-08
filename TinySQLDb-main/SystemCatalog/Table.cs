using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SystemCatalog.Index;

namespace SystemCatalog
{
    public class Table
    {
        public string Name { get; set; }
        public List<Column> Columns { get; set; }
        public List<Row> Rows { get; private set; }
        private Dictionary<string, Index> indexes = new Dictionary<string, Index>();

        public Table(string name)
        {
            Name = name;
            Columns = new List<Column>();
            Rows = new List<Row>();
        }

        public void AddColumn(string columnName, string dataType, bool isPrimaryKey = false)
        {
            Columns.Add(new Column(columnName, dataType, isPrimaryKey));
        }

        public void AddRow(Row row)
        {
            Rows.Add(row);
        }

        public bool IsEmpty()
        {
            return Rows.Count == 0;
        }

        public bool DeleteRow(string primaryKeyValue)
        {
            var primaryKeyColumn = Columns.FirstOrDefault(c => c.IsPrimaryKey);
            if (primaryKeyColumn == null)
            {
                throw new InvalidOperationException("Primary key not defined.");
            }

            // Buscar la fila donde la clave primaria coincida con el valor proporcionado
            var rowToDelete = Rows.FirstOrDefault(row =>
                row.GetValue(primaryKeyColumn.Name).ToString() == primaryKeyValue);

            if (rowToDelete != null)
            {
                Rows.Remove(rowToDelete);
                return true;
            }
            return false;
        }

        public bool HasColumn(string columnName)
        {
            return Columns.Any(c => c.Name == columnName);
        }

        public bool IndexExists(string indexName)
        {
            return indexes.ContainsKey(indexName);
        }

        public void CreateIndex(string indexName, string columnName, IndexType indexType)
        {
            if (indexes.ContainsKey(indexName))
            {
                throw new InvalidOperationException("Index already exists.");
            }

            var column = Columns.FirstOrDefault(c => c.Name == columnName);
            if (column == null)
            {
                throw new InvalidOperationException("Column not found.");
            }

            indexes.Add(indexName, new Index(indexName, columnName, indexType));
        }

        public bool HasIndexOnColumn(string columnName)
        {
            return indexes.Values.Any(index => index.ColumnName == columnName);
        }

        public void UpdateIndexAfterDelete(string columnName, Row deletedRow)
        {
            var index = indexes.Values.FirstOrDefault(idx => idx.ColumnName == columnName);
            if (index != null)
            {
                index.Remove(deletedRow);
            }
        }
    }
}