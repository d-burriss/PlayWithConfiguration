#Overview#

Shows using the new project configuration API from ASP.NET Core in any project (not just ASP.NET Core) as well as a little extension method for hooking it up to SimpleInjector to use the Options pattern.

Check out https://docs.asp.net/en/latest/fundamentals/configuration.html

#Usage#

```cs
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
```

#The Extension Method#

```cs
    public static void AddOptions(this Container container, IConfiguration configuration)
    {
        container.ResolveUnregisteredType += (sender, e) =>
        {
            if (e.UnregisteredServiceType.IsGenericType &&
                e.UnregisteredServiceType.GetGenericTypeDefinition() == typeof(IOptions<>))
            {
                var argType = e.UnregisteredServiceType.GenericTypeArguments.First();
                var optionsType = typeof(IOptions<>).MakeGenericType(argType);
                var concreteOptionsType = typeof(OptionsWrapper<>).MakeGenericType(argType);

                var optionValue = Activator.CreateInstance(argType);
                PopulateSettingObject(configuration, argType, optionValue);

                var options = Activator.CreateInstance(concreteOptionsType, optionValue);

                e.Register(() => options);
            }
        };
    }
    private static void PopulateSettingObject<T>(IConfiguration configuration, Type argType, T optionValue) where T : class, new()
    {
        var props = argType.GetProperties();
        foreach (var p in props)
        {
            var name = p.Name;
            var value = configuration[name];
            p.SetValue(optionValue, value);
        }
    }
```
