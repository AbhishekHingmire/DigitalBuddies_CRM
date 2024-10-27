using PgManagerApp.Models.Room;
using PgManagerApp.Models.Transaction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PgManagerApp.Models
{
    [NotMapped]
    public class InvoiceViewModel
    {
        public string? UserName { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public List<RoomDetail>? RoomDetails { get; set; }
        public string? TotalChargeAmount { get; set; }
        public string? TotalPaidAmount { get; set; }
    }

    [NotMapped]
    public class RoomDetail
    {
        public string RoomNumber { get; set; }
        public string ChargeAmount { get; set; }
        public string PaidAmount { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }


    [NotMapped]
    public class UserAddition
    {
        public string UserName { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    [NotMapped]
    public class DashboardViewModel
    {
        public string? TotalUsers { get; set; }
        public string? CountPaid { get; set; }
        public string? CountUnpaid { get; set; }
        public string? TotalAmount { get; set; }
        public string? TotalPendingAmount { get; set; }
        public string? AvailableRooms { get; set; }
        public List<UserRegistration>? UsersSummary { get; set; }
    }

    public class UserRegistration
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(60)]
        public string? Name { get; set; }

        [NotMapped]
        public string? PendingAmount { get; set; }

        [NotMapped]
        public string? PaidAmount { get; set; }

        [NotMapped]
        public string? ChargeAmount { get; set; }

        /*public string? Email { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter valid mobile number.")]
        public string? MobileNumber { get; set; } = "1234567890";*/

        [NotMapped]
        public string? RoomNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Designation is required.")]
        public string? Designation { get; set; }

        [Required(ErrorMessage = "Identity type is required.")]
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

        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime InitDate { get; set; } = DateTime.Now;

        [NotMapped]
        public bool ViewOnly { get; set; }

        [NotMapped]
        public List<UserRegistration>? Users { get; set; }

        public int? MasterId { get; set; }
        public int? ApprovedUserId { get; set; }
        public bool? ApprovedUser { get; set; } = false;

        [NotMapped]
        public List<TransactionViewModel>? Transactions { get; set; }

        [NotMapped]
        public string? RoomsCount { get; set; }

        [NotMapped]
        public List<string>? RoomNumbers { get; set; }
    }


    public class UserApproval
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }

        [NotMapped]
        public string? PendingAmount { get; set; }

        /*public string? Email { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter valid mobile number.")]
        public string? MobileNumber { get; set; } = "1234567890";*/

        [NotMapped]
        public string? RoomNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Designation is required.")]
        public string? Designation { get; set; }

        [Required(ErrorMessage = "Identity type is required.")]
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

        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime InitDate { get; set; } = DateTime.Now;

        [NotMapped]
        public List<UserApproval>? UserApprovalList { get; set; }

        public int? MasterId { get; set; }
    }

}
