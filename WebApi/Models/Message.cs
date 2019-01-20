using System;

namespace MessengerApi.Models
{
	public class Message
	{
		public long Id { get; set; }

		public DateTime Timestamp { get; set; }

		public Guid User { get; set; }

		public string Content { get; set; }
	}
}
