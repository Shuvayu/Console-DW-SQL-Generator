using DW_SQL_Generator.Models.DataModels;
using System.Collections.Generic;
using System.Text;

namespace DW_SQL_Generator.Models.LogicModels
{
    public class HashFunction
    {
        public static string Build(List<TableMapping> tableColumns, string tableName)
        {
            var resultString = new StringBuilder();
            resultString.AppendLine();
            resultString.AppendLine($"CREATE PROCEDURE [stg].[{tableName}_CreateHashValuesForAllStaging{tableName}]");
            resultString.Append($"@CurrentLoadTime DATETIME");
            resultString.AppendLine();
            resultString.AppendLine("AS");
            resultString.AppendLine("BEGIN");
            resultString.AppendLine();
            resultString.AppendLine("SET NOCOUNT ON");
            resultString.AppendLine("SET XACT_ABORT ON");
            resultString.AppendLine();
            resultString.AppendLine("-- Drop temp table if its exists");
            resultString.AppendLine($"IF OBJECT_ID('tempdb..##hvTempTable{tableName}') IS NOT NULL");
            resultString.AppendLine($"DROP TABLE #hvTempTable{tableName}");
            resultString.AppendLine();
            resultString.Append("SELECT ");
            resultString.AppendLine();

            resultString.Append("HASHBYTES('SHA2_256', ");
            resultString.AppendLine();

            foreach (var column in tableColumns)
            {
                if (column.DATA_TYPE == "varchar" ||
                    column.DATA_TYPE == "nvarchar" ||
                    column.DATA_TYPE == "char" ||
                    column.DATA_TYPE == "date" ||
                    column.DATA_TYPE == "time" ||
                    column.DATA_TYPE == "datetime" ||
                    column.DATA_TYPE == "uniqueidentifier" ||
                    column.DATA_TYPE == "Text")
                {
                    resultString.Append($"+ ISNULL([{column.COLUMN_NAME}],'') + '|'");
                    resultString.AppendLine();
                }
                else if (column.DATA_TYPE == "float" ||
                    column.DATA_TYPE == "int" ||
                    column.DATA_TYPE == "tinyint" ||
                    column.DATA_TYPE == "smallint" ||
                    column.DATA_TYPE == "bigint" ||
                    column.DATA_TYPE == "numeric" ||
                    column.DATA_TYPE == "decimal" ||
                    column.DATA_TYPE == "bit" ||
                    column.DATA_TYPE == "real")
                {
                    resultString.Append($"+ CAST(ISNULL([{column.COLUMN_NAME}],'0') AS NVARCHAR(10)) + '|'");
                    resultString.AppendLine();
                }
                else if (column.DATA_TYPE == "binary")
                {
                    continue;
                }
                else if (column.DATA_TYPE == "money")
                {
                    resultString.Append($"+ CAST(ISNULL([{column.COLUMN_NAME}],'0') AS NVARCHAR(15)) + '|'");
                    resultString.AppendLine();
                }
                else
                {
                    resultString.Append("Dont know what datatype that is !!! :");
                    resultString.Append(column.DATA_TYPE);
                    resultString.AppendLine();
                }
            }

            resultString.Append(") AS hbSource ");
            resultString.Append($"INTO #hvTempTable{tableName}");
            resultString.AppendLine($"FROM [stg].[Staging{tableName}]");
            resultString.AppendLine();
            resultString.AppendLine("-- Update the staging table");
            resultString.AppendLine($"UPDATE [stg].[Staging{tableName}]");
            resultString.AppendLine("SET hash_value = TEMP.hbSource, LoadTime = @CurrentLoadTime ");
            resultString.AppendLine($"FROM #hvTempTable{tableName} TEMP ");
            resultString.AppendLine($"INNER JOIN [stg].[Staging{ tableName}] ST");
            resultString.AppendLine($"ON ST.[ID] = TEMP.[ID] -- ADD PRIMARY KEY IDENTIFIER HERE");
            resultString.AppendLine("END");
            resultString.AppendLine();

            return resultString.ToString();
        }
    }
}