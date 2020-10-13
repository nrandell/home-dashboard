using System.Collections.Generic;

using Dashboard.Models;
using Dashboard.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dashboard.Controllers
{
    [Route("/history")]
    [ApiController]
    public class HistoricReadingController : ControllerBase
    {
        public ILogger Logger { get; }
        public HildebrandStateStore HildebrandStateStore { get; }

        public HistoricReadingController(ILogger<HistoricReadingController> logger, HildebrandStateStore hildebrandStateStore)
        {
            Logger = logger;
            HildebrandStateStore = hildebrandStateStore;
        }

        [HttpGet]
        public IEnumerable<HildebrandState> GetHistory() => HildebrandStateStore.Messages;
    }
}
