using System;

namespace Abstractions.Models
{
	public class Message : ModelBase<long>
	{
		public string ConversationId { get; set; }

		public DateTime Timestamp { get; set; }

		public Guid User { get; set; }

		public string Content { get; set; }
	}
}
