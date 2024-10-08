using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemCatalog
{

    public class Column
    {
        public string Name { get; set; }
        public string DataType { get; set; } // Tipo de datos, por ejemplo, "int", "varchar", etc.
        public bool IsPrimaryKey { get; set; }

        public Column(string name, string dataType, bool isPrimaryKey = false)
        {
            Name = name;
            DataType = dataType;
            IsPrimaryKey = isPrimaryKey;
        }
    }
    
}
