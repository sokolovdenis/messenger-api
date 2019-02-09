using Abstractions.DataSources;
using Abstractions.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DataAccess.Mongo
{
	public class IdentityDataSource : DataSourceBase<Identity>, IIdentityDataSource
	{
		public IdentityDataSource(MongoConnection connection) : base(connection)
		{ }

		public async Task<Identity> CreateAsync(Guid id, Guid userId, byte[] salt, byte[] hash)
		{
			var cursor = await Collection.FindAsync(m => m.Id == id);
			if (cursor.Any())
			{
				return null;
			}

			var identity = new Identity()
			{
				Id = id,
				UserId = userId,
				Salt = salt,
				Hash = hash
			};

			await Collection.InsertOneAsync(identity);

			return identity;
		}
	}
}
