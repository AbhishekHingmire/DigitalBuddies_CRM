﻿using Microsoft.EntityFrameworkCore;

namespace PgManagerApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<UserRegistration> Users { get; set; }
        public DbSet<MasterUser> MasterUser { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<TaskDetails> TaskDetails { get; set; }
        public DbSet<CheckInOut> CheckInOuts { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
    }
}
