using System.Text.Json;
using CoronaCheckIn.Managers;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("")]
    public ActionResult<IEnumerable<Room>> GetAll([FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null, [FromQuery] Faculty? faculty = null)
    {
        var rooms = _roomManager.GetRooms(sortBy: sortBy, sortOrder: sortOrder, faculty: faculty);

        return rooms.ToList();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("{id}")]
    public ActionResult<Room> GetOne(Guid id)
    {
        var room = _roomManager.GetRoom(id);
        if(room == null)
        {
            throw new Exception("No session found with this id");
        }
        
        return room;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("")]
    public ActionResult<Room> Create([FromBody] PostRoom postRoom)
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

        var createdRoom = _roomManager.AddRoom(room);
        if(createdRoom == null)
        {
            throw new Exception("Room could not be created");
        }
        
        return createdRoom;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostRoom
    {
        public string Name { get; set; }

        public int MaxParticipants { get; set; } = -1;

        public int MaxDuration { get; set; } = 90;

        public string Faculty { get; set; }
    }
}