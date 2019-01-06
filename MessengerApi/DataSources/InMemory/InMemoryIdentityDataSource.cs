using MessengerApi.DataSources.Contracts;
using MessengerApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessengerApi.DataSources.InMemory
{
	public class InMemoryIdentityDataSource : IIdentityDataSource
	{
		readonly Dictionary<Guid, Identity> _identities = new Dictionary<Guid, Identity>();

		public Task<Identity> Create(Guid id, Guid userId, byte[] salt, byte[] hash)
		{
			if (_identities.ContainsKey(id))
			{
				return Task.FromResult<Identity>(null);
			}

			var identity = new Identity()
			{
				Id = id,
				UserId = userId,
				Salt = salt,
				Hash = hash
			};

			_identities.Add(id, identity);

			return Task.FromResult(identity);
		}

		public Task<Identity> Read(Guid id)
		{
			return Task.FromResult(_identities.ContainsKey(id) ? _identities[id] : null);
		}
	}
}
