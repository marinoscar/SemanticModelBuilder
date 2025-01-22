using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.SemanticModel.Entities
{
    /// <summary>
    /// Represents a table in a database.
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Gets or sets the catalog name of the table.
        /// </summary>
        public string CatalogName { get; set; }

        /// <summary>
        /// Gets or sets the schema of the table.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the semantic name of the table.
        /// </summary>
        public string SemanticName { get; set; }

        /// <summary>
        /// Gets or sets the semantic description of the table.
        /// </summary>
        public string SemanticDescription { get; set; }

        /// <summary>
        /// Gets or sets the list of columns in the table.
        /// </summary>
        public List<Column> Columns { get; set; } = new List<Column>();

        /// <summary>
        /// Gets or sets the list of references for the table.
        /// </summary>
        public List<TableReference> References { get; set; } = new List<TableReference>();


        public static Table Create(IDictionary<string, object> record)
        {
            return new Table()
            {
                CatalogName = record["TABLE_CATALOG"].ToString(),
                Schema = record["TABLE_SCHEMA"].ToString(),
                TableName = record["TABLE_NAME"].ToString()
            };

        }
    }

}
