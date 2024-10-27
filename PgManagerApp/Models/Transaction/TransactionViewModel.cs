using Microsoft.AspNetCore.Mvc.Rendering;
using PgManagerApp.Models.Room;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PgManagerApp.Models.Transaction
{
    public class TransactionViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "User*")]
        public int UserId { get; set; }

        [NotMapped]
        public string? UserName { get; set; }

        [NotMapped]
        public string? RoomNumber { get; set; }

        [Required]
        [Display(Name = "Room*")]
        public int RoomId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid charge amount.")]
        [Display(Name = "Charge Amount*")]
        [MaxLength(5)]
        public string? ChargeAmount { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid amount paid.")]
        [Display(Name = "Amount Paid*")]
        [MaxLength(5)]
        public string? AmountPaid { get; set; }

        [Required(ErrorMessage="Please enter start date.")]
        [Display(Name = "Start Date*")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please enter end date.")]
        [Display(Name = "End Date*")]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [NotMapped]
        public bool IsEditable { get; set; } = false;

        [NotMapped]
        public List<TransactionViewModel>? Transactions { get; set; }

        [NotMapped]
        public List<SelectListItem>? Rooms { get; set; }

        [NotMapped]
        public List<SelectListItem>? Users { get; set; }
        public int? MasterId { get; set; }
        public DateTime InitDate { get; set; } = DateTime.Today;
    }
}
