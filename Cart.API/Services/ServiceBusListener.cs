using Azure.Messaging.ServiceBus;
using Cart.BLL.Interfaces;
using Cart.BLL;
using Cart.Core.DTOs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cart.API.Services
{
    public class ServiceBusListener : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ServiceBusListener> _logger;

        public ServiceBusListener(ServiceBusClient serviceBusClient, IConfiguration configuration, IServiceProvider serviceProvider, ILogger<ServiceBusListener> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            var topicName = configuration["ServiceBus:TopicName"];
            var subscriptionName = configuration["ServiceBus:SubscriptionName"];

            _processor = serviceBusClient.CreateProcessor(topicName, subscriptionName);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;
            return _processor.StartProcessingAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;
            return _processor.StartProcessingAsync(stoppingToken);
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var messageBody = args.Message.Body.ToString();
                var productUpdate = JsonSerializer.Deserialize<ProductUpdatedIntegrationEvent>(messageBody);

                if (productUpdate != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();
                        await cartService.UpdateProductInCartsAsync(productUpdate);
                    }
                }

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message.");
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Error in the Service Bus processor.");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }
    }
}