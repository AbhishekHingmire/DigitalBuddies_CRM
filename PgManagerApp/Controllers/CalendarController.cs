using Microsoft.AspNetCore.Mvc;

namespace PgManagerApp.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
