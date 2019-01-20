using Abstractions.DataSources;
using Abstractions.Models;
using System.Threading.Tasks;

namespace DataAccess.Mongo
{
	public class UserDataSource : DataSourceBase<User>, IUserDataSource
	{
		public UserDataSource(MongoConnection connection) : base(connection)
		{ }

		public async Task<long> CountAsync()
		{
			return await Collection.EstimatedDocumentCountAsync();
		}

		public async Task<User> CreateAsync(string name)
		{
			var user = new User()
			{
				Name = name
			};

			await Collection.InsertOneAsync(user);

			return user;
		}
	}
}
