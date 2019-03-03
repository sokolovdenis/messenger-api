using System;

namespace Abstractions.Models
{
	public class Conversation : ModelBase<string>
	{
		/// <summary>
		/// Last message in this conversation.
		/// </summary>
		public Message LastMessage { get; set; }

		/// <summary>
		/// User who is the second perticipant in this conversation. You are the first participant.
		/// Null for public conversation.
		/// </summary>
		public Guid? Participant { get; set; }
	}
}
