using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using InfluxDB.Client.Core;

using NodaTime;

namespace Dashboard.Models
{
    public class OctopusTariffEntry
    {
        [Column("_value")]
        public double Price { get; set; }

        [Column("_time", IsTimestamp = true)]
        public Instant Time { get; set; }
    }
}
