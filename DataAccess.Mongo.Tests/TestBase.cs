using Microsoft.Extensions.Options;
using System;

namespace DataAccess.Mongo.Tests
{
	public class TestBase
	{
		private static string _dbName = $"Messenger-Test-{DateTime.Now.Ticks}";

		protected static MongoConnection Connection { get; private set; }

		protected static UserDataSource UserDataSource { get; private set; }

		protected static IdentityDataSource IdentityDataSource { get; private set; }

		public static void Init()
		{
			var options = new MongoConnection.Options()
			{
				ConnectionString = "mongodb://localhost:27017",
				Database = _dbName
			};

			Connection = new MongoConnection(Options.Create(options));

			UserDataSource = new UserDataSource(Connection);

			IdentityDataSource = new IdentityDataSource(Connection);
		}

		public static void Cleanup()
		{
			Connection.Client.DropDatabase(_dbName);
		}
	}
}
