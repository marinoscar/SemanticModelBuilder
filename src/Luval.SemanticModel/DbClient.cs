using Luval.DbConnectionMate;
using Luval.SemanticModel.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Luval.SemanticModel
{
    /// <summary>
    /// Represents a database client for interacting with the database.
    /// </summary>
    public class DbClient
    {
        private readonly IDbConnection _connection;

        public static DbClient CreateSqlServer(string connectionString)
        {
            return new DbClient(new SqlConnection(connectionString));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbClient"/> class.
        /// </summary>
        /// <param name="connection">The database connection to be used by the client.</param>
        /// <exception cref="ArgumentNullException">Thrown when the connection is null.</exception>
        public DbClient(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// Asynchronously retrieves the list of tables from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of tables.</returns>
        public virtual async Task<List<Table>> GetTablesAsync()
        {
            var sql = "SELECT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
            var tables = (await _connection.ExecuteReaderAsync(sql))
                .Select(i => Table.Create(i)).ToList();
            foreach (var table in tables)
                table.Columns = await GetColumnsAsync(table);
            return tables;
        }

        /// <summary>
        /// Asynchronously retrieves the list of columns for a specified table.
        /// </summary>
        /// <param name="table">The table for which to retrieve the columns.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of columns.</returns>
        protected virtual async Task<List<Column>> GetColumnsAsync(Table table)
        {
            var sql = string.Format("SELECT COLUMN_NAME, ORDINAL_POSITION, IS_NULLABLE, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = '{0}' AND TABLE_SCHEMA = '{1}' AND TABLE_NAME = '{2}'", table.CatalogName, table.Schema, table.TableName);
            var columns = await _connection.ExecuteReaderAsync(sql);
            return columns.Select(i => Column.Create(table, i)).ToList();
        }

        /// <summary>
        /// Asynchronously retrieves the catalog from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the catalog.</returns>
        public virtual async Task<Catalog> GetCatalogAsync()
        {
            var tables = await GetTablesAsync();
            var catalog = new Catalog()
            {
                Tables = tables,
                CatalogName = tables.FirstOrDefault()?.CatalogName
            };
            catalog.Tables = tables;
            return catalog;
        }

        /// <summary>
        /// Asynchronously loads the references for a specified parent table and catalog.
        /// </summary>
        /// <param name="parentTable">The parent table for which to load the references.</param>
        /// <param name="catalog">The catalog containing the tables.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task LoadReferencesAsync(Table parentTable, Catalog catalog)
        {
            var sql = string.Format(@"
    SELECT 
					TC.CONSTRAINT_NAME,
        TC.TABLE_CATALOG AS CHILD_TABLE_CATALOG,
        TC.TABLE_SCHEMA AS CHILD_TABLE_SCHEMA,
        TC.TABLE_NAME AS CHILD_TABLE,
        KCU.COLUMN_NAME AS CHILD_COLUMN,
        CCU.TABLE_CATALOG AS PARENT_TABLE_CATALOG,
        CCU.TABLE_SCHEMA AS PARENT_TABLE_SCHEMA,
        CCU.TABLE_NAME AS PARENT_TABLE,
        CCU.COLUMN_NAME AS PARENT_COLUMN
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC
    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU
        ON TC.CONSTRAINT_NAME = KCU.CONSTRAINT_NAME
        AND TC.TABLE_SCHEMA = KCU.TABLE_SCHEMA
        AND TC.TABLE_CATALOG = KCU.TABLE_CATALOG
    JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE AS CCU
        ON CCU.CONSTRAINT_NAME = TC.CONSTRAINT_NAME
        AND CCU.TABLE_SCHEMA = TC.TABLE_SCHEMA
        AND CCU.TABLE_CATALOG = TC.TABLE_CATALOG
    WHERE TC.CONSTRAINT_TYPE = 'FOREIGN KEY'
    AND CCU.TABLE_CATALOG = '{0}'
    AND CCU.TABLE_SCHEMA = '{1}'
    AND CCU.TABLE_NAME = '{2}';
                ", parentTable.CatalogName, parentTable.Schema, parentTable.TableName);
            var references = await _connection.ExecuteReaderAsync(sql);
            foreach (var child in references)
            {
                var childColumnNames = references.Select(i => Convert.ToString(i["CHILD_COLUMN"])).ToList();
                var parentColumnNames = references.Select(i => Convert.ToString(i["PARENT_COLUMN"])).ToList();
                var childTables = catalog.Tables.Where(i => i.CatalogName == Convert.ToString(child["CHILD_TABLE_CATALOG"]) && i.Schema == Convert.ToString(child["CHILD_TABLE_SCHEMA"]) && i.TableName == Convert.ToString(child["CHILD_TABLE"]));
                var reference = new TableReference()
                {
                    ParentTable = parentTable,
                    ParentColumns = parentTable.Columns.Where(i => parentColumnNames.Contains(i.ColumnName)).ToList(),
                };
                foreach (var childTable in childTables)
                {
                    reference.ChildTable = childTable;
                    reference.ChildColumns = childTable.Columns.Where(i => childColumnNames.Contains(i.ColumnName)).ToList();
                }
                parentTable.References.Add(reference);
            }
        }
    }
}
