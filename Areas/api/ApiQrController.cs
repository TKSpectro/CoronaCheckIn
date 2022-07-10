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
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
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
        private readonly IConfiguration _config;

        public ApiQrController(ILogger<ApiQrController> logger, RoomManager roomManager, SessionManager sessionManager, IConfiguration config)
        {
            _logger = logger;
            _roomManager = roomManager;
            _sessionManager = sessionManager;
            _config = config;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("scan")]
        public ActionResult<ScanResponse> Scan([FromBody] ScanBody body)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var secret = _config.GetValue<string>("Jwt:Secret");
            var key = Encoding.UTF8.GetBytes(secret);
            var issuer = _config.GetValue<string>("Jwt:Issuer");
            if (issuer.Trim().Length == 0) issuer = "ccn";
            var audience = _config.GetValue<string>("Jwt:Audience");;
            if (audience.Trim().Length == 0) issuer = "ccn";
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = (jwtToken.Claims.First(x => x.Type == "UserId").Value);
            
            var runningSessions = _sessionManager.GetSessions(userId: userId, roomId: body.RoomId,
                endTimeNull: true, includeRoom: true, sortBy: "start", sortOrder: "desc").ToList();

            var currentRunningSession = false;
            foreach (var session in runningSessions)
            {
                // TODO: Need to check this algorithm and proof with more test cases
                var maxEndTime = session.StartTime.AddMinutes(session.Room.MaxDuration);
                if (maxEndTime <= DateTime.Now)
                {
                    // Session is an old session which the user did not close
                    session.EndTime = maxEndTime;
                    _sessionManager.UpdateSession(session);
                }
                else
                {
                    if (body.RoomId == session.RoomId)
                    {
                        // TODO: Check timestamp in qrcode against the one in the database
                        //body.Date == session.Room.QrCodeTimestamp;
                    }
                    // Session is the current session and the user scanned to leave
                    session.EndTime = DateTime.Now;
                    _sessionManager.UpdateSession(session);
                    currentRunningSession = true;
                }
            }

            if (currentRunningSession == false)
            {
                var newSession = new Session()
                {
                    Infected = false,
                    RoomId = body.RoomId,
                    UserId = userId,
                    StartTime = DateTime.Now,
                };
                _sessionManager.AddSession(newSession);
                
                return new ScanResponse{Message = "Started session"};
            }
            
            return new ScanResponse{Message = "Stopped session"};
        }
    }

    public class ScanBody
    {
        public Guid RoomId { get; set; }
        
        public string Date { get; set; } = string.Empty;
    }
    
    public class ScanResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}