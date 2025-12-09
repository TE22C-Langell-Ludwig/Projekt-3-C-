using System.Collections.Generic;
namespace Backend.models
{
	public class User
	{
		public int Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Role { get; set; } = string.Empty;

	// Navigation — all tickets created by this user
	public ICollection<Ticket> TicketsCreated { get; set; } = new List<Ticket>();
	}
}
