using MQTTnet;

namespace Dashboard.Models
{
    public class HildebrandState
    {
        public string Topic { get; set; } = default!;
        public string Json { get; set; } = default!;

        public HildebrandState() { }

        public HildebrandState(MqttApplicationMessage message)
        {
            Topic = message.Topic;
            Json = message.ConvertPayloadToString();
        }
    }
}
