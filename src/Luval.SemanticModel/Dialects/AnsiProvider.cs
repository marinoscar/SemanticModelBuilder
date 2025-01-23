using Luval.SemanticModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.SemanticModel.Dialects
{
    public class AnsiProvider : IDialectProvider
    {
        public string GetDistinctValuesForColumn(Table table, string columnName, int top)
        {
            var column = table.Columns.FirstOrDefault(i => i.ColumnName == columnName);
            if (column == null) return string.Empty;
            return $"SELECT DISTINCT TOP({top}) [{column.ColumnName}] FROM {table.QualifiedName}";
        }
    }
}
