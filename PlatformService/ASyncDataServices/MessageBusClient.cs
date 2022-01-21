using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;
//using Azure.Messaging.ServiceBus;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly string queueName = "platcomm";
        private IQueueClient queueClient;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            queueClient = new QueueClient(_configuration.GetConnectionString("AzureServiceBus"), queueName);
            
        }

        private async Task SendMessageAsync<T>(T serviceBusMessage, string queueName)
        {
            
            string messageBody = JsonSerializer.Serialize(serviceBusMessage);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await queueClient.SendAsync(message);
            Console.WriteLine($"--> We have sent {message}");
        }
        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            //var message = JsonSerializer.Serialize(platformPublishedDto);
            SendMessageAsync(platformPublishedDto, queueName);
            Console.WriteLine("--> Azure bus Connection Open, sending message...");
           
        }
    }
}