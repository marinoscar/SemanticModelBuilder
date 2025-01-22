using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.SemanticModel.Entities
{
    /// <summary>
    /// Represents a catalog in a database.
    /// </summary>
    public class Catalog
    {
        /// <summary>
        /// Gets or sets the name of the catalog.
        /// </summary>
        public string CatalogName { get; set; }

        /// <summary>
        /// Gets or sets the semantic name of the catalog.
        /// </summary>
        public string SemanticName { get; set; }

        /// <summary>
        /// Gets or sets the semantic description of the catalog.
        /// </summary>
        public string SemanticDescription { get; set; }

        /// <summary>
        /// Gets or sets the SQL engine of the catalog.
        /// </summary>
        public string SqlEngine { get; set; }

        /// <summary>
        /// Gets or sets the list of tables in the catalog.
        /// </summary>
        public List<Table> Tables { get; set; } = new List<Table>();
    }

}
