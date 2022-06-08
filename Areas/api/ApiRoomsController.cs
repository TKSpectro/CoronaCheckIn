using System.Text.Json;
using CoronaCheckIn.Managers;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoronaCheckIn.Areas.api;

[ApiController]
[Route("/api/rooms")]
public class ApiRoomsController : ControllerBase
{
    private readonly ILogger<ApiRoomsController> _logger;
    private readonly RoomManager _roomManager;

    public ApiRoomsController(ILogger<ApiRoomsController> logger, RoomManager roomManager)
    {
        _logger = logger;
        _roomManager = roomManager;
    }

    [HttpGet("")]
    public ActionResult<string> Index([FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null, [FromQuery] Faculty? faculty = null)
    {
        var rooms = _roomManager.GetRooms(sortBy: sortBy, sortOrder: sortOrder, faculty: faculty);

        return JsonSerializer.Serialize(rooms);
    }

    [HttpGet("{id}")]
    public ActionResult<string> GetOne(Guid id)
    {
        var room = _roomManager.GetRoom(id);
        return JsonSerializer.Serialize(room);
    }

    [HttpPost("")]
    public ActionResult<string> Create([FromBody] PostRoom postRoom)
    {
        var parsedFaculty = Room.ParseFacultyFromString(postRoom.Faculty);
        if (parsedFaculty == null) throw new Exception("Could not parse faculty");

        var room = new Room
        {
            Name = postRoom.Name,
            MaxDuration = postRoom.MaxDuration,
            MaxParticipants = postRoom.MaxParticipants,
            Faculty = (Faculty) parsedFaculty
        };

        var createdRoom = _roomManager.Add(room);
        return JsonSerializer.Serialize(createdRoom);
    }

    public class PostRoom
    {
        public string Name { get; set; }

        public int MaxParticipants { get; set; } = -1;

        public int MaxDuration { get; set; } = 90;

        public string Faculty { get; set; }
    }
}