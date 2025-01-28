using Luval.DbConnectionMate;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.SemanticModel.PlugIns
{
    public class SqlServerPlugIn
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public SqlServerPlugIn(IConfiguration configuration, ILogger logger)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [KernelFunction("get_data_from_sql")]
        [Description("Gets the result of a sql script as a markdown table")]
        public async Task<string> GetDataAsync(
            [Description("The sql script to run")]
                string sqlServerScript
            )
        {
            IEnumerable<IDictionary<string, object>> result = null;
            try
            {
                using (var conn = CreateConnection())
                {
                    _logger.LogInformation("Executing the SQL script\n" + sqlServerScript);
                    result = await conn.ExecuteReaderAsync(sqlServerScript);
                    _logger.LogInformation($"SQL script executed successfully Row Count: {result.Count()} ");
                }
                return result.ToMDTable();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "An error occurred while executing the SQL script.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                throw;
            }
        }

        private SqlConnection CreateConnection()
        {
            var connStr = _config.GetConnectionString("DefaultConnection");
            return new SqlConnection(connStr);
        }
    }
}
