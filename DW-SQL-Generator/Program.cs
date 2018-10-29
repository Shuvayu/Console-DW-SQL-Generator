using DW_SQL_Generator.Models.ConfigModels;
using DW_SQL_Generator.Repositories;
using DW_SQL_Generator.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
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
             .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            var appSettings = new DBSettings();
            Configuration.GetSection(nameof(DBSettings)).Bind(appSettings);

            var sqlGenerator = new SQLGeneratorService();

            var hashSelectStatement = await sqlGenerator.GenerateHashFromTemplate(appSettings);
            DataRepository.OutputToTxtFile(hashSelectStatement);

            Console.WriteLine(hashSelectStatement);
        }
    }
}
