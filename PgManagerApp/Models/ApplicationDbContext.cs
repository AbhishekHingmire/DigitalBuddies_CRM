using Microsoft.EntityFrameworkCore;
using PgManagerApp.Controllers;
using PgManagerApp.Models.Room;
using PgManagerApp.Models.Transaction;

namespace PgManagerApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<UserRegistration> Users { get; set; }
        public DbSet<UserApproval> UserApproval { get; set; }
        public DbSet<TransactionViewModel> Transactions { get; set; }
        public DbSet<RoomViewModel> Rooms { get; set; }
        public DbSet<MasterUser> MasterUser { get; set; }
        public DbSet<FormUrl> FormUrls { get; set; }
    }
}
