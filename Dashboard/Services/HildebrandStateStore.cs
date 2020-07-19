using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Dashboard.Models;

using Microsoft.Extensions.Logging;

namespace Dashboard.Services
{
    public class HildebrandStateStore
    {
        public const int Capacity = 1000;

        public ILogger Logger { get; }
        private ImmutableList<HildebrandState> _store = ImmutableList<HildebrandState>.Empty;

        public HildebrandStateStore(ILogger<HildebrandStateStore> logger)
        {
            Logger = logger;
        }

        public void Store(HildebrandState message)
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

        public IEnumerable<HildebrandState> Messages => _store;

        public HildebrandState? Latest => _store.Count > 0 ? _store.Last() : default;
    }
}
