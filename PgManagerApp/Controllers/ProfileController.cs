using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PgManagerApp.Models;
using System.Security.Cryptography;
using System.Text;

namespace PgManagerApp.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public ProfileController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IActionResult Index()
        {
            string? userId = HttpContext.Request.Cookies["UserId"];
            bool isEmployee = false;
            isEmployee = userId.ToUpper().Contains("EMP");
            ViewBag.IsEmploye = isEmployee;

            var userData = _context.Users.Where(x => x.UserId == userId).SingleOrDefault() ?? new UserRegistration();

            if(!isEmployee)
            {
                userData.LoginUser = _context.LoginUsers.Where(x => x.UserId == userId).SingleOrDefault();
            }
            return View(userData);
        }

        [HttpPost]
        public IActionResult UpdateProfile(UserRegistration model)
        {
            try
            {
                
                bool isEmployee = false;
                string? userId = HttpContext.Request.Cookies["UserId"];

                if (isEmployee)
                {
                    isEmployee = model.UserId.ToUpper().Contains("EMP");
                    ViewBag.IsEmploye = isEmployee;
                    var user = _context.Users.Where(x => x.UserId == userId).SingleOrDefault();
                    string passHash = HashPassword(model.PasswordHash);
                    user.PasswordHash = passHash;
                    user.MobileNumber = model.MobileNumber;
                    user.Email = model.Email;
                    user.Name = model.Name;
                    _context.Users.Update(user);
                    _context.SaveChanges();
                }
                else
                {
                    var admin = _context.LoginUsers.Where(x => x.UserId == userId).SingleOrDefault();
                    string passHash = HashPassword(model.PasswordHash);
                    admin.HashPassword = passHash;
                    _context.LoginUsers.Update(admin);
                    _context.SaveChanges();
                }
                TempData["Message"] = "Profile updated succesfully.";
            }
            catch (Exception ex) {
                TempData["Error"] = "Something went wrong!.";
            }
            return RedirectToAction("Index");
        }

        private string HashPassword(string? password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
