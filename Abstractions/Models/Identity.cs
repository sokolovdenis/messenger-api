using System;
using System.Linq;

namespace Abstractions.Models
{
	public class Identity : ModelBase<Guid>
	{
		public Guid UserId { get; set; }

		public byte[] Salt { get; set; }

		public byte[] Hash { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is Identity model)) return false;

			return 
				Id == model.Id && 
				UserId == model.UserId &&
				Enumerable.SequenceEqual(Salt, model.Salt) &&
				Enumerable.SequenceEqual(Hash, model.Hash);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id, UserId, Salt, Hash);
		}
	}
}
