using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PgManagerApp.Models;

namespace PgManagerApp.Controllers
{
    [Authorize(Roles = " Admin")]
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

        [HttpPost]
        public IActionResult EditCheckInOut([FromBody] CheckInOut model)
        {
            if (ModelState.IsValid)
            {
                var record = _context.CheckInOuts.Find(model.Id);
                if (record != null)
                {
                    record.EmpNameWithDes = model.EmpNameWithDes;
                    record.Date = model.Date;
                    record.CheckInTime = model.CheckInTime;
                    record.CheckOutTime = model.CheckOutTime;

                    _context.SaveChanges(); // Save changes to the database
                    return Ok();
                }
                return NotFound("Record not found.");
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public IActionResult DeleteCheckInOut([FromBody] CheckInOut model)
        {
            if (model.Id > 0 )
            {
                var record = _context.CheckInOuts.Find(model.Id);
                if (record != null)
                {
                    _context.CheckInOuts.Remove(record);
                    _context.SaveChanges(); // Save changes to the database
                    return Ok();
                }
                return NotFound("Record not found.");
            }
            return BadRequest(ModelState);
        }
    }
}
