using Abstractions;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DataAccess.Mongo
{
	public abstract class DataSourceBase<T> where T : ModelBase<Guid>
	{
		protected IMongoCollection<T> Collection { get; }

		public DataSourceBase(MongoConnection connection)
		{
			Collection = connection.Database.GetCollection<T>(typeof(T).Name);
		}

		public virtual void Initialize()
		{ }

		public async Task<long> CountAsync()
		{
			return await Collection.EstimatedDocumentCountAsync();
		}

		public async Task<T> DeleteAsync(Guid id)
		{
			return await Collection.FindOneAndDeleteAsync(u => u.Id == id);
		}

		public async Task<T> ReadAsync(Guid id)
		{
			var cursor = await Collection.FindAsync(m => m.Id == id);
			return await cursor.SingleOrDefaultAsync();
		}
	}
}
