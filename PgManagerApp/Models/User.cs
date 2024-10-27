using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PgManagerApp.Models
{
    [NotMapped]
    public class DashboardViewModel
    {
        public string? TotalUsers { get; set; }
        public string? TotalTasks { get; set; }
        public string? TotalProjects { get; set; }
        public string? HolidayList { get; set; }
        public string? CreateDesignation { get; set; }
        public string? CreateProject { get; set; }
    }

    public class UserRegistration
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(60)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        public string? Designation { get; set; }

        [Required(ErrorMessage = "Mobile no is required.")]
        [StringLength(10)]
        public string? MobileNumber { get; set; }

        public string? Address { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        public string? Salary { get; set; }

        public string? OnboardDate { get; set; }

        public string? IdentityType { get; set; }

        public string? IdentityNumber { get; set; }
        [NotMapped]
        public string? FormUrl { get; set; }

        [Required(ErrorMessage = "Please upload identity.")]
        [Display(Name = "Identity Proof")]
        [NotMapped]
        public IFormFile? ProfilePicture { get; set; } // Not mapped to DB, used for the file upload

        // New Property to store the image path
        public string? ProfilePicturePath { get; set; } // This will store the path of the uploaded image

        [NotMapped]
        public List<UserRegistration>? Users { get; set; }

        public int? MasterId { get; set; }
    }

}
