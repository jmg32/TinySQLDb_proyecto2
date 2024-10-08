using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemCatalog
{
    public class Row
    {
        public Dictionary<string, object> Data { get; private set; }

        public Row()
        {
            Data = new Dictionary<string, object>();
        }

        public object GetValue(string columnName)
        {
            if (Data.ContainsKey(columnName))
            {
                return Data[columnName];
            }
            throw new InvalidOperationException("Column not found.");
        }
        public bool TryGetValue(string columnName, out object value)
        {
            return Data.TryGetValue(columnName, out value);
        }

        public void SetValue(string columnName, object value)
        {
            Data[columnName] = value;
        }
    }
}

