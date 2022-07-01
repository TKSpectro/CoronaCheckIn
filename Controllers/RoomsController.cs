using System.Diagnostics;
using CoronaCheckIn.Managers;
using Microsoft.AspNetCore.Mvc;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authorization;
using QRCoder;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using static QRCoder.PayloadGenerator;


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
            ViewBag.newRoom = new Room();
            return View(rooms);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Details(Guid id)
        {
            byte[] qrCodeImage = createQrCode(id);

            ViewBag.QrCode = qrCodeImage;
            return View(_roomManager.GetRoom(id));
        }

        public IActionResult RemoveRoom(Guid id)
        {
            _roomManager.RemoveRoom(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CreateRoom(string? roomId)
        {
            if (roomId == null)
            {
                return PartialView(new Room());
            }

            var room = _roomManager.GetRoom(new Guid(roomId));
            ViewBag.getRoom = room;
            return Json(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRoom(Room room)
        {
            if (ModelState.IsValid)
            {
                 var checkRoom = _roomManager.GetRoom(room.Id);
                
                if (checkRoom != null)
                {
                    Console.WriteLine("update");
                    var newRoom = _roomManager.UpdateRoom(room);
                    Console.WriteLine(newRoom);
                }
                else
                {
                    Console.WriteLine("add");
                    _roomManager.AddRoom(room);
                }
            }

            return Json(room);
        }

        public byte[] GenerateQrCode(Guid id)
        {
            Room room = _roomManager.GetRoom(id);
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            room.QrCodeTimestamp = dateTimeOffset.ToUnixTimeSeconds();
            _roomManager.UpdateRoom(room);

            return createQrCode(id);
        }

        private byte[] createQrCode(Guid id)
        {
            Room room = _roomManager.GetRoom(id);
            long? timestamp = room.QrCodeTimestamp;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            string payload = "{'id': " + 1 + ", 'timestamp':" + timestamp + "}";
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            return qrCode.GetGraphic(10);
        }
    }
}