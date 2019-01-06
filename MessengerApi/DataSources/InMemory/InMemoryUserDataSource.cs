using MessengerApi.DataSources.Contracts;
using MessengerApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessengerApi.DataSources.InMemory
{
	public class InMemoryUserDataSource : IUserDataSource
	{
		private readonly Dictionary<Guid, User> _users = new Dictionary<Guid, User>();

		public Task<User> Create(Guid id, string name)
		{
			if (_users.ContainsKey(id))
			{
				return Task.FromResult<User>(null);
			}

			var user = new User()
			{
				Id = id,
				Name = name
			};

			_users.Add(id, user);

			return Task.FromResult(user);
		}

		public Task<User> Read(Guid id)
		{
			return Task.FromResult(_users.ContainsKey(id) ? _users[id] : null);
		}
	}
}
