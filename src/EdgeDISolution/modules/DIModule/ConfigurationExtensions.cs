using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.DependencyInjection;

namespace DIModule
{
    public static class ConfigurationExtensions 
    {
     
        // Adds the IModuleClient to the service collection
        public static ServiceCollection AddModuleClient(this ServiceCollection serviceCollection, ITransportSettings transportSettings)
        {
            serviceCollection.AddSingleton<IModuleClient>((sp) => {
                ITransportSettings[] settings = { transportSettings };

                var ioTHubModuleClient = ModuleClient.CreateFromEnvironmentAsync(settings).GetAwaiter().GetResult();
                return new ModuleClientWrapper(ioTHubModuleClient);
            });

            return serviceCollection;
        }
    }

}