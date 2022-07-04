using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoronaCheckIn.Managers;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CoronaCheckIn.Areas.api
{
    [ApiController]
    [Route("/api/qr")]
    public class ApiQrController : ControllerBase
    {
        private readonly ILogger<ApiQrController> _logger;
        private readonly RoomManager _roomManager;
        private readonly SessionManager _sessionManager;

        public ApiQrController(ILogger<ApiQrController> logger, RoomManager roomManager, SessionManager sessionManager)
        {
            _logger = logger;
            _roomManager = roomManager;
            _sessionManager = sessionManager;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("scan")]
        public ActionResult<string> Scan([FromBody] ScanBody body)
        {
            var room = _roomManager.GetRoom(new Guid(body.RoomId));
            return "TODO: needs implementation";
        }
    }

    public class ScanBody
    {
        public string RoomId { get; set; } = string.Empty;
    }
}