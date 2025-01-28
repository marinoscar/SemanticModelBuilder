using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

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
        public string? CatalogName { get; set; }

        /// <summary>
        /// Gets or sets the semantic name of the catalog.
        /// </summary>
        public string? SemanticName { get; set; }

        /// <summary>
        /// Gets or sets the semantic description of the catalog.
        /// </summary>
        public string? SemanticDescription { get; set; }

        /// <summary>
        /// Gets or sets the SQL engine of the catalog.
        /// </summary>
        public string? SqlEngine { get; set; }

        /// <summary>
        /// Gets or sets the list of tables in the catalog.
        /// </summary>
        public List<Table>? Tables { get; set; } = new List<Table>();

        /// <summary>
        /// Converts the catalog to a semantic model representation.
        /// </summary>
        /// <returns>A dictionary containing the semantic model representation of the catalog.</returns>
        public IDictionary<string, object> ToSemanticModel()
        {
            return new Dictionary<string, object>
            {
                { "CatalogName", CatalogName },
                { "SqlEngine", SqlEngine },
                { "Tables", Tables?.Select(i => i.ToSemanticModel()).ToList() }
            };
        }

        /// <summary>
        /// Converts the catalog object to a YAML string representation.
        /// </summary>
        /// <returns>A YAML string representation of the catalog.</returns>
        public string ToYAML()
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var model = ToSemanticModel();
            var yaml = serializer.Serialize(model);
            return yaml;
        }
    }
}
