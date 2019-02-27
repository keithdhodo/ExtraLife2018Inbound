using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ExtraLife2018InboundETL.Extensions
{
    public static class CloudTableClientExtensions
    {
        public static CloudTableClient InitializeCloudTableClientAsync(CloudTableClient cloudTableClient,
            string tableStorageAccountConnectionString)
        {
            if (cloudTableClient == null)
            {
                var storageAccount = CloudStorageAccount.Parse(tableStorageAccountConnectionString);
                cloudTableClient = storageAccount.CreateCloudTableClient();
            }

            return cloudTableClient;
        }
    }
}
