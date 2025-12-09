using System;
namespace Backend.models
{
	public class Ticket
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime DateCreated { get; set; }

		// Foreign Key → User
		public int CreatorUserId { get; set; }
		public User CreatorUser { get; set; } = new User;

		// Foreign Key → TicketStatus
		public int StatusId { get; set; }
		public TicketStatus Status { get; set; }
	}
}
