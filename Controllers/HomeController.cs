using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Team2_EarthquakeAlertApp.Models;
using Team2_EarthquakeAlertApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

using System.Threading.Tasks;

namespace Team2_EarthquakeAlertApp.Controllers
{
    public class HomeController : Controller
    {
        private static readonly List<string> Messages = new()
        {
            "Aftershock detected near downtown area.",
            "Emergency services dispatched to collapsed building.",
            "Power outage affecting east district.",
            "Minor tremor recorded 2 km offshore.",
            "Bridge inspection underway due to quake impact."
        };

        private readonly DynamoDbService _dynamoDbService = new DynamoDbService();
        private static readonly Random Rand = new();

        private readonly IWebHostEnvironment _env;

        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string token, string email, string password)
        {
            var user = UserData.Users.FirstOrDefault(u =>
                u.Token == token &&
                u.Email == email &&
                u.Password == password);

            // If no matching user found
            if (user == null)
            {
                ViewBag.Error = "Invalid token, email, or password.";
                return View();
            }

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);

            // If valid First Responder
            if (user.Role == "FirstResponder")
            {
                return RedirectToAction("firstRespDashboard");
            }

            // If valid Disaster Victim (for future use)
            if (user.Role == "DisasterVictim")
            {
                return RedirectToAction("SOS");
            }

            // Fallback (should never hit here)
            ViewBag.Error = "Invalid role.";
            return View();
        }


        public IActionResult firstRespDashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "FirstResponder")
            {
                return RedirectToAction("Login");
            }

            return View();
        }
        public IActionResult SOS()
        {
            return View();
        }
        public IActionResult Resources()
        {
            return View();
        }
        public IActionResult NearestHospital()
        {
            return View();
        }
        public async Task<IActionResult> DistressAlerts()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "FirstResponder")
            {
                return RedirectToAction("Login");
            }

            List<SosRequest> activeAlerts = await _dynamoDbService.GetActiveAlerts();

            return View(activeAlerts);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptAlert([FromBody] Dictionary<string, string> data)
        {
            if (data == null || !data.TryGetValue("timestamp", out string timestamp))
            {
                return BadRequest("Missing alert timestamp.");
            }

            string result = await _dynamoDbService.UpdateAlertStatus(timestamp, "Accepted");

            if (result.StartsWith("Error"))
            {
                return StatusCode(500, result);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendSOS(SosRequest request, IFormFile? photo)
        {
            if (request == null)
                return BadRequest("Invalid request.");

            if (photo != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                string savePath = Path.Combine(_env.WebRootPath, "sos_photos", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                request.photoUrl = "/sos_photos/" + fileName;
            }

            request.timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            // Save to DynamoDB
            string result = await _dynamoDbService.SendAlert(request);

            return Ok("SOS submitted successfully.");
        }


        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();          
            return RedirectToAction("Login");     
        }

        public IActionResult VictimForm()
        {
            // Auto-fill if logged in
            ViewBag.UserName = HttpContext.Session.GetString("UserEmail") ?? "";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VictimForm(VictimReport report, IFormFile? injuryPhoto)
        {
            // Auto-fill name if logged in
            string? sessionUser = HttpContext.Session.GetString("UserEmail");

            if (!string.IsNullOrEmpty(sessionUser))
                report.Name = sessionUser;

            
            if (injuryPhoto != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(injuryPhoto.FileName);
                string savePath = Path.Combine(_env.WebRootPath, "victim_photos", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await injuryPhoto.CopyToAsync(stream);
                }

                report.PhotoPath = "/victim_photos/" + fileName;
            }

            
            await _dynamoDbService.SaveVictimReport(report);

            return RedirectToAction("SOS");
        }

        public DynamoDbService Get_dynamoDbService()
        {
            return _dynamoDbService;
        }

        private static int _incidentIndex = 0;

        [HttpGet]
        public JsonResult GetNewMessage()
        {
            var incidents = new[]
            {
                new { message = "Bridge inspection underway due to quake impact.", lat = 49.2734, lng = -123.1219 },
                new { message = "Gas leak detected on 4th Avenue.", lat = 49.2639, lng = -123.1681 },
                new { message = "Power outage affecting Yaletown district.", lat = 49.2765, lng = -123.1231 },
                new { message = "Aftershock detected near downtown Vancouver.", lat = 49.2827, lng = -123.1207 },
                new { message = "Collapsed building reported in Gastown.", lat = 49.2834, lng = -123.1080 },
                new { message = "Multiple injuries reported at Chinatown.", lat = 49.2796, lng = -123.1021 }
            };
            var selected = incidents[_incidentIndex];

            _incidentIndex = (_incidentIndex + 1) % incidents.Length;

            return Json(new
            {
                message = selected.message,
                lat = selected.lat,
                lng = selected.lng,
                time = DateTime.Now.ToString("h:mm:ss tt")
            });
        }

    }
}
