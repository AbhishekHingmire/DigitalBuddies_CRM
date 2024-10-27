using System.ComponentModel.DataAnnotations;

namespace PgManagerApp.Models
{
    public class FormUrl
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Url { get; set; }
        [Required]
        public int MasterId { get; set; }
    }
}
