using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.SemanticModel.Entities
{
    public class Column
    {

        private string[] _textDataTypes = new[] { "char", "varchar", "nvarchar", "varchar2", "nvarchar2", "text" }; 

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        public string? ColumnName { get; set; }

        /// <summary>
        /// Gets the qualified name of the column.
        /// </summary>
        public string? ColumnQualifiedName => $"{TableQualifiedName}.{ColumnName}";

        /// <summary>
        /// Gets or sets the semantic name of the column.
        /// </summary>
        public string? SemanticName { get; set; }

        /// <summary>
        /// Gets or sets the semantic description of the column.
        /// </summary>
        public string? SemanticDescription { get; set; }

        /// <summary>
        /// Gets or sets the ordinal position of the column.
        /// </summary>
        public int? OrdinalPoistion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is nullable.
        /// </summary>
        public bool? IsNullable { get; set; }

        /// <summary>
        /// Gets or sets the SQL data type of the column.
        /// </summary>
        public string? SqlDataType { get; set; }

        /// <summary>
        /// Gets or sets the table to which the column belongs.
        /// </summary>
        public string? TableQualifiedName { get; set; }

        /// <summary>
        /// Indicates the type of columne where it is dimension, measure or primary key
        /// </summary>
        public string? ColumnType { get; set; }

        /// <summary>
        /// Gets or sets the list of sample values for the column.
        /// </summary>
        public List<string>? SampleValues { get; set; }

        /// <summary>
        /// Gets or sets the default aggregation for the column.
        /// </summary>
        public string? DefaultAggreation { get; set; }

        /// <summary>
        /// Gets a value indicating whether the column is a text data type.
        /// </summary>
        public bool IsText => _textDataTypes.Contains(SqlDataType?.ToLower());



        /// <summary>
        /// Creates a new instance of the <see cref="Column"/> class from a record and table.
        /// </summary>
        /// <param name="record">The record containing column data.</param>
        /// <returns>A new instance of the <see cref="Column"/> class.</returns>
        public static Column Create(IDictionary<string, object> record)
        {
            return Create(null, record);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Column"/> class from a record and table.
        /// </summary>
        /// <param name="tableQualifiedName">The table to which the column belongs.</param>
        /// <param name="record">The record containing column data.</param>
        /// <returns>A new instance of the <see cref="Column"/> class.</returns>
        public static Column Create(string tableQualifiedName, IDictionary<string, object> record)
        {
            return new Column()
            {
                ColumnName = record["COLUMN_NAME"].ToString(),
                OrdinalPoistion = Convert.ToInt32(record["ORDINAL_POSITION"]),
                IsNullable = record["IS_NULLABLE"].ToString().ToLower() == "yes",
                SqlDataType = record["DATA_TYPE"].ToString(),
                TableQualifiedName = tableQualifiedName
            };
        }



    }
}
