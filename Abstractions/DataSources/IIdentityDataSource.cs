using Abstractions.Models;
using System;
using System.Threading.Tasks;

namespace Abstractions.DataSources
{
	public interface IIdentityDataSource
	{
		Task<Identity> CreateAsync(Guid id, Guid userId, byte[] salt, byte[] hash);

		Task<Identity> ReadAsync(Guid id);

		Task<Identity> DeleteAsync(Guid id);
	}
}
