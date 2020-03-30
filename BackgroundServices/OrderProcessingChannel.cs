using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AspNetCoreChannelsAndBackgroundServices.BackgroundServices
{
    public class OrderProcessingChannel
    {
        private const int MaxMessagesInChannel = 100;

        private readonly Channel<string> _channel;
        private readonly ILogger<OrderProcessingChannel> _logger;

        public OrderProcessingChannel(ILogger<OrderProcessingChannel> logger)
        {
            _logger = logger;
            
            var options = new BoundedChannelOptions(MaxMessagesInChannel)
            {
                SingleWriter = true,
                SingleReader = true
            };

            _channel = Channel.CreateBounded<string>(options);
        }

        public async Task<bool> AddOrderAsync(string orderId, CancellationToken ct = default)
        {
            while (await _channel.Writer.WaitToWriteAsync(ct) && !ct.IsCancellationRequested)
            {
                if (_channel.Writer.TryWrite(orderId))
                {
                    return true;
                }
            }
            return false;
        }

        public IAsyncEnumerable<string> ReadAllAsync(CancellationToken ct = default) =>
            _channel.Reader.ReadAllAsync(ct);
        
    }
}