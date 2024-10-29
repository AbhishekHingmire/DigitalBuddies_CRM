using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PgManagerApp.Models
{
    public class CheckInOut
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required]
        public string? EmpNameWithDes { get; set; } // Name of the employee

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } // Date of the check-in/out

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan? CheckInTime { get; set; } // Check-in time

        [DataType(DataType.Time)]
        public TimeSpan? CheckOutTime { get; set; } // Check-out time (optional until checked out)

        [NotMapped]
        public TimeSpan? TotalTime { get; set; }
    }

    [NotMapped]
    public class TimeShetViewModel
    {
        public List<CheckInOut>? CheckInOuts { get; set; }
        public List<UserTimer>? UserTimers { get; set; }
        public List<UserRegistration>? UserDetails { get; set; }
        public string? TotalTime { get; set; }
        public bool? IsEmployee { get; set; }
    }

    public class UserTimer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public TimeSpan TotalTime => EndTime - StartTime;
    }

}
