namespace Orc.DynamicObjects.Tests
{
    using Catel;
    using Microsoft.Extensions.DependencyInjection;
    using Orc.DynamicObjects;

    internal static class ServiceCollectionHelper
    {
        public static IServiceCollection CreateServiceCollection()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging();
            serviceCollection.AddCatelCore();
            serviceCollection.AddOrcDynamicObjects();

            return serviceCollection;
        }
    }
}
