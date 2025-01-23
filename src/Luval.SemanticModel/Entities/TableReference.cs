using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.SemanticModel.Entities
{
    /// <summary>
    /// Represents a reference between tables in a database.
    /// </summary>
    public class TableReference
    {
        /// <summary>
        /// Gets the name of the parent catalog.
        /// </summary>
        public string ParentCatalogName => GetParts(ParentTableQualifiedName, 0);

        /// <summary>
        /// Gets the schema of the parent table.
        /// </summary>
        public string ParentSchema => GetParts(ParentTableQualifiedName, 1);

        /// <summary>
        /// Gets the name of the parent table.
        /// </summary>
        public string ParentTableName => GetParts(ParentTableQualifiedName, 2);

        /// <summary>
        /// Gets the name of the child catalog.
        /// </summary>
        public string ChildCatalogName => GetParts(ChildTableQualifiedName, 0);

        /// <summary>
        /// Gets the schema of the child table.
        /// </summary>
        public string ChildSchema => GetParts(ChildTableQualifiedName, 1);

        /// <summary>
        /// Gets the name of the child table.
        /// </summary>
        public string ChildTableName => GetParts(ChildTableQualifiedName, 2);

        /// <summary>
        /// Gets or sets the name of the constraint.
        /// </summary>
        public string? ConstraintName { get; set; }

        /// <summary>
        /// Gets or sets the parent table in the reference.
        /// </summary>
        public string? ParentTableQualifiedName { get; set; }

        /// <summary>
        /// Gets or sets the list of parent columns in the reference.
        /// </summary>
        public List<string>? ParentColumns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the child table in the reference.
        /// </summary>
        public string? ChildTableQualifiedName { get; set; }

        /// <summary>
        /// Gets or sets the list of child columns in the reference.
        /// </summary>
        public List<string>? ChildColumns { get; set; } = new List<string>();


        /// <summary>
        /// Converts the table reference to a semantic model representation.
        /// </summary>
        /// <returns>A dictionary containing the semantic model representation of the table reference.</returns>
        public IDictionary<string, object> ToSemanticModel()
        {
            return new Dictionary<string, object>()
            {

                { "ParentTableName", ParentTableName },
                { "ParentColumns", ParentColumns.Select(i => GetParts(i, 3)).ToList() },
                { "ChildTableName", ChildTableName },
                { "ChildColumns", ChildColumns.Select(i => GetParts(i, 3)).ToList() }
            };
        }

        private string GetParts(string qualifiedName, int idx)
        {
            if (string.IsNullOrWhiteSpace(qualifiedName)) return default!;
            var parts = qualifiedName.Split('.');
            if (idx < 0) return default!;
            if (idx >= parts.Length) return default!;
            return parts[idx];
        }
    }
}
