using System;

namespace Abstractions.Models
{
	public class Conversation : ModelBase<string>
	{
		public Message LastMessage { get; set; }

		public Guid? Participant { get; set; }
	}
}
