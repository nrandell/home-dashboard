using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Dashboard.Hubs;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MQTTnet;

namespace Dashboard.Services
{
    public class ProcessingService : BackgroundService
    {
        public ILogger Logger { get; }
        public ChannelReader<MqttApplicationMessage> ChannelReader { get; }
        public IHubContext<DataHub> HubContext { get; }
        public MqttReceiverService MqttReceiverService { get; }

        public ProcessingService(ILogger<ProcessingService> logger, ChannelReader<MqttApplicationMessage> channelReader, IHubContext<DataHub> hubContext, MqttReceiverService mqttReceiverService)
        {
            Logger = logger;
            ChannelReader = channelReader;
            HubContext = hubContext;
            MqttReceiverService = mqttReceiverService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await MqttReceiverService.StartAsync(stoppingToken);
            await foreach (var message in ChannelReader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await HandleMessageAsync(message, stoppingToken);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Error handling payload: {Exception}", ex.Message);
                }
            }
        }

        private Task HandleMessageAsync(MqttApplicationMessage message, CancellationToken stoppingToken)
        {
            return message.Topic switch
            {
                "nick/sensor/hildebrand/state" => HandleHildebrandState(message, stoppingToken),
                _ => Task.CompletedTask,
            };
        }

        private Task HandleHildebrandState(MqttApplicationMessage message, CancellationToken stoppingToken)
        {
            var json = message.ConvertPayloadToString();
            return HubContext.Clients.All.SendAsync("Data", message.Topic, json, stoppingToken);
        }
    }
}
