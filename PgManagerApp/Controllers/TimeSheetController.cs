using Microsoft.AspNetCore.Mvc;
using PgManagerApp.Models;

namespace PgManagerApp.Controllers
{
    public class TimeSheetController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TimeSheetController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            var timesheet = new TimeShetViewModel();
            timesheet.CheckInOuts = _context.CheckInOuts.ToList()?? new List<CheckInOut>();
            return View(timesheet);
        }
    }
}
