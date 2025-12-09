using System.Collections.Generic;
namespace Backend.models
{
	public class TicketStatus
	{
		public int Id { get; set; }
		public string StatusName { get; set; }

		// Navigation — all tickets with this status
		public ICollection<Ticket> Tickets { get; set; }
	}
}
