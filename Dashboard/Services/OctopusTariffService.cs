using System;
using System.Linq;
using System.Threading.Tasks;

using Dashboard.Models;

using InfluxDB.Client;
using InfluxDB.Client.Core.Exceptions;

using Microsoft.Extensions.Logging;

using NodaTime;

using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

namespace Dashboard.Services
{
    public class OctopusTariffService : IDisposable
    {
        public ILogger Logger { get; }
        public AsyncRetryPolicy RetryPolicy { get; }

        private const string DatabaseName = "HomeMeasurements";
        private readonly InfluxDBClient _client;

        public OctopusTariffService(ILogger<OctopusTariffService> logger)
        {
            Logger = logger;
            var options = InfluxDBClientOptions.Builder.CreateNew()
                .Url("http://server.home:8086")
                .Bucket(DatabaseName + "/one_year")
                .Org("-")
                .Build();

            _client = InfluxDBClientFactory.Create(options);

            var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), retryCount: 100);
            RetryPolicy = Policy.Handle<HttpException>()
                .WaitAndRetryAsync(delay,
                    (ex, ts) => Logger.LogWarning(ex, "Waiting {TimeSpan} due to {Exception}", ts, ex.Message));
        }

        public void Dispose() => _client.Dispose();

        private const long SecondsPerPeriod = 60 * 30;
        private long ConvertSecondsToTimePeriod(long seconds) => SecondsPerPeriod * (seconds / SecondsPerPeriod);

        public async Task<OctopusTariffEntry?> TryGetTariffFor(Instant timestamp)
        {
            var timePeriod = Instant.FromUnixTimeSeconds(ConvertSecondsToTimePeriod(timestamp.ToUnixTimeSeconds()));
            var next = timePeriod.PlusTicks(TimeSpan.TicksPerSecond);
            var flux = @$"
from(bucket: ""HomeMeasurements/one_year"")
|> range(start: {timePeriod}, stop: {next})
|> filter(fn: (r) => (r._measurement == ""Tariff"") and (r._field == ""ValueExcVat""))
";
            var results = await _client.GetQueryApi().QueryAsync<OctopusTariffEntry>(flux);
            return results.SingleOrDefault();
        }
    }
}
