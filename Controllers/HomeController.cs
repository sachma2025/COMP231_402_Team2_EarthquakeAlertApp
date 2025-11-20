using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Team2_EarthquakeAlertApp.Models;

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
        private static readonly Random Rand = new();

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
            return View();
        }
        public IActionResult SOS()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendSOS([FromBody] SosRequest request)
        {
            if (request == null)
                return BadRequest("No data received.");

            Console.WriteLine($"SOS received: Lat={request.Latitude}, Lng={request.Longitude}" +
                $", Details={request.Description}");

            return Ok();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();          
            return RedirectToAction("Login");     
        }
        
        [HttpGet]
        public JsonResult GetNewMessage()
        {
            var msg = Messages[Rand.Next(Messages.Count)];
            var time = DateTime.Now.ToString("h:mm:ss tt");
            return Json(new { message = msg, time });
        }
    }
}
