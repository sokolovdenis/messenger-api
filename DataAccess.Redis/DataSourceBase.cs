using StackExchange.Redis;

namespace DataAccess.Redis
{
	public abstract class DataSourceBase
	{
		protected IDatabase Database { get; }

		public DataSourceBase(RedisConnection connection)
		{
			Database = connection.Database;
		}
	}
}
