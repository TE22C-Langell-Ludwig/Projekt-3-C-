using Microsoft.EntityFrameworkCore;
using Backend.models;

namespace Backend.data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<User> Users { get; set; }
		public DbSet<Ticket> Tickets { get; set; }
		public DbSet<TicketStatus> TicketStatuses { get; set; }
	}
}
