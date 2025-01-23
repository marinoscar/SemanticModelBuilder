using Luval.SemanticModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.SemanticModel.Dialects
{
    public interface IDialectProvider
    {
        string GetDistinctValuesForColumn(Table table, string columnName, int top);
    }
}
