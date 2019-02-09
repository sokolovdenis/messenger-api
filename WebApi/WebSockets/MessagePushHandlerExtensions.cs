using Microsoft.AspNetCore.Builder;

namespace WebApi.WebSockets
{
	public static class MessagePushHandlerExtensions
	{
		public static IApplicationBuilder UseMessagePushHandler(this IApplicationBuilder builder, string url)
		{
			return builder.UseMiddleware<MessagePushHandler>(url);
		}
	}
}
