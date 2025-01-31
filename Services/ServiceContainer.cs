using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions; 
namespace PorcupineBot.Services
{ 
    public static class ServiceContainer
    {
        private static readonly IServiceCollection _services = new ServiceCollection();
        private static IServiceProvider _primaryServiceProvider = _services.BuildServiceProvider();
        private static readonly List<IServiceCollection> _dynamicCollections = new();
 
        public static void ConfigureServices(Action<IServiceCollection> configureServices)
        {
            var newServices = new ServiceCollection();
            configureServices(newServices);
            _dynamicCollections.Add(newServices);

            _primaryServiceProvider = BuildCompositeServiceProvider();
        }

        public static void AddSingleton<T>() where T : class, new()
        {
            var instance = new T();

            ConfigureServices(services =>
            {
                services.AddSingleton(instance); 
            });
        }

        public static void AddSingleton<T>(T instance) where T : class
        { 
            ConfigureServices(services =>
            {
                services.AddSingleton(instance);
            });
        }

        public static void AddScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        { 
            ConfigureServices(services =>
            { 
                services.AddScoped<TService, TImplementation>();
            }); 
        }

        public static T Resolve<T>() where T : notnull
        {
            var scope = _primaryServiceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }

        private static IServiceProvider BuildCompositeServiceProvider()
        {
            if (_primaryServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            var combinedServices = new ServiceCollection();
            foreach (var collection in _dynamicCollections)
            {
                foreach (var descriptor in collection)
                {
                    combinedServices.Add(descriptor);
                }
            }

            foreach (var descriptor in _services)
            {
                combinedServices.Add(descriptor);
            }

            return combinedServices.BuildServiceProvider();
        }
    }
}
