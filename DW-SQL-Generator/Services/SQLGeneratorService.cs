using DW_SQL_Generator.Models.ConfigModels;
using DW_SQL_Generator.Models.LogicModels;
using DW_SQL_Generator.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DW_SQL_Generator.Services
{
    public class SQLGeneratorService
    {
        public async Task<string> GenerateHashFromTemplate(DBSettings settings)
        {
            try
            {
                if (string.IsNullOrEmpty(settings.ConnectionString) || string.IsNullOrEmpty(settings.DatabaseName) || string.IsNullOrEmpty(settings.TableName))
                {
                    Console.WriteLine("Parameters are empty !!!");
                }

                var dataRepository = new DataRepository(settings.ConnectionString);
                var columns = await dataRepository.DbCallForSchema(settings.DatabaseName, settings.TableName, settings.SchemaName);

                var BuiltHashFunction = BuildHashFunction.BuildStatement(columns.ToList());

                return BuiltHashFunction;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }

        }
    }
}
