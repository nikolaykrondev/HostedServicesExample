using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCoreChannelsAndBackgroundServices.BackgroundServices
{
    public class FileCreatorService : BackgroundService
    {
        private readonly ILogger<FileCreatorService> _logger;
        private readonly OrderProcessingChannel _orderProcessingChannel;
        
        private readonly int _refreshIntervalInSeconds;

        public FileCreatorService(ILogger<FileCreatorService> logger,
            OrderProcessingChannel orderProcessingChannel)
        {
            _logger = logger;
            _orderProcessingChannel = orderProcessingChannel;
            _refreshIntervalInSeconds = 30;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await foreach (var order in _orderProcessingChannel.ReadAllAsync(stoppingToken))
                {
                    await File.WriteAllTextAsync(Environment.CurrentDirectory + "/check_me.txt", order, stoppingToken);
                    _logger.LogInformation($"File {Environment.CurrentDirectory}/check_me.txt was created");
                }

                await Task.Delay(TimeSpan.FromSeconds(_refreshIntervalInSeconds), stoppingToken);
            }
        }
    }
}