using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PgManagerApp.Models;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PgManagerApp.Controllers
{
    public class EmployeeDirectoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; // To handle file saving


        public EmployeeDirectoryController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment; 
        }
        public IActionResult Index()
        {
            var model = _context.Users.ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(UserRegistration user, IFormFile ProfilePicture)
        {
            if (ModelState.IsValid)
            {
                // Handle profile picture upload
                if (ProfilePicture != null)
                {
                    // Save the uploaded file to wwwroot/images
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ProfilePicture.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfilePicture.CopyToAsync(fileStream);
                    }

                    // Set the ProfilePicturePath for saving in the database
                    user.ProfilePicturePath = "/images/" + uniqueFileName;
                }

                // Save the employee details to the database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index)); // Redirect back to the employee list
            }

            // If validation fails, return to the same view
            return View(user);
        }

        public IActionResult View(int id)
        {
            var employee = _context.Users.Find(id);
            return PartialView("_ViewEmployee", employee); // Return a partial view
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var employee = _context.Users.Find(id);
            if (employee == null)
            {
                return NotFound(); // Return 404 if not found
            }
            return PartialView("_EditEmployee", employee); // Load partial view for editing
        }

        [HttpPost]
        public IActionResult Edit(UserRegistration employee, IFormFile ProfilePicture)
        {
            if (ModelState.IsValid)
            {
                var existingEmployee = _context.Users.Find(employee.Id);
                if (existingEmployee == null)
                {
                    return NotFound(); // Return 404 if not found
                }

                // Update the employee's properties
                existingEmployee.Name = employee.Name;
                existingEmployee.Email = employee.Email;
                existingEmployee.Designation = employee.Designation;
                existingEmployee.MobileNumber = employee.MobileNumber;
                existingEmployee.Salary = employee.Salary;
                existingEmployee.OnboardDate = employee.OnboardDate;
                existingEmployee.IdentityType = employee.IdentityType;
                existingEmployee.IdentityNumber = employee.IdentityNumber;
                existingEmployee.Address = employee.Address;

                // Handle image upload
                if (ProfilePicture != null && ProfilePicture.Length > 0)
                {
                    var uploads = Path.Combine(_hostEnvironment.WebRootPath, "uploads");

                    // Check if uploads directory exists, if not create it
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    var filePath = Path.Combine(uploads, ProfilePicture.FileName);

                    // Delete the existing profile picture if necessary
                    if (!string.IsNullOrEmpty(existingEmployee.ProfilePicturePath))
                    {
                        var oldFilePath = Path.Combine(uploads, existingEmployee.ProfilePicturePath);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath); // Remove the old file
                        }
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ProfilePicture.CopyTo(stream);
                    }
                    existingEmployee.ProfilePicturePath = Path.Combine("uploads", ProfilePicture.FileName);
                }

                // Save changes to the database
                _context.Users.Update(existingEmployee);
                _context.SaveChanges();
                return Ok(); // Return 200 OK on success
            }
            return BadRequest(); // Return 400 Bad Request on failure
        }



        [HttpPost]
        public async Task<IActionResult> EditEmployee(UserRegistration user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Users.FindAsync(id);
            if (employee != null)
            {
                _context.Users.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
