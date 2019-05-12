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
                Environment.Exit(0);
            }

            var dataRepository = new DataRepository(appSettings.ConnectionString);
            var columns = (await dataRepository.DbCallForSchema(appSettings.DatabaseName, appSettings.TableName, appSettings.SchemaName)).ToList();

            if (columns.Count == 0)
            {
                Console.WriteLine("Table columns are empty are empty !!!");
                Environment.Exit(0);
            }


            var sqlGenerator = new SQLGeneratorService();

            var hashSelectStatement = sqlGenerator.GenerateHashFromTemplate(columns, appSettings.TableName);
            var mergeStatement = sqlGenerator.GenerateMergeFromTemplate(columns, appSettings.TableName, appSettings.SchemaName);

            var completeText = string.Concat(hashSelectStatement, mergeStatement);

            DataRepository.OutputToTxtFile(completeText);
            
            Console.WriteLine("Task Completed !!!");
        }
    }
}
