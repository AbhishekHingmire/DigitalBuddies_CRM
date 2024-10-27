using Microsoft.AspNetCore.Mvc;

namespace PgManagerApp.Controllers
{
    public class CheckInOutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
