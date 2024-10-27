using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PgManagerApp.Models;
using PgManagerApp.Models.Room;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace PgManagerApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDbContext _context;

        public DashboardController(ILogger<DashboardController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
                var model = new DashboardViewModel();
                var users = _context.Users.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList();
                model.UsersSummary = users;

                model.TotalUsers = users.Count().ToString();

                var transactions = _context.Transactions.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList();
                foreach(var usersSum in model.UsersSummary)
                {
                    usersSum.Transactions = transactions.Where(x => x.UserId == usersSum.Id).ToList();
                }

                // Count of users who paid
                var paidUserCount = transactions
                    .Select(t => t.UserId)
                    .Distinct()
                    .Count();

                model.CountPaid = paidUserCount.ToString();

                // Count of users who did not pay
                var userIdsWhoPaid = transactions
                    .Select(t => t.UserId)
                    .Distinct()
                    .ToList();

                var notPaidUserCount = users
                    .Count(u => !userIdsWhoPaid.Contains(u.Id));

                model.CountUnpaid = notPaidUserCount.ToString();

                // Calculate total paid amount after converting string to decimal
                var totalPaidAmount = transactions
                    .Select(t =>
                    {
                        // Try to parse the AmountPaid, defaulting to 0 if parsing fails
                        decimal.TryParse(t.AmountPaid, out var amountPaid);
                        return amountPaid;
                    })
                    .Sum();

                var totalRoomCharge = transactions
                    .Select(t =>
                    {
                        // Try to parse the AmountPaid, defaulting to 0 if parsing fails
                        decimal.TryParse(t.ChargeAmount, out var amountPaid);
                        return amountPaid;
                    })
                    .Sum();

                var totalNotPaidAmount = totalRoomCharge - totalPaidAmount;

                model.TotalPendingAmount = totalNotPaidAmount.ToString();

                model.TotalAmount = totalPaidAmount.ToString();


                //CHartData
                var daysOfWeek = new List<DayOfWeek>
    {
        DayOfWeek.Sunday,
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday
    };

                var weeklyData = transactions
                    .GroupBy(t => t.InitDate.DayOfWeek)
                    .Select(g => new
                    {
                        Day = g.Key,
                        Count = g.Count()
                    })
                    .ToDictionary(x => x.Day, x => x.Count); // Convert to dictionary for easy lookup

                var dataPoints = daysOfWeek.Select(day => new
                {
                    label = day.ToString(),
                    y = weeklyData.ContainsKey(day) ? weeklyData[day] : 0
                }).ToList();

                ViewBag.DataPoints = JsonSerializer.Serialize(dataPoints);
                ViewBag.UserCounts = weeklyData;

                var roomData = new RoomViewModel();
                roomData.MasterId = HttpContext.Session.GetInt32("MasterUserId");

                roomData.Rooms = _context.Rooms.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList();
                roomData.TotalRooms = roomData.Rooms.Count().ToString() ?? "0";
                foreach (var usersummry in model.UsersSummary)
                {
                    usersummry.RoomNumbers = new List<string>();
                    foreach (var tran in usersummry.Transactions)
                    {
                        var RoomNumbers = roomData.Rooms
                            .Where(x => x.Id == tran.RoomId)
                            .Select(x => x.RoomNumber)
                            .ToList();

                            usersummry.RoomNumbers.AddRange(RoomNumbers);
                    }
                    usersummry.RoomsCount = usersummry.RoomNumbers.Count().ToString();

                    var paid = usersummry.Transactions.Where(x => x.UserId == usersummry.Id).Select(x => Convert.ToInt32(x.AmountPaid)).ToList().Sum();
                    var total = usersummry.Transactions.Where(x => x.UserId == usersummry.Id).Select(x => Convert.ToInt32(x.ChargeAmount)).ToList().Sum();
                    usersummry.PendingAmount = (total - paid).ToString();
                    usersummry.ChargeAmount = total.ToString();
                    usersummry.PaidAmount = paid.ToString();
                }



                int counter = 0;
                foreach (var rooms in roomData.Rooms)
                {
                    // LINQ query to count the number of users for the specified room
                    int occupiedSpace = _context.Transactions
                   .Where(t => t.RoomId == rooms.Id && t.MasterId == HttpContext.Session.GetInt32("MasterUserId")) // Filter transactions by the specific RoomId
                   .Select(t => t.UserId) // Select the UserId from the transactions
                   .Distinct() // Ensure unique users (in case of duplicate transactions)
                   .Count(); // Count the number of unique users

                    int remainingSpace = Convert.ToInt32(rooms.Capacity) - occupiedSpace;
                    rooms.RemainingSpace = remainingSpace.ToString();
                    if (rooms.RemainingSpace != "0")
                    {
                        counter++;
                    }
                }
                model.AvailableRooms = counter.ToString();
                return View(model);
           
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
