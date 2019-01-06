using System;

namespace MessengerApi.Models
{
	public class Identity
	{
		public Guid Id { get; set; }

		public Guid UserId { get; set; }

		public byte[] Salt { get; set; }

		public byte[] Hash { get; set; }
	}
}
