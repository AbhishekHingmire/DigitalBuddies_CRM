using System.ComponentModel.DataAnnotations;

namespace PgManagerApp.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
