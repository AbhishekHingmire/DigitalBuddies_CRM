using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PgManagerApp.Models;
using PgManagerApp.Models.Transaction;

namespace PgManagerApp.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
                var model = new TransactionViewModel();
                model.MasterId = HttpContext.Session.GetInt32("MasterUserId");

                if (TempData["TransData"] != null)
                {
                    model = JsonConvert.DeserializeObject<TransactionViewModel>(TempData["TransData"].ToString()) ?? new TransactionViewModel();
                }

                //For user names
                model.Users = _context.Users.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Name
                }).ToList();

                //For transactions
                model.Transactions = _context.Transactions.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).Select(t => new TransactionViewModel
                {
                    Id = t.Id,
                    UserName = _context.Users.FirstOrDefault(u => u.Id == t.UserId).Name, // Map user name
                    RoomNumber = _context.Rooms.FirstOrDefault(u => u.Id == t.RoomId).RoomNumber,
                    ChargeAmount = t.ChargeAmount,
                    AmountPaid = t.AmountPaid,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate
                }).ToList();

                //Same for rooms
                var rooms = _context.Rooms.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList();

                foreach (var room in rooms)
                {
                    // LINQ query to count the number of users for the specified room
                    int occupiedSpace = _context.Transactions
                   .Where(t => t.RoomId == room.Id && t.MasterId == HttpContext.Session.GetInt32("MasterUserId")) // Filter transactions by the specific RoomId
                   .Select(t => t.UserId) // Select the UserId from the transactions
                   .Count(); // Count the number of unique users

                    int remainingSpace = Convert.ToInt32(room.Capacity) - occupiedSpace;
                    room.RemainingSpace = remainingSpace.ToString();
                }
                var availableRooms = rooms
        .Where(r => !string.IsNullOrEmpty(r.RemainingSpace) && int.TryParse(r.RemainingSpace, out int remainingSpace) && remainingSpace > 0)
        .Select(r => new SelectListItem
        {
            Value = r.Id.ToString(),
            Text = r.RoomNumber
        })
        .ToList();


                model.Rooms = availableRooms;

                return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(TransactionViewModel model) 
        {
                var tran = new TransactionViewModel();

                if (model.Id == 0)
                {
                    if (ModelState.IsValid)
                    {
                        _context.Transactions.Add(model);
                        _context.SaveChanges();
                        TempData["Message"] = "Transaction added.";
                    }
                    else
                    {
                        TempData["Error"] = "Something went wrong!";
                    }
                }
                else
                {
                    //Update if valid
                    if (ModelState.IsValid)
                    {
                        _context.Transactions.Update(model);
                        _context.SaveChanges();
                        TempData["Message"] = "Transaction updated succesfuly.";
                    }

                    //Populate popup for update if its IsEditable true
                    else
                    {
                        tran = _context.Transactions.Where(x => x.Id == model.Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault();
                        tran.IsEditable = model.IsEditable;
                        TempData["TransData"] = JsonConvert.SerializeObject(tran);
                    }
                }
                return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTransaction(int Id)
        {
                var transaction = new TransactionViewModel();
                transaction = _context.Transactions.Where(x => x.Id == Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault();
                if (transaction != null)
                {
                    _context.Transactions.Remove(transaction);
                    _context.SaveChanges();
                }
                TempData["Message"] = "Transaction succesfully deleted.";
                return RedirectToAction("Index");
        }
    }
}
