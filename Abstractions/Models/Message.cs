using System;

namespace Abstractions.Models
{
	public class Message : ModelBase<long>
	{
		/// <summary>
		/// Conversation ID where this message was written.
		/// </summary>
		public string ConversationId { get; set; }

		/// <summary>
		/// Created timestamp.
		/// </summary>
		public DateTime Timestamp { get; set; }

		/// <summary>
		/// User who wrote this message.
		/// </summary>
		public Guid User { get; set; }

		/// <summary>
		/// Content.
		/// </summary>
		public string Content { get; set; }
	}
}
