using DW_SQL_Generator.Models.DataModels;
using System.Collections.Generic;
using System.Text;

namespace DW_SQL_Generator.Models.LogicModels
{
    public class HashFunction
    {
        public static string Build(List<TableMapping> tableColumns)
        {

            var resultString = new StringBuilder();

            resultString.Append("SELECT ");
            resultString.AppendLine();

            resultString.Append("HASHBYTES('SHA2_256', ");
            resultString.AppendLine();

            foreach (var column in tableColumns)
            {
                if (column.DATA_TYPE == "varchar" ||
                    column.DATA_TYPE == "nvarchar" ||
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
                    resultString.Append("Dont know what datatype that is !!!");
                    resultString.AppendLine();
                }
            }

            resultString.Append(") AS hbSource ");
            resultString.AppendLine();

            return resultString.ToString();
        }
    }
}
