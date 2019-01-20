using System;

namespace Abstractions.Models
{
	public class User : ModelBase
	{
		public string Name { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is User model)) return false;

			return
				Id == model.Id &&
				Name == model.Name;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id, Name);
		}
	}
}
