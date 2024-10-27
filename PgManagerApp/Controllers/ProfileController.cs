using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PgManagerApp.Models;
using System.Security.Cryptography;
using System.Text;

namespace PgManagerApp.Controllers
{
    [Authorize]
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
                var admin = new MasterUser();

                admin = _context.MasterUser.Where(x => x.Id == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault();

                if (admin != null)
                {
                    return View(admin);
                }
                else
                {
                    return RedirectToAction("Login", "Auth");
                }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateMaster(MasterUser model)
        {
                if (ModelState.IsValid)
                {
                    string passHash = HashPassword(model.PasswordHash);
                    model.PasswordHash = passHash;
                    var user = _context.MasterUser.Update(model);
                    _context.SaveChanges();
                    TempData["Message"] = "Profile updated succesfully.";
                    _cache.Remove("Username");
                    _cache.Set("Username", model.Name);
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Something went wrong!";
                }

                return View(model);
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
