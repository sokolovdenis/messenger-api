using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace DataAccess.Mongo
{
	public class MongoConnection
	{
		public class Options
		{
			public string ConnectionString { get; set; }

			public string Database { get; set; }
		}

		public MongoConnection(IOptions<Options> options)
		{
			var optionsValue = options?.Value ?? throw new ArgumentException(nameof(options));

			Client = new MongoClient(options.Value.ConnectionString);
			Database = Client.GetDatabase(options.Value.Database);
		}

		public MongoClient Client { get; private set; }

		public IMongoDatabase Database { get; private set; }
	}
}
