using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MQTTnet;
using MQTTnet.Client.Subscribing;

using Nick.Mqtt;

namespace Dashboard.Services
{
    public class MqttReceiverService : ReliableMqttClient<MqttConfiguration>
    {
        public JsonSerializerOptions JsonSerializerOptions { get; }
        public ChannelWriter<MqttApplicationMessage> ChannelWriter { get; }

        public MqttReceiverService(ILogger<MqttReceiverService> logger, IMqttFactory mqttFactory, IOptions<MqttConfiguration> configurationOptions, IHostApplicationLifetime hostApplicationLifetime, ChannelWriter<MqttApplicationMessage> channelWriter) :
            base(logger, mqttFactory, configurationOptions.Value, hostApplicationLifetime)
        {
            ChannelWriter = channelWriter;
            SubscriptionOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(
                new MqttTopicFilterBuilder()
                .WithTopic("#")
                .Build()
                ).Build();
        }

        protected override Task HandleMessage(MqttApplicationMessageReceivedEventArgs ev)
        {
            if (!ChannelWriter.TryWrite(ev.ApplicationMessage))
            {
                ev.ProcessingFailed = true;
            }
            return Task.CompletedTask;
        }
    }
}
