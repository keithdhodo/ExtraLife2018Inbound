using ExtraLife2018InboundETL.Interfaces;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraLife2018InboundETL.Extensions
{
    public static class QueueExtensions<T>
    {
        public static async Task WriteToQueueAsync(IQueueClient queueClient, IEnumerable<T> items, string queueName)
        {
            var messages = new List<Message>();

            foreach (var item in items.Cast<IHashable>().ToList())
            {
                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item)))
                {
                    ContentType = "application/json",
                    Label = queueName,
                    MessageId = item.CreateGuidFromSHA256Hash().ToString(),
                    TimeToLive = TimeSpan.FromMinutes(2)
                };

                messages.Add(message);
            }

            if (messages.Count() > 0)
            {
                await queueClient.SendAsync(messages);
            }
        }
    }
}
