using Microsoft.AspNetCore.Mvc;
using Team2_EarthquakeAlertApp.Models;

namespace Team2_EarthquakeAlertApp.Controllers
{
    public class VictimController : Controller
    {
        private static List<VictimReport> reports = new List<VictimReport>();

        // Show victim form
        public IActionResult ReportVictim()
        {
            return View();
        }

        // Submit form
        [HttpPost]
        public IActionResult SubmitVictim(VictimReport model)
        {
            reports.Add(model);
            return RedirectToAction("ReportSubmitted");
        }

        public IActionResult ReportSubmitted()
        {
            return View();
        }

        // First responders call this every 5 sec
        [HttpGet]
        public IActionResult GetVictims()
        {
            return Json(reports);
        }
    }
}
