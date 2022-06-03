using System.Collections.Immutable;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using CoronaCheckIn.Managers;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace CoronaCheckIn.Controllers.api
{
    [ApiController]
    [Route("/api/rooms")]
    public class RoomsController : ControllerBase
    {
        private readonly ILogger<RoomsController> _logger;
        private readonly RoomManager _roomManager;

        public RoomsController(ILogger<RoomsController> logger, RoomManager roomManager)
        {
            _logger = logger;
            _roomManager = roomManager;
        }

        [HttpGet("")]
        public ActionResult<String> Index([FromQuery] string? faculty)
        {
            var parsedFaculty = Room.ParseFacultyFromString(faculty);
            if (faculty != null && parsedFaculty == null)
            {
                return JsonSerializer.Serialize(Array.Empty<Room>());
            }

            var rooms = _roomManager.GetRooms(parsedFaculty);

            return JsonSerializer.Serialize(rooms);
        }

        [HttpGet("{id}")]
        public ActionResult<String> GetOne(Guid id)
        {
            var room = _roomManager.GetRoom(id);
            return JsonSerializer.Serialize(room);
        }

        public class PostRoom
        {
            public string Name { get; set; }

            public int MaxParticipants { get; set; } = -1;

            public int MaxDuration { get; set; } = 90;

            public string Faculty { get; set; }
        }

        [HttpPost("")]
        public ActionResult<String> Create([FromBody] PostRoom postRoom)
        {
            var parsedFaculty = Room.ParseFacultyFromString(postRoom.Faculty);
            if (parsedFaculty == null)
            {
                throw new Exception("Could not parse faculty");
            }

            Room room = new Room()
            {
                Name = postRoom.Name,
                MaxDuration = postRoom.MaxDuration,
                MaxParticipants = postRoom.MaxParticipants,
                Faculty = (Faculties) parsedFaculty
            };

            var createdRoom = _roomManager.Add(room);
            return JsonSerializer.Serialize(createdRoom);
        }
    }
}