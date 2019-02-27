using ExtraLife2018InboundETL.Entities;
using ExtraLife2018InboundETL.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace ExtraLife2018InboundETL.Functions
{
    public static class GetWinners
    {
        private static IConfigurationRoot config;
        private static IKeyVaultClient keyVaultClient;

        [FunctionName("GetWinners")]
        [ProducesResponseType(200)]
        public static async Task<ImmutableList<Prize>> Run([HttpTrigger(AuthorizationLevel.Function, "get")]
            HttpRequest req, 
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("GetWinners HTTP trigger function processed a request.");

            config = Configuration.InitializeConfiguration(config, context);
            KeyVaultExtensions.InitializeConfiguration(config);
            keyVaultClient = KeyVaultExtensions.GetInstance();
            var sqlConnectionString = await KeyVaultExtensions.GetSecretAsync(Constants.KeyVaultConstants.SqlConnectionString);
            var prizes = await SqlExtensions<Prize>.GetWinnersFromSqlAsync(sqlConnectionString);
            var builder = ImmutableList.CreateBuilder<Prize>();
            builder.AddRange(prizes);
            
            return builder.ToImmutableList();
        }
    }
}
