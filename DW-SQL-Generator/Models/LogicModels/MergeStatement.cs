using DW_SQL_Generator.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DW_SQL_Generator.Models.LogicModels
{
    public class MergeStatement
    {
        public static string Build(List<TableMapping> tableColumns, string tableName, string schemaName)
        {

            var resultString = new StringBuilder();
            resultString.AppendLine();
            resultString.AppendLine($"CREATE PROCEDURE [stg].[{tableName}_MergeDataIntoDestination]");
            resultString.Append($"@CurrentLoadTime DATETIME");
            resultString.AppendLine();
            resultString.AppendLine("AS");
            resultString.AppendLine("BEGIN");
            resultString.AppendLine("BEGIN TRY");
            resultString.AppendLine();
            resultString.AppendLine("SET NOCOUNT ON");
            resultString.AppendLine("SET XACT_ABORT ON");
            resultString.AppendLine();
            resultString.AppendLine("-- Variable Declaration");
            resultString.AppendLine("DECLARE @InsertedRows INT;");
            resultString.AppendLine("DECLARE @UpdatesRows INT;");
            resultString.AppendLine("DECLARE @DeletedRows INT;");
            resultString.AppendLine();
            resultString.AppendLine("BEGIN TRANSACTION");
            resultString.AppendLine();
            resultString.AppendLine("-- Updates changed rows");
            resultString.AppendLine();
            resultString.AppendLine($"UPDATE [{schemaName}].[{tableName}]");
            resultString.AppendLine("SET");
            foreach (var column in tableColumns)
            {
                if (column.COLUMN_NAME != "LoadTime")
                {
                    resultString.AppendLine($"[{column.COLUMN_NAME}] = STAGING.[{column.COLUMN_NAME}],");
                }
                else
                {
                    resultString.AppendLine($"[{column.COLUMN_NAME}] = @CurrentLoadTime");
                }
            }
            resultString.AppendLine($"FROM [stg].[Staging{tableName}] STAGING");
            resultString.AppendLine($"LEFT JOIN [{schemaName}].[{tableName}] FINAL");
            resultString.AppendLine($"ON STAGING.[ID] = FINAL.[ID] -- ADD PRIMARY KEY IDENTIFIER HERE");
            resultString.AppendLine($"WHERE STAGING.[HashValue] <> FINAL.[HashValue];");
            resultString.AppendLine();
            resultString.AppendLine("SET @UpdatesRows = @@ROWCOUNT;");
            resultString.AppendLine();
            resultString.AppendLine("-- Insert new rows");
            resultString.AppendLine($"INSERT INTO [{schemaName}].[{tableName}]");
            resultString.Append("(");
            foreach (var column in tableColumns)
            {
                resultString.AppendLine($"[{column.COLUMN_NAME}],");
            }
            resultString.Append(") SELECT ");
            foreach (var column in tableColumns)
            {
                resultString.AppendLine($"STAGING.[{column.COLUMN_NAME}],");
            }
            resultString.AppendLine($"FROM [stg].[Staging{tableName}] STAGING");
            resultString.AppendLine($"LEFT JOIN [{schemaName}].[{tableName}] FINAL");
            resultString.AppendLine($"ON STAGING.[ID] = FINAL.[ID] -- ADD PRIMARY KEY IDENTIFIER HERE");
            resultString.AppendLine($"WHERE FINAL.[ID] IS NULL;");
            resultString.AppendLine();
            resultString.AppendLine("SET @InsertedRows = @@ROWCOUNT;");
            resultString.AppendLine();
            resultString.AppendLine("-- Delete not required rows");
            resultString.AppendLine();
            resultString.AppendLine($"UPDATE [{schemaName}].[{tableName}]");
            resultString.AppendLine("SET");
            resultString.AppendLine("[IsActive] = 0,");
            resultString.AppendLine("[DeletedTime] = @CurrentLoadTime");
            resultString.AppendLine($"FROM [stg].[Staging{tableName}] STAGING");
            resultString.AppendLine($"LEFT JOIN [{schemaName}].[{tableName}] FINAL");
            resultString.AppendLine($"ON STAGING.[ID] = FINAL.[ID] -- ADD PRIMARY KEY IDENTIFIER HERE");
            resultString.AppendLine($"WHERE STAGING.[ID] IS NULL;");
            resultString.AppendLine();
            resultString.AppendLine("SET @DeletedRows = @@ROWCOUNT;");
            resultString.AppendLine();
            resultString.AppendLine("COMMIT TRANSACTION");
            resultString.AppendLine();
            resultString.AppendLine($"PRINT Convert(Varchar(12),@UpdatesRows) + ' record(s) updated in {tableName} table.';");
            resultString.AppendLine($"PRINT Convert(Varchar(12),@InsertedRows) + ' record(s) inserted in {tableName} table.';");
            resultString.AppendLine($"PRINT Convert(Varchar(12),@DeletedRows) + ' record(s) deleted in {tableName} table.';");
            resultString.AppendLine();
            resultString.AppendLine("END TRY");
            resultString.AppendLine("BEGIN CATCH");
            resultString.AppendLine();
            resultString.AppendLine("DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();");
            resultString.AppendLine("RAISERROR(@ErrorMessage, 16, 1);");
            resultString.AppendLine("ROLLBACK;");
            resultString.AppendLine();
            resultString.AppendLine("END CATCH");
            resultString.AppendLine("END");
            resultString.AppendLine();
            return resultString.ToString();
        }
    }
}

