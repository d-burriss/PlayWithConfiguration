using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleInjector.Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace PlayWithConfiguration
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddXmlFile("App.config", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            var simple = new SimpleInjector.Container();
            simple.AddOptions(Configuration);

            foreach (var v in Configuration.AsEnumerable())
            {
                Console.WriteLine($"{v.Key} : {v.Value}");
            }

            var dbOptions = simple.GetInstance<IOptions<DatabaseSettings>>();

            Console.WriteLine($"DB Name : {dbOptions.Value.Name}");
            Console.ReadKey();
        }
    }
}
