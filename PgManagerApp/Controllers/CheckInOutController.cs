using Microsoft.AspNetCore.Mvc;
using PgManagerApp.Models;

namespace PgManagerApp.Controllers
{
    public class CheckInOutController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CheckInOutController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var empList = _context.Users.ToList() ?? new List<UserRegistration>();
            ViewBag.Employees = empList;

            var model = _context.CheckInOuts.ToList() ?? new List<CheckInOut>();
            return View(model);
        }

        [HttpPost]
        public IActionResult AddCheckInOut(CheckInOut model)
        {
            if(ModelState.IsValid)
            {
                _context.CheckInOuts.Add(model);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
