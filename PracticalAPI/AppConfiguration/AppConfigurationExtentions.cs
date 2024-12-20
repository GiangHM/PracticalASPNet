using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace PracticalAPI.AppConfiguration
{
    /// <summary>
    /// Why use app configruation: https://learn.microsoft.com/en-us/azure/azure-app-configuration/overview#why-use-app-configuration
    /// For me,
    /// 1. Change configuration value => no need to restart app
    /// 2. Centralize the configuration
    /// </summary>
    public static class AppConfigurationExtentions
    {
        /// <summary>
        /// This method for advance use case to:
        /// Reduce request to App Configuration to check sentinel key and refresh value
        /// The refresh interval can be use to maximize value if your app doen't change config frequently
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddAppConfigurationWithSentinelKey(this ConfigurationManager configuration)
        {
            var appConfigEndpoint = configuration.GetValue<string>("appConfigEndpoint");
            var manageIdentity = configuration.GetValue<string>("ClientId") ?? null;

            return configuration.AddAzureAppConfiguration(option =>
            {
                option.Connect(new Uri(appConfigEndpoint), new ChainedTokenCredential(
                    new VisualStudioCredential(),
                    new ManagedIdentityCredential(manageIdentity)
                    ));
                // Load all keys that start with `TestApp:` and have no label
                option.Select("TestApp:*", LabelFilter.Null)
                // Configure to reload configuration if the registered sentinel key is modified
                .ConfigureRefresh(refreshOptions =>
                {
                    refreshOptions.Register("SentinelKey", refreshAll: true);
                    // Set referesh interval to reduce the request checking sentinel key to refresh configs
                    refreshOptions.SetRefreshInterval(new TimeSpan(500));
                });

                // If use App config with Azure Key Valaut
                option.ConfigureKeyVault(keyValaultOptions =>
                {
                    keyValaultOptions.SetSecretRefreshInterval(new TimeSpan(500));
                    keyValaultOptions.SetCredential(new ChainedTokenCredential(
                        new VisualStudioCredential(),
                        new ManagedIdentityCredential(manageIdentity)
                        ));
                });
            });
        }

        /// <summary>
        /// This method for normal use case, but there are requests to app configuration to update latest value
        /// Should consider when use it
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddAppConfiguration(this ConfigurationManager configuration)
        {
            var appConfigEndpoint = configuration.GetValue<string>("appConfigEndpoint");
            var manageIdentity = configuration.GetValue<string>("ClientId") ?? null;

            return configuration.AddAzureAppConfiguration(option =>
            {
                option.Connect(new Uri(appConfigEndpoint), new ChainedTokenCredential(
                    new VisualStudioCredential(),
                    new ManagedIdentityCredential(manageIdentity)
                    ));

                // If use App config with Azure Key Valaut
                option.ConfigureKeyVault(keyValaultOptions =>
                {
                    keyValaultOptions.SetSecretRefreshInterval(new TimeSpan(500));
                    keyValaultOptions.SetCredential(new ChainedTokenCredential(
                        new VisualStudioCredential(),
                        new ManagedIdentityCredential(manageIdentity)
                        ));
                });
            });
        }
    }
}
