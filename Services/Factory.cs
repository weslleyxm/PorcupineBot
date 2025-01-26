using Microsoft.Extensions.DependencyInjection;

namespace PorcupineBot.Services
{
    public class Factory
    {
        public static T Create<T>(Type type, params object[] args) where T : class
        {
            var serviceProvider = ServiceContainer.Resolve<IServiceProvider>();
            var instance = (T)ActivatorUtilities.CreateInstance(serviceProvider, type, args); 
            return instance;
        }
    }
}
