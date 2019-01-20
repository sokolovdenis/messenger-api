using Abstractions.Models;
using System;
using System.Threading.Tasks;

namespace Abstractions.DataSources
{
	public interface IUserDataSource
	{
		Task<User> CreateAsync(string name);

		Task<User> ReadAsync(Guid id);

		Task<User> DeleteAsync(Guid id);

		Task<long> CountAsync();
	}
}
