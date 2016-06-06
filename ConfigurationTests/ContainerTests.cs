using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleInjector;
using SimpleInjector.Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConfigurationTests
{
    public class ContainerTests
    {
        [Fact]
        public void AddOptions_CallIOptions_ReturnsInstance()
        {
            var container = new Container();
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddXmlFile("App.config", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            container.AddOptions(configuration);

            var value = container.GetInstance<IOptions<DatabaseSettings>>();

            Assert.NotNull(value);
        }

        [Fact]
        public void AddOptions_CallIOptionsTwice_ReturnsInstance()
        {
            var container = new Container();
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddXmlFile("App.config", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            container.AddOptions(configuration);

            var value1 = container.GetInstance<IOptions<DatabaseSettings>>();
            var value2 = container.GetInstance<IOptions<DatabaseSettings>>();

            Assert.NotNull(value2);
        }
    }
}
