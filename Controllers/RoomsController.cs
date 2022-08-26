using System.Diagnostics;
using CoronaCheckIn.Managers;
using Microsoft.AspNetCore.Mvc;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authorization;
using QRCoder;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using static QRCoder.PayloadGenerator;
using System.Text.Json;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;

namespace CoronaCheckIn.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ILogger<RoomsController> _logger;
        private readonly RoomManager _roomManager;
        private readonly SessionManager _sessionManager;

        public RoomsController(ILogger<RoomsController> logger, RoomManager roomManager, SessionManager sessionManager)
        {
            _logger = logger;
            _roomManager = roomManager;
            _sessionManager = sessionManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index([FromQuery] string? name = null, [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null, [FromQuery] Faculty? faculty = null)
        {
            var rooms =
                _roomManager.GetRooms(name: name, sortBy: sortBy, sortOrder: sortOrder, faculty: faculty).ToArray();

            if (rooms.Length > 0)
            {
                ViewBag.room = rooms[0];
            }
            ViewBag.newRoom = new Room();
            return View(rooms);
        }

   
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Details(Guid id, [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            Room room = _roomManager.GetRoom(id);

            if (room == null)
            { return View("404"); }

            if (room.QrCodeCreatedAt == null)
            {
                updateRoomTimestamp(room);
            }

            byte[] qrCodeImage = createQrCode(id);
            ViewBag.QrCode = qrCodeImage;

            var allRoomSessions = _sessionManager.GetSessions(room, includeRoom: true, includeUser: true, sortBy: sortBy, sortOrder: sortOrder);
            var infectedRoomSessions = _sessionManager.GetSessions(room, includeRoom: true, includeUser: true, isInfected: true, sortBy: sortBy, sortOrder: sortOrder);

            var model = (room, allRoomSessions, infectedRoomSessions);
            return View(model);
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
                    var newRoom = _roomManager.UpdateRoom(room);
                }
                else
                {
                    _roomManager.AddRoom(room);
                }
            }

            return Json(room);
        }

        public byte[] GenerateQrCode(Guid id)
        {
            Room room = _roomManager.GetRoom(id);
            updateRoomTimestamp(room);

            return createQrCode(id);
        }

        [HttpGet]
        public IActionResult generateRoomPdf(Guid id)
        {
            Room room = _roomManager.GetRoom(id);

            using (MemoryStream stream = new MemoryStream())
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var headerFont = new XFont("Arial", 24, XFontStyle.Bold);
                var boldFont = new XFont("Arial", 14, XFontStyle.Bold);
                var normalFont = new XFont("Arial", 14, XFontStyle.Regular);
                XTextFormatter tf = new XTextFormatter(gfx);

                var roomNameText = "Raum: " + room.Name;
                gfx.DrawString(
                    roomNameText, headerFont, XBrushes.Black,
                    new XRect(60, 40, page.Width, page.Height),
                    XStringFormats.TopLeft);

                var roomDescText = "Max. Teilnehmer:\n" +
                    "Max. Participants:\n\n" +
                    "Max. Dauer:\n" +
                    "Max. Duration:\n\n" +
                    "Fakultät:\n" +
                    "Faculty:";
                tf.DrawString(
                    roomDescText, boldFont, XBrushes.Black,
                    new XRect(60, 80, page.Width, page.Height),
                    XStringFormats.TopLeft);

                var roomInfoText = room.MaxParticipants.ToString() + " Personen" + "\n\n\n" +
                    room.MaxDuration.ToString() + " Minuten" + "\n\n\n" +
                    room.Faculty.ToString();
                tf.DrawString(
                    roomInfoText, normalFont, XBrushes.Black,
                    new XRect(200, 90, page.Width, page.Height),
                    XStringFormats.TopLeft);

                Stream qrCodeStream() => new MemoryStream(this.createQrCode(id));
                XImage image = XImage.FromStream(qrCodeStream);
                image.Interpolate = false;
                gfx.DrawImage(image, 80, 240);

                var scanText1 = "Bitte scannen Sie den QR-Code mit der Corona-Checkin-App ein!";
                gfx.DrawString(
                    scanText1, normalFont, XBrushes.Black,
                    new XRect(0, 260, page.Width, page.Height),
                    XStringFormats.Center);

                var scanText2 = "Please scan the QR-Code with your Corona-Checkin-App!";
                gfx.DrawString(
                    scanText2, normalFont, XBrushes.Black,
                    new XRect(0, 280, page.Width, page.Height),
                    XStringFormats.Center);

                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf", "corona-checkin_raum_" + room.Name + ".pdf");
            }
        }


        private void updateRoomTimestamp(Room room)
        {
            room.QrCodeCreatedAt = DateTime.UtcNow;

            _roomManager.UpdateRoom(room);
        }

        private byte[] createQrCode(Guid id)
        {
            Room room = _roomManager.GetRoom(id);
            string qrCodeCreatedAt = room.QrCodeCreatedAt.Value.ToUniversalTime().ToString("o", System.Globalization.CultureInfo.InvariantCulture);

            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            var json = new
            {
                roomId = id,
                qrCodeCreatedAt = qrCodeCreatedAt
            };

            var jsonPayload = JsonSerializer.Serialize(json);

            QRCodeData qrCodeData = qrGenerator.CreateQrCode(jsonPayload, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            return qrCode.GetGraphic(10);
        }
    }
}