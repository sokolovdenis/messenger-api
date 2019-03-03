using Abstractions.DataSources;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Mongo
{
	public static class ServiceCollectionExt
	{
		public static void AddMongo(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<MongoConnection.Options>(configuration);
			services.AddSingleton<MongoConnection>();
			services.AddSingleton<IUserDataSource, UserDataSource>();
			services.AddSingleton<IIdentityDataSource, IdentityDataSource>();
		}

		public static void InitializeMongo(this IApplicationBuilder applicationBuilder)
		{
			var sp = applicationBuilder.ApplicationServices;

			sp.GetRequiredService<IUserDataSource>()
				.Initialize();
		}
	}
}
