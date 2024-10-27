using Microsoft.AspNetCore.Mvc;

namespace PgManagerApp.Controllers
{
    public class TimeSheetController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
