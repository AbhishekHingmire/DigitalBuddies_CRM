using Microsoft.AspNetCore.Mvc;
using PgManagerApp.Models;

namespace PgManagerApp.Controllers
{
    public class TaskDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;// Update with your actual DbContext

        public TaskDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            TaskDetails model = new TaskDetails();
            model.Tasks = _context.TaskDetails.ToList();

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

                // Save taskModel to the database using your DbContext
                _context.TaskDetails.Add(taskModel);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // Redirect to the index page or another action
            }

            // If we got this far, something failed; redisplay the form.
            return View(taskModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(TaskDetails taskModel)
        {
            var taskDetails = _context.TaskDetails.Find(taskModel.Id);

            taskDetails.TaskDescription = taskModel.TaskDescription;
            taskDetails.Status = taskModel.Status;
            taskDetails.Priority = taskModel.Priority;

            _context.TaskDetails.Update(taskDetails);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
