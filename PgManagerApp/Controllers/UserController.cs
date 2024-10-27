using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PgManagerApp.Models;
using PgManagerApp.Models.Transaction;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PgManagerApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
                var users = new UserRegistration();
                users.MasterId = HttpContext.Session.GetInt32("MasterUserId");
                if (TempData["UserData"] != null)
                {
                    users = JsonConvert.DeserializeObject<UserRegistration>(TempData["UserData"].ToString());
                    int RoomId = _context.Transactions.Where(x => x.UserId == users.Id).Select(x => x.RoomId).FirstOrDefault();
                    string RoomNo = _context.Rooms.Where(x => x.Id == RoomId).Select(x => x.RoomNumber).FirstOrDefault();
                    users.RoomNumber = RoomNo;
                }
                users.Users = _context.Users.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList();

                foreach (var user in users.Users)
                {
                    // Convert nvarchar to decimal and aggregate the totals for each user
                    var totalCharge = _context.Transactions
                        .Where(x => x.UserId == user.Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId"))
                        .Select(x => Convert.ToDecimal(x.ChargeAmount))
                        .Sum();

                    var totalPaid = _context.Transactions
                        .Where(x => x.UserId == user.Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId"))
                        .Select(x => Convert.ToDecimal(x.AmountPaid))
                        .Sum();

                    // Calculate the pending amount
                    var pendingAmount = totalCharge - totalPaid;

                    // Set the pending amount in the user object
                    user.PendingAmount = pendingAmount.ToString();
                }
                string formUrl = _context.FormUrls.Where(a => a.MasterId == HttpContext.Session.GetInt32("MasterUserId")).Select(a => a.Url).FirstOrDefault() ?? "";
                users.FormUrl = formUrl;

                return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(int Id)
        {
                var userDetails = new UserRegistration();
                var transDetails = new TransactionViewModel();

                userDetails = _context.Users.Where(x => x.Id == Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault();
                transDetails = _context.Transactions.Where(x => x.UserId == Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault();
                if (userDetails != null)
                {
                    _context.Users.Remove(userDetails);
                    if (transDetails != null)
                    {
                        _context.Transactions.Remove(transDetails);
                    }
                    _context.SaveChanges();
                }
                else
                {
                    TempData["Error"] = "Something went wrong!";
                }
                TempData["Message"] = "User succesfully deleted.";
                return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(UserRegistration newUser)
        {
                var usr = new UserRegistration();

                // Handle file upload
                if (newUser.ProfilePicture != null && newUser.ProfilePicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");

                    // Ensure directory exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Add unique identifier to file name to prevent collisions
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(newUser.ProfilePicture.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        newUser.ProfilePicture.CopyTo(stream);
                    }

                    // Set the image path in the model
                    newUser.ProfilePicturePath = "/images/profiles/" + uniqueFileName;
                }

                if (newUser.Id == 0)
                {
                    if (ModelState.IsValid)
                    {
                        _context.Users.Add(newUser);
                        _context.SaveChanges();
                        TempData["Message"] = "User successfully added.";
                    }
                    else
                    {
                        TempData["Error"] = "Something went wrong!";
                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        _context.Users.Update(newUser);
                        _context.SaveChanges();
                        TempData["Message"] = "User successfully updated.";
                        return RedirectToAction("Index");
                    }
                    usr = _context.Users.Where(x => x.Id == newUser.Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault();
                    usr.ViewOnly = newUser.ViewOnly;
                    TempData["UserData"] = JsonConvert.SerializeObject(usr);
                }
                return RedirectToAction("Index");
        }

    }
}
