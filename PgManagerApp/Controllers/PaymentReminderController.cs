using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PgManagerApp.Models;

namespace PgManagerApp.Controllers
{
    [Authorize]
    public class PaymentReminderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public PaymentReminderController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public IActionResult Index()
        {
                var users = new UserRegistration();
                users.Users = _context.Users.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList();

                foreach (var user in users.Users)
                {
                    // Convert nvarchar to decimal and aggregate the totals for each user
                    var totalCharge = _context.Transactions
                        .Where(x => x.UserId == user.Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId"))
                        .Select(x => Convert.ToInt32(x.ChargeAmount))
                    .Sum();

                    var totalPaid = _context.Transactions
                        .Where(x => x.UserId == user.Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId"))
                        .Select(x => Convert.ToInt32(x.AmountPaid))
                        .Sum();

                    // Calculate the pending amount
                    var pendingAmount = totalCharge - totalPaid;

                    // Set the pending amount in the user object
                    user.PendingAmount = pendingAmount.ToString();
                }
                return View(users);
           
        }

        public void PendingCountService()
        {
                _cache.Remove("PendingCounter");
                int pendingCount = 0;
                var users = new UserRegistration();
                users.Users = _context.Users.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList();
                foreach (var user in users.Users)
                {
                    // Convert nvarchar to decimal and aggregate the totals for each user
                    var totalCharge = _context.Transactions
                        .Where(x => x.UserId == user.Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId"))
                        .Select(x => Convert.ToInt32(x.ChargeAmount))
                    .Sum();

                    var totalPaid = _context.Transactions
                        .Where(x => x.UserId == user.Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId"))
                        .Select(x => Convert.ToInt32(x.AmountPaid))
                        .Sum();

                    // Calculate the pending amount
                    var pendingAmount = totalCharge - totalPaid;

                    // Set the pending amount in the user object
                    user.PendingAmount = pendingAmount.ToString();
                    if (user.PendingAmount != "0")
                    {
                        pendingCount++;
                    }
                }

                _cache.Set("PendingCounter", pendingCount);
        }
    }
}
