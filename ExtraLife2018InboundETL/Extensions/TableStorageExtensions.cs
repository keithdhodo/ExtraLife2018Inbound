using ExtraLife2018InboundETL.Interfaces;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraLife2018InboundETL.Extensions
{
    public static class TableStorageExtensions<T>
    {
        public static async Task WriteToTableStorageAsync(CloudTableClient cloudTableClient, IEnumerable<T> items, string tableName)
        {
            var tableBatchOperation = new TableBatchOperation();

            foreach (var item in items.Cast<IHashable>().ToList())
            {
                item.PartitionKey = tableName;
                item.RowKey = item.CreateGuidFromSHA256Hash().ToString();

                var operation = TableOperation.InsertOrReplace(item);
                tableBatchOperation.Add(operation);
            }

            if (tableBatchOperation.Count() > 0)
            { 
                var table = cloudTableClient.GetTableReference(tableName);
                if (! await table.ExistsAsync())
                {
                    await table.CreateIfNotExistsAsync();
                }
                await table.ExecuteBatchAsync(tableBatchOperation);
            }
        }

        public static async Task<IList<T>> GetRecordsFromTableStorageAsync<T>(CloudTable table, TableQuery<T> query) where T : TableEntity, new()
        {
            TableContinuationToken token = null;
            var participantTableEntities = new List<T>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                participantTableEntities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);
            return participantTableEntities;
        }
    }
}
