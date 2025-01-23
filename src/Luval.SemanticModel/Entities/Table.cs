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
        /// Gets the qualified name of the table.
        /// </summary>
        public string QualifiedName => $"{CatalogName}.{Schema}.{TableName}";

        /// <summary>
        /// Gets or sets the catalog name of the table.
        /// </summary>
        public string? CatalogName { get; set; }

        /// <summary>
        /// Gets or sets the schema of the table.
        /// </summary>
        public string? Schema { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// Gets or sets the semantic name of the table.
        /// </summary>
        public string? SemanticName { get; set; }

        /// <summary>
        /// Gets or sets the semantic description of the table.
        /// </summary>
        public string? SemanticDescription { get; set; }

        /// <summary>
        /// Gets or sets the list of columns in the table.
        /// </summary>
        public List<Column>? Columns { get; set; } = new List<Column>();

        /// <summary>
        /// Gets or sets the list of references for the table.
        /// </summary>
        public List<TableReference>? References { get; set; } = new List<TableReference>();

        /// <summary>
        /// Gets or sets the list of primary keys for the table.
        /// </summary>
        public List<string> PrimaryKeys { get; set; } = new List<string>();


        /// <summary>
        /// Creates a new instance of the <see cref="Table"/> class from a record.
        /// </summary>
        /// <param name="record">The record containing table data.</param>
        /// <returns>A new instance of the <see cref="Table"/> class.</returns>
        public static Table Create(IDictionary<string, object> record)
        {
            return new Table()
            {
                CatalogName = record["TABLE_CATALOG"].ToString(),
                Schema = record["TABLE_SCHEMA"].ToString(),
                TableName = record["TABLE_NAME"].ToString()
            };

        }

        /// <summary>
        /// Converts the table to a semantic model representation.
        /// </summary>
        /// <returns>A dictionary containing the semantic model representation of the table.</returns>
        public IDictionary<string, object> ToSemanticModel()
        {
            var result = new Dictionary<string, object>();
            result.Add("CatalogName", CatalogName);
            result.Add("Schema", Schema);
            result.Add("TableName", TableName);
            result.Add("SemanticName", SemanticName);
            result.Add("Decription", SemanticDescription);
            result.Add("Columns", Columns.Select(i => i.ToSemanticModel()).ToList());
            result.Add("References", References.Select(i => i.ToSemanticModel()).ToList());
            return result;
        }
    }

}
