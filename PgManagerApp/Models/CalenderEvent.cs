using System.ComponentModel.DataAnnotations;

namespace PgManagerApp.Models
{
    public class CalendarEvent
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }// Date of the event

        [Required]
        public string? EventName { get; set; } // Name of the event
    }
}
