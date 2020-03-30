using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreChannelsAndBackgroundServices.BackgroundServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNetCoreChannelsAndBackgroundServices.Models;

namespace AspNetCoreChannelsAndBackgroundServices.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly OrderProcessingChannel _orderProcessingChannel;

        public HomeController(ILogger<HomeController> logger,
            OrderProcessingChannel orderProcessingChannel)
        {
            _logger = logger;
            _orderProcessingChannel = orderProcessingChannel;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            // var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            // cts.CancelAfter(TimeSpan.FromSeconds(3));
                        
            try
            {
                var isSuccess = await _orderProcessingChannel.AddOrderAsync(Guid.NewGuid().ToString(), cancellationToken/*cts.Token*/);

                if (isSuccess)
                {
                    _logger.LogInformation("Order was successfully added to the channel");
                }
            }
            catch
            {
                _logger.LogError("shit happened");
            }
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}