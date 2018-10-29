using Dapper;
using DW_SQL_Generator.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace DW_SQL_Generator.Repositories
{
    public class DataRepository
    {
        private readonly string _connectionString;

        public DataRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<TableMapping>> DbCallForSchema(string databaseName, string tableName, string schemaName)
        {
            var parameters = new DynamicParameters();
            IEnumerable<TableMapping> result;

            parameters.Add("TableName", tableName, DbType.String, ParameterDirection.Input);
            parameters.Add("SchemaName", schemaName, DbType.String, ParameterDirection.Input);

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var query = string.Format(Queries.getColumnNamesAndTypes, databaseName);
                result = await sqlConnection.QueryAsync<TableMapping>(query, parameters);
            }
            return result;

        }

        public static void OutputToTxtFile(string txtString)
        {
            using (var file = new StreamWriter(@"Output.txt"))
            {
                file.WriteLine(txtString);
                file.WriteLine(Environment.NewLine);
                file.Close();
            }
        }
    }
}
