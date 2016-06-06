using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace SimpleInjector.Microsoft.Extensions.Configuration
{
    public static class SimpleInjectorOptionsExtension
    {
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

        public static void AddSettings<T>(this Container container, IConfiguration configuration) where T : class, new()
        {
            container.ResolveUnregisteredType += (sender, e) =>
            {
                if (e.UnregisteredServiceType.IsGenericType &&
                    e.UnregisteredServiceType.GetGenericTypeDefinition() == typeof(IOptions<>))
                {
                    var argType = e.UnregisteredServiceType.GenericTypeArguments.First();
                    var optionsType = typeof(IOptions<>).MakeGenericType(argType);

                    var optionValue = Activator.CreateInstance<T>();
                    PopulateSettingObject(configuration, argType, optionValue);
                    var optionsObj = Options.Create(optionValue);
                    e.Register(() => optionsObj);
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

    }
}
