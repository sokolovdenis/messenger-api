using Abstractions.DataSources;
using Abstractions.Models;
using WebApi.Helpers;
using WebApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.WebSockets
{
	public class MessagePushHandler
	{
		private const int PushMessagePollingInterval = 100;

		private readonly RequestDelegate _next;
		private readonly CancellationToken _applicationStoppingCancellationToken;
		private readonly IConversationDataSource _conversationDataSource;
		private readonly JwtAuthenticationService _jwtAuthentioncationService;
		private readonly string _url;

		public MessagePushHandler(
			RequestDelegate next, 
			IApplicationLifetime appLifetime, 
			IConversationDataSource conversationDataSource,
			JwtAuthenticationService jwtAuthentioncationService,
			string url)
		{
			_next = next;
			_applicationStoppingCancellationToken = appLifetime.ApplicationStopping;
			_conversationDataSource = conversationDataSource;
			_jwtAuthentioncationService = jwtAuthentioncationService;
			_url = url;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (context.Request.Path != _url)
			{
				await _next.Invoke(context);
				return;
			}

			if (!context.WebSockets.IsWebSocketRequest)
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				return;
			}

			var token = context.Request.Query["token"];
			if (string.IsNullOrEmpty(token))
			{
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				return;
			}

			var claimsPrincipal = _jwtAuthentioncationService.ValidateToken(token, out var validatedToken);
			if (claimsPrincipal == null)
			{
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				return;
			}

			try
			{
				using (var socket = await context.WebSockets.AcceptWebSocketAsync())
				{
					var socketClosingTokenSource = new CancellationTokenSource();

					var timeoutDelay = validatedToken.ValidTo - DateTime.UtcNow;
					var sessionTimeoutTokenSource = new CancellationTokenSource(timeoutDelay);

					var unitedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(
						_applicationStoppingCancellationToken, 
						socketClosingTokenSource.Token,
						sessionTimeoutTokenSource.Token
					).Token;

					var pushTask = Task.Run(async () => {
						try
						{
							var currentUserId = claimsPrincipal.GetCurrentUserId();
							await PushMessages(_conversationDataSource, currentUserId, socket, unitedCancellationToken);
						}
						finally
						{
							socketClosingTokenSource.Cancel();
						}
					});

					try
					{
						await ReceiveMessages(socket, unitedCancellationToken);
					}
					finally
					{
						socketClosingTokenSource.Cancel();
					}

					await pushTask; // Wait for any pending send operations to complete before sending the close message
									// (since the WebSocket class is not thread-safe in general)

					await socket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "Server is stopping", CancellationToken.None);
				}
			}
			catch (WebSocketException ex)
			{
				switch (ex.WebSocketErrorCode)
				{
					case WebSocketError.ConnectionClosedPrematurely:
						//_log.LogInformation(ex, "Client connection closed prematurely");
						break;

					default:
						//_log.LogError(ex, "Unexpected WebSocket error");
						break;
				}
			}
		}

		static async Task ReceiveMessages(WebSocket socket, CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				try
				{
					var (response, _) = await ReceiveFullMessage(socket, cancellationToken);
					if (response.MessageType == WebSocketMessageType.Close)
					{
						break;
					}
				}
				catch (OperationCanceledException)
				{
					// Исключение OperationCanceledException обрабатывать не нужно.
					// Выходим из метода при следующей итерации цикла.
				}
			}
		}

		static async Task PushMessages(IConversationDataSource conversationDataSource, Guid userId, WebSocket socket, CancellationToken cancellationToken)
		{
			var messagesQueue = new ConcurrentQueue<Message>();

			conversationDataSource.OnMessage += enqueueMessage;
			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					if (!messagesQueue.TryDequeue(out var message))
					{
						await Task.Delay(PushMessagePollingInterval, cancellationToken);
						continue;
					}

					try
					{
						var json = JsonConvert.SerializeObject(message);
						var buffer = Encoding.UTF8.GetBytes(json);
						await socket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
					}
					catch (OperationCanceledException)
					{
						// Исключение OperationCanceledException обрабатывать не нужно.
						// Выходим из метода при следующей итерации цикла.
					}
				}
			}
			finally
			{
				conversationDataSource.OnMessage -= enqueueMessage;
			}

			void enqueueMessage(object sender, MessageEventArgs messageEventArgs)
			{
				var message = messageEventArgs.Message;
				var conversationId = message.ConversationId;

				// TODO: переписать
				if (conversationId == "public" || 
					conversationId.Contains($"{userId}", StringComparison.OrdinalIgnoreCase))
				{
					messagesQueue.Enqueue(message);
				}
			}
		}

		static async Task<(WebSocketReceiveResult, IEnumerable<byte>)> ReceiveFullMessage(WebSocket socket, CancellationToken cancellationToken)
		{
			WebSocketReceiveResult response;
			var data = new List<byte>();

			var buffer = new byte[4096];
			do
			{
				response = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
				data.AddRange(new ArraySegment<byte>(buffer, 0, response.Count));
			}
			while (!response.EndOfMessage);

			return (response, data);
		}
	}
}
