using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.AspNetCore.Mvc;


namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private string _queueName = "platcomm";
        static IQueueClient queueClient;
        private readonly IMapper _mapper;
        private MessageHandlerOptions messageHandlerOptions;

        public MessageBusSubscriber(
            IConfiguration configuration, 
            IServiceScopeFactory scopeFactory,
            IMapper mapper)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _mapper = mapper;
            Console.WriteLine("Initializing the message bus");
            InitializeBus(_configuration, _queueName);
        }
        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine($"Message handler exception: { arg.Exception }");
            return Task.CompletedTask;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            Console.WriteLine("Execute async");
             //queueClient.RegisterMessageHandler(_eventProcessor.ProcessMessagesAsync, messageHandlerOptions);
             //await InitializeBus(_configuration, _queueName);
             queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.ReadLine();

            await queueClient.CloseAsync();
        }

        
        void InitializeBus( IConfiguration _configuration, string _queueName) {
            
            queueClient = new QueueClient(_configuration.GetConnectionString("AzureServiceBus"), _queueName);

            messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            //_eventProcessor.InitializeClient(ref queueClient);
            //queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            //Console.ReadLine();

            //await queueClient.CloseAsync();
        }

        private void addPlatform(Message message)
        {
            Console.WriteLine("Adding platform");
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var jsonString = Encoding.UTF8.GetString(message.Body);
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(jsonString);

                try
                {
                    var plat = _mapper.Map<Platformm>(platformPublishedDto);
                    if(!repo.ExternalPlatformExists(plat.ExternalID))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                        Console.WriteLine("--> Platform added!");
                    }
                    else
                    {
                        Console.WriteLine("--> Platform already exisits...");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
                }
                //await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            }
        }
        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            addPlatform(message);

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }
        

    }
}