using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;

namespace DataAccess.Redis
{
	public class RedisConnection : IDisposable
	{
		public class Options
		{
			public string Address { get; set; } = "localhost:6379";

			public int Database { get; set; } = -1;
		}

		private readonly Options _options;

		public ConnectionMultiplexer ConnectionMultiplexer { get; }

		public IDatabase Database { get; }

		public RedisConnection(IOptions<Options> options)
		{
			_options = options?.Value ?? throw new ArgumentException(nameof(options));

			ConnectionMultiplexer = ConnectionMultiplexer.Connect(_options.Address);

			Database = ConnectionMultiplexer.GetDatabase(_options.Database);
		}

		public void Dispose(bool cleanup)
		{
			if (cleanup)
			{
				ConnectionMultiplexer.GetServer(_options.Address).FlushDatabase(_options.Database);
			}
			Dispose();
		}

		public void Dispose()
		{
			ConnectionMultiplexer.Dispose();
		}
	}
}
