using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemCatalog
{
    public class Database
    {
        public string Name { get; set; }
        public Dictionary<string, Table> Tables { get; set; }

        public Database(string name)
        {
            Name = name;
            Tables = new Dictionary<string, Table>();
        }

        public void AddTable(string tableName)
        {
            if (!Tables.ContainsKey(tableName))
            {
                Tables.Add(tableName, new Table(tableName));
            }
        }

        public bool TableExists(string tableName)
        {
            return Tables.ContainsKey(tableName);
        }

        public Table GetTable(string tableName)
        {
            if (Tables.ContainsKey(tableName))
            {
                return Tables[tableName];
            }
            throw new InvalidOperationException("Table not found.");
        }
        public void RemoveTable(string tableName)
        {
            if (Tables.ContainsKey(tableName))
            {
                Tables.Remove(tableName);
            }
            else
            {
                throw new InvalidOperationException("Table not found.");
            }
        }
    }
}
