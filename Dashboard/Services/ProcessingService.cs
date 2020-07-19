using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Dashboard.Hubs;
using Dashboard.Models;

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
        public IHubContext<DataHub, IDataHub> HubContext { get; }
        public MqttReceiverService MqttReceiverService { get; }
        public HildebrandStateStore Store { get; }

        public ProcessingService(ILogger<ProcessingService> logger, ChannelReader<MqttApplicationMessage> channelReader, IHubContext<DataHub, IDataHub> hubContext, MqttReceiverService mqttReceiverService, HildebrandStateStore store)
        {
            Logger = logger;
            ChannelReader = channelReader;
            HubContext = hubContext;
            MqttReceiverService = mqttReceiverService;
            Store = store;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await MqttReceiverService.StartAsync(stoppingToken);
            await foreach (var message in ChannelReader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await HandleMessageAsync(message);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Error handling payload: {Exception}", ex.Message);
                }
            }
        }

        private Task HandleMessageAsync(MqttApplicationMessage message)
        {
            return message.Topic switch
            {
                "nick/sensor/hildebrand/state" => HandleHildebrandState(message),
                _ => Task.CompletedTask,
            };
        }

        private Task HandleHildebrandState(MqttApplicationMessage message)
        {
            var state = new HildebrandState(message);
            Store.Store(state);
            return HubContext.Clients.All.Data(state.Topic, state.Json);
        }
    }
}
