using Abstractions.DataSources;
using DataAccess.Mongo;
using DataAccess.Redis;
using MessengerApi.Services;
using MessengerApi.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Reflection;
using WebApi.WebSockets;

namespace MessengerApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<JwtAuthenticationService.Options>(
				Configuration.GetSection("Authentication"));

			JwtAuthenticationService.AddJwtAuthentication(
				services,
				Configuration.GetValue<string>("Authentication:Secret"));

			services.Configure<MongoConnection.Options>(
				Configuration.GetSection("Mongo"));
			services.AddSingleton<MongoConnection>();

			services.AddSingleton<IUserDataSource, UserDataSource>();
			services.AddSingleton<IIdentityDataSource, IdentityDataSource>();

			services.Configure<RedisConnection.Options>(
				Configuration.GetSection("Redis"));
			services.AddSingleton<RedisConnection>();

			services.AddSingleton<IConversationDataSource, ConversationDataSource>();

			services.AddSingleton<IdentityService>();
			services.AddSingleton<JwtAuthenticationService>();

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "Messenger API", Version = "v1" });
				c.IncludeXmlComments(Path.Combine(
					AppContext.BaseDirectory,
					Assembly.GetEntryAssembly().GetName().Name + ".xml"));

				c.OperationFilter<JsonOperationFilter>();
				c.OperationFilter<AuthResponsesOperationFilter>();
				c.OperationFilter<ResponseCodeOperationFilter>();
			});

			services.AddCors();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseDefaultFiles();
			app.UseStaticFiles();

			app.UseWebSockets();
			app.UseMessagePushHandler("/websocket");

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();

			app.UseCors(builder => builder
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader()
			);

			app.UseMvc();

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Messenger API V1");
				c.SupportedSubmitMethods(new SubmitMethod[0]); // disable Try button
			});

			app.UseWebSockets();
			app.UseMessagePushHandler("messages");
		}
	}
}
