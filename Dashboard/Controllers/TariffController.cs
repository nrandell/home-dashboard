using System;
using System.Threading.Tasks;

using Dashboard.Models;
using Dashboard.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NodaTime;

namespace Dashboard.Controllers
{
    [Route("tariff")]
    [ApiController]
    public class TariffController : ControllerBase
    {
        public ILogger Logger { get; }
        public OctopusTariffService OctopusTariffService { get; }

        public TariffController(ILogger<TariffController> logger, OctopusTariffService octopusTariffService)
        {
            Logger = logger;
            OctopusTariffService = octopusTariffService;
        }


        [HttpGet("{timestamp}")]
        public async Task<ActionResult<OctopusTariffEntry>> GetTariff(Instant timestamp)
        {
            try
            {
                var result = await OctopusTariffService.TryGetTariffFor(timestamp);
                return result ?? (ActionResult<OctopusTariffEntry>)NoContent();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving tariff for {Timestamp}: {Exception}", timestamp, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
