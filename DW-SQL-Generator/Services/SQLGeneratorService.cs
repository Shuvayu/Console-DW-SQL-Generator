using DW_SQL_Generator.Models.ConfigModels;
using DW_SQL_Generator.Models.DataModels;
using DW_SQL_Generator.Models.LogicModels;
using DW_SQL_Generator.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DW_SQL_Generator.Services
{
    public class SQLGeneratorService
    {
        public string GenerateHashFromTemplate(List<TableMapping> tableColumns, string tableName)
        {
            try
            {
                return HashFunction.Build(tableColumns, tableName);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }

        }

        public string GenerateMergeFromTemplate(List<TableMapping> tableColumns, string tableName, string schemaName)
        {
            try
            {
                return MergeStatement.Build(tableColumns, tableName, schemaName);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }

        }
    }
}
