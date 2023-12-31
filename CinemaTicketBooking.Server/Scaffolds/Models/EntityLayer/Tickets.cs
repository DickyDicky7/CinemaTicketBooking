using System;
using System.Text.Json.Serialization;

namespace CinemaTicketBooking.Server.Scaffolds.Models.EntityLayer
{
	public class Tickets : BaseEntity
	{
		public Tickets() : base()
		{
		}

		public Tickets(long id) : base()
		{
			Id = id;
		}

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public long? Id { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public long? ShowtimeId { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public long? BillId { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public decimal? Price { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public bool? Checked { get; set; }
	}
}
