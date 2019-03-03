using Abstractions.DataSources;
using Abstractions.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Mongo
{
	public class UserDataSource : DataSourceBase<User>, IUserDataSource
	{
		public UserDataSource(MongoConnection connection) : base(connection)
		{ }

		public override void Initialize()
		{
			var indexBuilder = Builders<User>.IndexKeys;
			var indexModel = new CreateIndexModel<User>(indexBuilder.Text(u => u.Name));

			Collection.Indexes.CreateOne(indexModel);
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

		public async Task<IList<User>> SearchAsync(string query)
		{
			var filter = Builders<User>.Filter.Text(query);
			var users = await Collection.FindAsync(filter);
			return await users.ToListAsync();
		}
	}
}
