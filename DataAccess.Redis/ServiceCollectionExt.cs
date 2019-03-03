using Abstractions.DataSources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Redis
{
	public static class ServiceCollectionExt
	{
		public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<RedisConnection.Options>(configuration);
			services.AddSingleton<RedisConnection>();
			services.AddSingleton<IConversationDataSource, ConversationDataSource>();
		}
	}
}
