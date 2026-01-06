namespace Orc
{
    using Catel.Services;
    using Catel.ThirdPartyNotices;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcDynamicObjectsModule
    {
        public static IServiceCollection AddOrcDynamicObjects(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.DynamicObjects", "Orc.DynamicObjects.Properties", "Resources"));

            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new LibraryThirdPartyNotice("Orc.DynamicObjects", "https://github.com/wildgums/orc.dynamicobjects"));

            return serviceCollection;
        }
    }
}
