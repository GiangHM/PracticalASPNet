using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace PracticalAPI.AppConfiguration
{
    /// <summary>
    /// Why use app configuration: https://learn.microsoft.com/en-us/azure/azure-app-configuration/overview#why-use-app-configuration
    /// For me,
    /// 1. Change configuration value => no need to restart app
    /// 2. Centralize the configuration
    /// </summary>
    public static class AppConfigurationExtensions
    {
        /// <summary>
        /// This method is for advanced use cases to:
        /// Reduce requests to App Configuration to check sentinel key and refresh value.
        /// The refresh interval can be used to maximize value if your app doesn't change config frequently.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddAppConfigurationWithSentinelKey(this ConfigurationManager configuration)
        {
            var appConfigEndpoint = configuration.GetValue<string>("appConfigEndpoint");
            var managedIdentity = configuration.GetValue<string>("ClientId");

            return configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(appConfigEndpoint), new ChainedTokenCredential(
                    new VisualStudioCredential(),
                    new ManagedIdentityCredential(managedIdentity)
                ));
                // Load all keys that start with `TestApp:` and have no label
                options.Select("TestApp:*", LabelFilter.Null)
                // Configure to reload configuration if the registered sentinel key is modified
                .ConfigureRefresh(refreshOptions =>
                {
                    refreshOptions.Register("SentinelKey", refreshAll: true);
                    // Set refresh interval to reduce the request checking sentinel key to refresh configs
                    refreshOptions.SetRefreshInterval(TimeSpan.FromMinutes(5));
                });

                // If using App Configuration with Azure Key Vault
                options.ConfigureKeyVault(keyVaultOptions =>
                {
                    keyVaultOptions.SetSecretRefreshInterval(TimeSpan.FromMinutes(5));
                    keyVaultOptions.SetCredential(new ChainedTokenCredential(
                        new VisualStudioCredential(),
                        new ManagedIdentityCredential(managedIdentity)
                    ));
                });
            });
        }

        /// <summary>
        /// This method is for normal use cases, but there are requests to app configuration to update the latest value.
        /// Should consider when to use it.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddAppConfiguration(this ConfigurationManager configuration)
        {
            var appConfigEndpoint = configuration.GetValue<string>("appConfigEndpoint");
            var managedIdentity = configuration.GetValue<string>("ClientId");

            return configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(appConfigEndpoint), new ChainedTokenCredential(
                    new VisualStudioCredential(),
                    new ManagedIdentityCredential(managedIdentity)
                ));

                // If using App Configuration with Azure Key Vault
                options.ConfigureKeyVault(keyVaultOptions =>
                {
                    keyVaultOptions.SetSecretRefreshInterval(TimeSpan.FromMinutes(5));
                    keyVaultOptions.SetCredential(new ChainedTokenCredential(
                        new VisualStudioCredential(),
                        new ManagedIdentityCredential(managedIdentity)
                    ));
                });
            });
        }
    }
}
