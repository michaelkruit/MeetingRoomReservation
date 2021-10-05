using MeetingRooms.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRooms.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<MeetingRoom> MeetingRooms { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Attendees> Attendees { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
