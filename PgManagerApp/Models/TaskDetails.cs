using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PgManagerApp.Models
{
    public class TaskDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? ProjectId { get; set; }

        [Required]
        public string? AssignedBy { get; set; }

        [Required]
        public string? AssignedToId { get; set; }

        [Required]
        public string TaskDescription { get; set; }

        // New properties for media uploads
        public string? UploadImage { get; set; }
        public string? UploadVideo { get; set; }

        // New property for media description
        public string? MediaDescription { get; set; }

        [Required]
        public string? Priority { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? Comments { get; set; }

        // File upload properties
        [NotMapped]
        public IFormFile? UploadImageFile { get; set; }

        [NotMapped]
        public IFormFile? UploadVideoFile { get; set; }

        [NotMapped]
        public List<TaskDetails>? Tasks { get; set; }
    }

}
