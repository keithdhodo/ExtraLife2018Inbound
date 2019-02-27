using Microsoft.Azure.ServiceBus;

namespace ExtraLife2018InboundETL.Extensions
{
    public static class QueueClientExtensions
    {
        public static IQueueClient InitializeQueueClient(IQueueClient queueClient, string queueConnectionString, string queueName)
        {
            if (queueClient == null)
            {
                queueClient = new QueueClient(queueConnectionString, queueName);
            }

            return queueClient;
        }
    }
}
