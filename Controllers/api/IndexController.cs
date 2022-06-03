using System.Collections.Immutable;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using CoronaCheckIn.Managers;

namespace CoronaCheckIn.Controllers.api
{
    [ApiController]
    [Route("/api")]
    public class IndexController : ControllerBase
    {
        private readonly ILogger<IndexController> _logger;

        public IndexController(ILogger<IndexController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public ActionResult<String> Index()
        {
            return "This is the rest api of CoronaCheckIn";
        }
    }
}