using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PgManagerApp.Models;

namespace PgManagerApp.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class TaskDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;// Update with your actual DbContext

        public TaskDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            string? userId = HttpContext.Request.Cookies["UserId"];
            bool isEmployee = false;
            isEmployee = userId.ToUpper().Contains("EMP");

            ViewBag.IsEmployee = isEmployee;

            TaskDetails model = new TaskDetails();
            model.Tasks = isEmployee ? _context.TaskDetails.Where(x => x.AssignedToId == userId).ToList() : _context.TaskDetails.ToList();

            var users = _context.Users.ToList(); // Get users from the database
            ViewBag.Users = users; // Pass users to the view

            var projects = _context.Projects.ToList(); 
            ViewBag.Projects = projects;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskDetails taskModel)
        {
            if (ModelState.IsValid)
            {
                if (taskModel.UploadImageFile != null)
                {
                    var imagePath = Path.Combine("wwwroot/uploads", taskModel.UploadImageFile.FileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await taskModel.UploadImageFile.CopyToAsync(stream);
                    }
                    taskModel.UploadImage = taskModel.UploadImageFile.FileName; // Save the file name in the model
                }

                if (taskModel.UploadVideoFile != null)
                {
                    var videoPath = Path.Combine("wwwroot/uploads", taskModel.UploadVideoFile.FileName);
                    using (var stream = new FileStream(videoPath, FileMode.Create))
                    {
                        await taskModel.UploadVideoFile.CopyToAsync(stream);
                    }
                    taskModel.UploadVideo = taskModel.UploadVideoFile.FileName; // Save the file name in the model
                }

                // Get user contact
                var userContact = await _context.Users.SingleOrDefaultAsync(x => x.UserId == taskModel.AssignedToId);
                if (userContact != null)
                {
                    // Generate WhatsApp link
                    var message = $"You have been assigned a new task by {taskModel.AssignedBy}, please check.";
                    var whatsappUrl = GenerateWhatsAppUrl(message, userContact.MobileNumber);
                    TempData["WhatsappNoti"] = whatsappUrl;
                }

                // Save taskModel to the database
                _context.TaskDetails.Add(taskModel);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // Redirect to the index page or another action
            }

            // If we got this far, something failed; redisplay the form.
            return View(taskModel);
        }

        private string GenerateWhatsAppUrl(string message, string phoneNumber)
        {
            string encodedMessage = Uri.EscapeDataString(message);
            return $"https://web.whatsapp.com/send?phone={phoneNumber}&text={encodedMessage}";
        }


        [HttpPost]
        public async Task<IActionResult> UpdateTask(TaskDetails taskModel)
        {
            var taskDetails = _context.TaskDetails.Find(taskModel.Id);

            taskDetails.Status = taskModel.Status;
            if (taskModel.AssignedToId != null)
            {
                taskDetails.AssignedToId = taskModel.AssignedToId;
            }

            _context.TaskDetails.Update(taskDetails);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(int Id)
        {
            var taskDetails = await _context.TaskDetails.FindAsync(Id);

            if (taskDetails != null)
            {
                _context.TaskDetails.Remove(taskDetails);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }



    }
}
