using MessengerApi.Models;
using System;
using System.Threading.Tasks;

namespace MessengerApi.DataSources.Contracts
{
	public interface IIdentityDataSource
	{
		Task<Identity> Create(Guid id, Guid userId, byte[] salt, byte[] hash);

		Task<Identity> Read(Guid id);
	}
}
