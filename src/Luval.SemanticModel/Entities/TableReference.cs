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
        /// Gets or sets the name of the constraint.
        /// </summary>
        public string ConstraintName { get; set; }

        /// <summary>
        /// Gets or sets the parent table in the reference.
        /// </summary>
        public Table ParentTable { get; set; }

        /// <summary>
        /// Gets or sets the list of parent columns in the reference.
        /// </summary>
        public List<Column> ParentColumns { get; set; } = new List<Column>();

        /// <summary>
        /// Gets or sets the child table in the reference.
        /// </summary>
        public Table ChildTable { get; set; }

        /// <summary>
        /// Gets or sets the list of child columns in the reference.
        /// </summary>
        public List<Column> ChildColumns { get; set; } = new List<Column>();
    }
}
