using ExtraLife2018InboundETL.Entities;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExtraLife2018InboundETL.Extensions
{
    public static class KeyVaultExtensions
    {
        public static KeyVaultClient KeyVaultClient { get; private set; }
        public static ConcurrentDictionary<string, Secret> Secrets;

        private static HttpClient httpClient;
        private static IConfigurationRoot Configuration;

        public static IKeyVaultClient GetInstance()
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient();
            }

            if (KeyVaultClient == null)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                KeyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback), httpClient);
            }

            if (Secrets == null)
            {
                Secrets = new ConcurrentDictionary<string, Secret>();
            }

            return KeyVaultClient;
        }

        public static void InitializeConfiguration(IConfigurationRoot config)
        {
            if (Configuration == null)
            {
                Configuration = config;
            }

            return;
        }

        public static async Task<string> GetSecretAsync(string secretName)
        {
            var secret = Secrets.ContainsKey(secretName) ? Secrets[secretName] : new Secret();

            if (secret == null || secret.Expiration < DateTime.Now.AddDays(-1))
            {
                secret = new Secret()
                {
                    Value = (await KeyVaultClient.GetSecretAsync(Constants.KeyVaultConstants.KeyVaultBaseUrl, secretName)).Value,
                    Expiration = DateTime.Now.AddDays(1)
                };

                Secrets.AddOrUpdate(
                    secretName,
                    secret,
                    (key, oldValue) => secret
                );
            }

            return secret.Value;
        }
    }
}
