using MessengerApi.Models;
using System;
using System.Threading.Tasks;

namespace MessengerApi.DataSources.Contracts
{
	public interface IUserDataSource
	{
		Task<User> Create(Guid id, string name);

		Task<User> Read(Guid id);
	}
}
