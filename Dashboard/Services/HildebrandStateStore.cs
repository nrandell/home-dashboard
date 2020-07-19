using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.Extensions.Logging;

using MQTTnet;

namespace Dashboard.Services
{
    public class HildebrandStateStore
    {
        public const int Capacity = 1000;

        public ILogger Logger { get; }
        private ImmutableList<MqttApplicationMessage> _store = ImmutableList<MqttApplicationMessage>.Empty;

        public HildebrandStateStore(ILogger<HildebrandStateStore> logger)
        {
            Logger = logger;
        }

        public void Store(MqttApplicationMessage message)
        {
            var store = _store;
            if (store.Count == Capacity)
            {
                _store = store.RemoveAt(0).Add(message);
            }
            else
            {
                _store = store.Add(message);
            }
        }

        public IEnumerable<MqttApplicationMessage> Messages => _store;

        public MqttApplicationMessage? Latest => _store.Count > 0 ? _store.Last() : default;
    }
}
