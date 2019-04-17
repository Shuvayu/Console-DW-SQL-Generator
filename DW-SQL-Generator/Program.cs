using DW_SQL_Generator.Models.ConfigModels;
using DW_SQL_Generator.Repositories;
using DW_SQL_Generator.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DW_SQL_Generator
{
    internal class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();

            Console.ReadKey();
        }

        private static async Task MainAsync()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile($"appSettings.development.json");

            Configuration = builder.Build();

            var appSettings = new DBSettings();
            Configuration.GetSection(nameof(DBSettings)).Bind(appSettings);

            if (appSettings.IsNotValid())
            {
                Console.WriteLine("Parameters are empty !!!");
            }

            var dataRepository = new DataRepository(appSettings.ConnectionString);
            var columns = (await dataRepository.DbCallForSchema(appSettings.DatabaseName, appSettings.TableName, appSettings.SchemaName)).ToList();

            var sqlGenerator = new SQLGeneratorService();

            var hashSelectStatement = sqlGenerator.GenerateHashFromTemplate(columns);
            var mergeStatement = sqlGenerator.GenerateMergeFromTemplate(columns, appSettings.TableName, appSettings.SchemaName);

            DataRepository.OutputToTxtFile(hashSelectStatement);
            DataRepository.OutputToTxtFile(mergeStatement);
            
            Console.WriteLine("Task Completed !!!");
        }
    }
}
