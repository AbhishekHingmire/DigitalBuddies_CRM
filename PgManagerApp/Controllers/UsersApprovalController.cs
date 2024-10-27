using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PgManagerApp.Models;

namespace PgManagerApp.Controllers
{
    [Authorize]
    public class UsersApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public UsersApprovalController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;            
        }
        public IActionResult Index()
        {
                var user = new UserApproval();
                user.UserApprovalList = _context.UserApproval.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList();
                return View(user);
        }

        public IActionResult ApproveRequest(int Id)
        {
                _cache.Remove("approvalCount");
                var reg = new UserRegistration();
                var user = _context.UserApproval.Where(x => x.Id == Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault() ?? new UserApproval();
                var checkExistingUser = _context.Users.Where(x => x.IdentityNumber == user.IdentityNumber && x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault();
                if (user != null)
                {
                    if (checkExistingUser == null)
                    {
                        reg = new UserRegistration()
                        {
                            Id = 0,
                          //MobileNumber = user.MobileNumber,
                            MasterId = user.MasterId,
                            InitDate = DateTime.Today,
                            IdentityType = user.IdentityType,
                            IdentityNumber = user.IdentityNumber,
                            FormUrl = user.FormUrl,
                            Name = user.Name,
                            Designation = user.Designation,
                            CreatedDate = user.CreatedDate,
                            Address = user.Address,
                         // Email = user.Email,
                            PendingAmount = user.PendingAmount,
                            ProfilePicture = user.ProfilePicture,
                            ProfilePicturePath = user.ProfilePicturePath,
                            RoomNumber = user.RoomNumber,
                            ApprovedUser = true,
                            ApprovedUserId = user.Id,
                        };
                        _context.Users.Add(reg);
                        _context.UserApproval.Remove(user);
                        _context.SaveChanges();
                        TempData["Message"] = "Request approved succesfully.";
                    }
                    else
                    {
                        TempData["Error"] = "This user alredy exists in database!";
                    }
                }
                return RedirectToAction("Index");
        }
        public IActionResult DeleteRequest(int Id)
        {
                var user = _context.UserApproval.Where(x => x.Id == Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault() ?? new UserApproval();
                if (user != null)
                {
                    _context.UserApproval.Remove(user);
                    _context.SaveChanges();
                    TempData["Message"] = "Request deleted succesfully!";
                }
                else
                {
                    TempData["Error"] = "Something went wrong!";
                }

                return RedirectToAction("Index");
        }

        public void UserApprovalCountService()
        {
                _cache.Remove("approvalCount");
                int approvalCount = _context.UserApproval.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList().Count();
                _cache.Set("approvalCount", approvalCount);
        }
    }
}
