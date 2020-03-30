using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;

namespace AspNetCoreChannelsAndBackgroundServices.BackgroundServices
{
    public class FileCheckerService : IInvocable
    {
        private readonly ILogger<FileCheckerService> _logger;

        public FileCheckerService(ILogger<FileCheckerService> logger)
        {
            _logger = logger;
        }
        public Task Invoke()
        {
            var filePath = Environment.CurrentDirectory + "/check_me.txt";
            _logger.LogInformation($"Checking if {filePath} exists at {DateTime.Now}");
            if (System.IO.File.Exists(filePath))
            {
                _logger.LogInformation("Yes it exists, removing...");
                System.IO.File.Delete(filePath);
                _logger.LogInformation("File was successfully removed");
            }
            else
            {
                _logger.LogInformation("No, there is no file!!! Hibernating...");
            }

            return Task.CompletedTask;
        }
    }
}