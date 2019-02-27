using EL.DonorDrive;
using ExtraLife2018InboundETL.Entities;
using ExtraLife2018InboundETL.Extensions;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraLife2018InboundETL
{
    public static class GetTeamParticipants
    {
        // I'm using a static HttpClient and ServiceBus so that 
        // multiple function executions within a single function app 
        // instance don't have to initialize new resources every execution.
        private static CloudTableClient cloudTableClient;
        private static IConfigurationRoot config;
        private static IKeyVaultClient keyVaultClient;
        private static IQueueClient queueClient;

        static GetTeamParticipants()
        {
            MapInitializer.Activate();
        }

        [FunctionName("GetTeamParticipants")]
        // https://codehollow.com/2017/02/azure-functions-time-trigger-cron-cheat-sheet/
        public static async Task RunAsync([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            config = Configuration.InitializeConfiguration(config, context);
            KeyVaultExtensions.InitializeConfiguration(config);
            keyVaultClient = KeyVaultExtensions.GetInstance();

            var tableStorageAccountConnectionString = await KeyVaultExtensions.GetSecretAsync(Constants.KeyVaultConstants.AzureWebjobsStorageKey);

            cloudTableClient = CloudTableClientExtensions.InitializeCloudTableClientAsync(cloudTableClient, tableStorageAccountConnectionString);

            var queueConnectionString = await KeyVaultExtensions.GetSecretAsync(Constants.KeyVaultConstants.ServiceBusConnectionString);
            queueClient = QueueClientExtensions.InitializeQueueClient(queueClient, queueConnectionString, "participants");

            uint.TryParse(config["CascadiaGamersGroupId"], out var extraLifeGroupId);

            log.LogInformation($"Getting gamers: {DateTime.Now}");
            var donorDriveClient = new DonorDriveClient();
            var gamers = new List<ParticipantDto>(); //AutoMapper.Mapper.Map<List<ParticipantDto>>(await donorDriveClient.GetTeamParticipantsAsync(extraLifeGroupId));
            log.LogInformation($"Retrieved: {gamers.Count()} gamers.");

            var participants = AutoMapper.Mapper.Map<List<ParticipantTableEntity>>(gamers);

            await TableStorageExtensions<ParticipantTableEntity>.WriteToTableStorageAsync(cloudTableClient, participants, 
                "CascadiaGamersParticipants");

            log.LogInformation($"Wrote to Table Storage values for: {gamers.Count()} participants.");

            await QueueExtensions<ParticipantTableEntity>.WriteToQueueAsync(queueClient, participants, 
                "parcicipants");

            log.LogInformation($"Wrote {gamers.Count()} participants to Service Bus to check for new donations.");

            log.LogInformation($"Writing to Sql Server values for: {gamers.Count()} participants.");

            var sqlConnectionString = await KeyVaultExtensions.GetSecretAsync(Constants.KeyVaultConstants.SqlConnectionString);
            await SqlExtensions<ParticipantTableEntity>.WriteParticipantsToSqlAsync(participants, sqlConnectionString);

            log.LogInformation($"Wrote to Sql Server values for: {gamers.Count()} participants.");
        }
    }
}
