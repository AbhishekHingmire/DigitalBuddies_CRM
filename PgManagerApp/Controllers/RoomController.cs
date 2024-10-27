using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PgManagerApp.Models;
using PgManagerApp.Models.Room;

namespace PgManagerApp.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
                var roomData = new RoomViewModel();
                roomData.MasterId = HttpContext.Session.GetInt32("MasterUserId");

                if (TempData["RoomData"] != null)
                {
                    roomData = JsonConvert.DeserializeObject<RoomViewModel>(TempData["RoomData"].ToString()) ?? new RoomViewModel();
                }
                roomData.Rooms = _context.Rooms.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList();
                roomData.TotalRooms = roomData.Rooms.Count().ToString() ?? "0";
                int counter = 0;
                foreach (var rooms in roomData.Rooms)
                {
                    // LINQ query to count the number of users for the specified room
                    int occupiedSpace = _context.Transactions
                   .Where(t => t.RoomId == rooms.Id && t.MasterId == HttpContext.Session.GetInt32("MasterUserId")) // Filter transactions by the specific RoomId
                   .Select(t => t.UserId) // Select the UserId from the transactions
                   .Count(); // Count the number of unique users

                    int remainingSpace = Convert.ToInt32(rooms.Capacity) - occupiedSpace;
                    rooms.RemainingSpace = remainingSpace.ToString();
                    if(rooms.RemainingSpace != "0")
                    {
                        counter++;
                    }
                }
                roomData.AvailableRooms = counter.ToString() ?? "0";
                return View(roomData);
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(RoomViewModel model)
        {
                var room = new RoomViewModel();

                if (model.Id == 0)
                {
                    if (ModelState.IsValid)
                    {
                        _context.Rooms.Add(model);
                        _context.SaveChanges();
                        TempData["Message"] = $"Room {model.RoomNumber} added succesfully";
                    }
                }
                else
                {
                    //Update if valid
                    if (ModelState.IsValid)
                    {
                        _context.Rooms.Update(model);
                        _context.SaveChanges();
                        TempData["Message"] = $"Room {model.RoomNumber} updated succesfuly.";
                    }
                    //Populate popup for update if its IsEditable true
                    else
                    {
                        room = _context.Rooms.Where(x => x.Id == model.Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault();
                        room.IsEditable = model.IsEditable;
                        TempData["RoomData"] = JsonConvert.SerializeObject(room);
                    }
                }
                return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRoom(int Id)
        {
                var room = new RoomViewModel();
                room = _context.Rooms.Where(x => x.Id == Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault();
                if (room != null)
                {
                    _context.Rooms.Remove(room);
                    _context.SaveChanges();
                }
                TempData["Message"] = $"Room {room.RoomNumber} succesfully deleted.";
                return RedirectToAction("Index");
        }
    }
}
