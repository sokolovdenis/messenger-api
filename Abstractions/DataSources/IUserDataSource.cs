using Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.DataSources
{
	public interface IUserDataSource
	{
		void Initialize();

		Task<User> CreateAsync(string name);

		Task<User> ReadAsync(Guid id);

		Task<User> DeleteAsync(Guid id);

		Task<long> CountAsync();

		Task<IList<User>> SearchAsync(string query);
	}
}
