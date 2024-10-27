using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PgManagerApp.Models;

namespace PgManagerApp.Controllers
{
    public class FormController : Controller
    {
        public readonly ApplicationDbContext _context;
        public FormController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? masterId)
        {
            var formData = new UserApproval();
            var master = _context.MasterUser.Where(x => x.Id == masterId).FirstOrDefault();
            if (master != null)
            {
                formData.MasterId = masterId;
                return View(formData);
            }
            else
            {
                TempData["OuterFormError"] = "You are not authorized to fill this form!";
                return View();
            }
        }

        [Authorize]
        public IActionResult GenerateFormLink()
        {
                int masterId = HttpContext.Session.GetInt32("MasterUserId") ?? 0;
                var url = _context.FormUrls.Where(a => a.MasterId == masterId).FirstOrDefault();

                if (url == null)
                {
                    // Generate the URL for the form
                    string formUrl = Url.Action(
                        action: "Index",
                        controller: "Form",
                        values: new { masterId = masterId },
                        protocol: Request.Scheme, // This ensures the URL uses the same protocol as the request (http/https)
                        host: Request.Host.ToString() // This includes the domain name
                    ) ?? "";

                    var urlForm = new FormUrl
                    {
                        MasterId = masterId,
                        Url = formUrl
                    };

                    _context.FormUrls.Add(urlForm);
                    _context.SaveChanges();
                    TempData["Message"] = "Url generated succesfully, tap to view form url.";
                }

                // Redirect to a view that shows the generated URL or any other desired page
                return RedirectToAction("Index", "User");
           
        }


        // Action to handle form submission
        [HttpPost]
        public IActionResult SubmitForm(UserApproval model)
        {

            if (ModelState.IsValid)
            {
                if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");

                    // Ensure directory exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Add unique identifier to file name to prevent collisions
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ProfilePicture.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ProfilePicture.CopyTo(stream);
                    }

                    // Set the image path in the model
                    model.ProfilePicturePath = "/images/profiles/" + uniqueFileName;
                }
                _context.UserApproval.Add(model);
                 _context.SaveChanges();
                TempData["OuterFormError"] = null;
                TempData["OuterFormMessage"] = "Form submitted successfully.";
            }
            else
            {
                TempData["OuterFormError"] = "Something went wrong, please try again later!";
            }

            return View("Index");
        }
    }
}
