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
        public IActionResult Index([FromQuery] string? name = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null, [FromQuery] Faculty? faculty = null)
        {
            IEnumerable<Room> rooms = _roomManager.GetRooms(name: name, sortBy: sortBy, sortOrder: sortOrder, faculty: faculty);

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
            byte[] qrCodeImage = createQrCode();

            ViewBag.QrCode = qrCodeImage;
            return View(_roomManager.GetRoom(id));
        }

        public IActionResult Remove(Guid id)
        {
            _roomManager.RemoveRoom(id);
            return RedirectToAction("Index");
        }

        public byte[] GenerateQrCode()
        {
            return createQrCode();
        }

        public async Task<ActionResult> GetQRCode()
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                string payload = "{'id': " + 1 + ", 'timestamp':" + DateTime.Now + "}";
                
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(20);

                var image = SixLabors.ImageSharp.Image.Load<Rgba32>(qrCodeAsBitmapByteArr);
                image.Mutate(x => x.Grayscale());
                var result = File(qrCodeAsBitmapByteArr, "image/png");


                return result;
            }
            catch
            {
                return NotFound();
            }
        }

        private byte[] createQrCode()
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            string payload = "{'id': " + 1 + ", 'timestamp':" + DateTime.Now + "}";
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            return qrCode.GetGraphic(10);
        }
    }
}
