using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PgManagerApp.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace PgManagerApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        public AuthController(ApplicationDbContext context, IMemoryCache cache, IConfiguration configuration)
        {
            _context = context;
            _cache = cache;
            _configuration = configuration;
        }   

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterUser(MasterUser model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (_context.MasterUser.Any(u => u.Email == model.Email))
                {
                    TempData["Error"] = $"{model.Email} already in use";
                    return RedirectToAction("Register");
                }

                model.PasswordHash = HashPassword(model.PasswordHash);
                _context.MasterUser.Add(model);
                _context.SaveChanges();
                TempData["Message"] = $"User account has been created, you can log in now.";
                return RedirectToAction("Login");
            }
            return View("Register", model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginUser(LoginRequest login)
        {
            // Validate user credentials
            var user = _context.LoginUsers.SingleOrDefault(x => x.UserId == login.UserId && x.HashPassword == HashPassword(login.Password));

            if (user != null)
            {
                Response.Cookies.Append("UserId", user.UserId, new CookieOptions
                {
                    HttpOnly = true,
                    // Secure = true, // Use only in production for HTTPS
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    SameSite = SameSiteMode.Strict, // Optional: enhances security by limiting cookie exposure
                });
                // Creating claims for user authentication
                var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
            new Claim("Id", user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserId),
        };

                // Retrieve user roles
                var userRoles = _context.UserRoles.Where(x => x.UserId == user.UserId).ToList();
                var roleIds = userRoles.Select(x => x.RoleId).ToList();

                // Add roles to claims
                var roles = _context.Roles.Where(x => roleIds.Contains(x.Id)).ToList();
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }

                // JWT token generation
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(30), // Set token expiry
                    signingCredentials: signIn
                );

                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                // Store JWT in a cookie
                Response.Cookies.Append("JwtToken", jwtToken, new CookieOptions
                {
                    HttpOnly = true,
                    // Secure = true, // Use only in production for HTTPS
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    SameSite = SameSiteMode.Strict, // Optional: enhances security by limiting cookie exposure
                });

                // Redirect to Dashboard
                return RedirectToAction("Index", "Dashboard");
            }

            // Invalid credentials case
            TempData["Error"] = "Invalid credentials!";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            // Clear the JWT token cookie
            Response.Cookies.Delete("JwtToken"); // Remove the JWT token from cookies

            // Optionally clear session data
            //HttpContext.Session.Clear(); // Clear session data

            // Redirect the user to the login page (or homepage)
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteAccount()
        {
            
            return View("Login", "Auth");
        }
    }
}
