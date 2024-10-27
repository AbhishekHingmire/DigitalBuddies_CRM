using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PgManagerApp.Models.Room
{
    public class RoomViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Enter room number/name")]
        [Display(Name = "Room Number or Name*")]
        [StringLength(10)]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be a valid.")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Enter room capacity")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
        [Display(Name = "Capacity*")]
        [StringLength(2)]
        public string Capacity { get; set; }

        [NotMapped]
        public string? RemainingSpace { get; set; }

        [Display(Name = "Hot water")]
        public bool HasHotWater { get; set; } = false;

        [Display(Name = "Shower")]
        public bool HasShower { get; set; } = false;

        [Display(Name = "Commode")]
        public bool HasCommode { get; set; } = false;

        [Display(Name = "Wifi")]
        public bool HasWiFi { get; set; } = false;

        [NotMapped]
        public bool IsEditable { get; set; } = false;

        [NotMapped]
        public List<RoomViewModel>? Rooms { get; set; }
        public int? MasterId { get; set; }

        [NotMapped]
        public string? TotalRooms { get; set; }

        [NotMapped]
        public string? AvailableRooms { get; set; }
    }
}
