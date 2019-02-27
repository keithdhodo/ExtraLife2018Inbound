using EL.DonorDrive;
using EL.DonorDrive.Entities;
using ExtraLife2018InboundETL.Entities;
using ExtraLife2018InboundETL.Extensions;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExtraLife2018InboundETL
{
    public static class GetDonationsForParticipants
    {
        private static HttpClient client = new HttpClient();
        private static CloudTableClient cloudTableClient;
        private static IConfigurationRoot config;
        private static IKeyVaultClient keyVaultClient;
        private static IQueueClient queueClient;
        private static IDictionary<Guid, ParticipantDto> participants;
        private static ILogger logging;

        static GetDonationsForParticipants()
        {
            MapInitializer.Activate();

            if (participants == null)
            {
                participants = new ConcurrentDictionary<Guid, ParticipantDto>();
            }
        }

        [FunctionName("GetDonationsForParticipants")]
        public static async Task RunAsync(
            [ServiceBusTrigger("participants", Connection = "AzureWebJobsServiceBus")]
            ParticipantTableEntity myQueueItem,
            ILogger log,
            ExecutionContext context)
        {
            config = Configuration.InitializeConfiguration(config, context);
            KeyVaultExtensions.InitializeConfiguration(config);
            keyVaultClient = KeyVaultExtensions.GetInstance();
            logging = log;

            var tableStorageAccountConnectionString = await KeyVaultExtensions.GetSecretAsync(Constants.KeyVaultConstants.AzureWebjobsStorageKey);

            cloudTableClient = CloudTableClientExtensions.InitializeCloudTableClientAsync(cloudTableClient, tableStorageAccountConnectionString);

            logging.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem.DisplayName}");

            var table = cloudTableClient.GetTableReference("CascadiaGamersParticipants");
            var query = new TableQuery<ParticipantTableEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, myQueueItem.RowKey)); ;

            Guid.TryParse(myQueueItem.RowKey, out var participantId);
            if (!participants.ContainsKey(participantId))
            {
                await AddParticipantToCacheAsync(table, query);
            }

            table = cloudTableClient.GetTableReference("CascadiaGamersDonations");
            var donationQuery = new TableQuery<DonationTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("ParticipantId", QueryComparisons.Equal, myQueueItem.ParticipantId.ToString()));
            await AddDonationToCacheAsync(table, donationQuery, participants[participantId]);

            logging.LogInformation($"Getting donations for {myQueueItem.DisplayName} at: {DateTime.Now}");
            var donorDriveClient = new DonorDriveClient();
            var donations = await donorDriveClient.GetDonorParticipantsAsync(uint.Parse(myQueueItem.ParticipantId));
            logging.LogInformation($"Retrieved {donations.Count()} donations for {myQueueItem.DisplayName} at: {DateTime.Now}");

            var participantDto = participants[participantId];
            var newDonations = new List<Donor>();

            foreach (var donation in donations)
            {
                var donationid = participantDto.CreateGuidFromSHA256Hash(donation);
                if (!participantDto.Donations.ContainsKey(donationid))
                {
                    participantDto.Donations.Add(donationid, donation);
                    newDonations.Add(donation);
                }
            }

            var donationsForTableStorage = AutoMapper.Mapper.Map<List<DonationTableEntity>>(donations);

            donationsForTableStorage.Select(
                d => 
                { d.ParticipantUniqueIdentifier = participantDto.CreateGuidFromSHA256Hash().ToString(); return d; }
                ).ToList();

            logging.LogInformation($"Updating or Inserting {donationsForTableStorage.Count} donations for {myQueueItem.DisplayName} to Table Storage at: {DateTime.Now}");

            await TableStorageExtensions<DonationTableEntity>.WriteToTableStorageAsync(cloudTableClient, donationsForTableStorage,
                "CascadiaGamersDonations");

            logging.LogInformation($"Updating or Inserting {donationsForTableStorage.Count} donations for {myQueueItem.DisplayName} to Sql Server at: {DateTime.Now}");
            var sqlConnectionString = await KeyVaultExtensions.GetSecretAsync(Constants.KeyVaultConstants.SqlConnectionString);
            await SqlExtensions<DonationTableEntity>.WriteDonationsToSqlAsync(donationsForTableStorage, sqlConnectionString);
            logging.LogInformation($"Completed upserting {donationsForTableStorage.Count} donations for {myQueueItem.DisplayName} to Sql Server at: {DateTime.Now}");

            if (newDonations.Count() > 0)
            {
                var donationsForNewDonationsQueue = AutoMapper.Mapper.Map<List<DonationTableEntity>>(newDonations);

                var queueConnectionString = await KeyVaultExtensions.GetSecretAsync(Constants.KeyVaultConstants.ServiceBusConnectionString);
                var queueName = "newdonations";
                queueClient = QueueClientExtensions.InitializeQueueClient(queueClient, queueConnectionString, queueName);
                logging.LogInformation($"Adding {donationsForNewDonationsQueue.Count} new donations for {myQueueItem.DisplayName} to Service Bus at: {DateTime.Now}");
                await QueueExtensions<DonationTableEntity>.WriteToQueueAsync(queueClient, donationsForNewDonationsQueue, queueName);
            }
        }

        private static async Task AddParticipantToCacheAsync(CloudTable table, TableQuery<ParticipantTableEntity> query)
        {
            var participantTableEntities = await TableStorageExtensions<ParticipantTableEntity>
                .GetRecordsFromTableStorageAsync(table, query);

            var participantCount = participantTableEntities.Count();

            for (int i = 0; i < participantCount; i++)
            {
                var currentParticipant = participantTableEntities[i];

                logging.LogInformation($"Added {currentParticipant.DisplayName} to the participant cache");

                Guid.TryParse(currentParticipant.RowKey, out var participantId);

                if (!participants.ContainsKey(participantId))
                {
                    var participantDto = AutoMapper.Mapper.Map<ParticipantDto>(currentParticipant);
                    participants.Add(participantId, participantDto);
                }
            }
        }

        private static async Task AddDonationToCacheAsync(CloudTable table, 
            TableQuery<DonationTableEntity> query, ParticipantDto participant)
        {
            var results = await TableStorageExtensions<DonationTableEntity>
                .GetRecordsFromTableStorageAsync(table, query);

            var donations = AutoMapper.Mapper.Map<List<Donor>>(results);
            var donationCount = results.Count();

            for (int i = 0; i < donationCount; i++)
            {
                var currentDonation = donations[i];

                logging.LogInformation($"Added {currentDonation.DisplayName} to the donation cache for {participant.DisplayName}");
                var donationid = participant.CreateGuidFromSHA256Hash(currentDonation);
                if (!participant.Donations.ContainsKey(donationid))
                {
                    participant.Donations.Add(donationid, currentDonation);
                }
            }
        }
    }
}
