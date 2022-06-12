using System.Diagnostics;
using CoronaCheckIn.Managers;
using Microsoft.AspNetCore.Mvc;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authorization;

namespace CoronaCheckIn.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ILogger<RoomsController> _logger;
        private readonly RoomManager _roomManager;

        public RoomsController(ILogger<RoomsController> logger, RoomManager roomManager)
        {
            _logger = logger;
            _roomManager = roomManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index([FromQuery] string? name = null, [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null, [FromQuery] Faculty? faculty = null)
        {
            IEnumerable<Room> rooms =
                _roomManager.GetRooms(name: name, sortBy: sortBy, sortOrder: sortOrder, faculty: faculty);

            ViewBag.room = rooms.ToArray()[0];
            return View(rooms);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult RemoveRoom(Guid id)
        {
            _roomManager.RemoveRoom(id);
            return RedirectToAction("Index");
        }

        // public IActionResult AddRoom(Room room)
        // {
        //     _roomManager.AddRoom(room);
        //     return RedirectToAction("Index");
        // }

        public IActionResult CreateRoom(Room? room=null)
        {
            return PartialView(new Room());
        }

        [HttpPost]
        [Route("CreateRoom")]
        public IActionResult AddRoom(Room room)
        {
            room.QrCode = "code";
            if (ModelState.IsValid)
            {
                _roomManager.AddRoom(room);
                return RedirectToAction("Index");
            }

            return View("CreateRoom", room);
        }
        
        
        [HttpPost]
        public IActionResult UpdateRoom(Room room)
        {
            // Room? room = _roomManager.GetRoom(newRoom.Id);

            Room? newRoom = _roomManager.UpdateRoom(room);
            Console.WriteLine(newRoom);
            return RedirectToAction("Index");
        }
    }
}