using Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.DataSources
{
	public interface IConversationDataSource
	{
		Task<List<Conversation>> GetConversationsAsync(Guid userId);

		Task<Conversation> GetPublicConversationAsync();

		Task<List<Message>> GetMessagesAsync(Guid user1, Guid user2, long from, long count);

		Task<List<Message>> GetPublicMessagesAsync(long from, long count);

		Task<Message> PostMessageAsync(Guid fromUser, Guid toUser, string content);

		Task<Message> PostPublicMessageAsync(Guid fromUser, string content);

		event EventHandler<MessageEventArgs> OnMessage;
	}

	public class MessageEventArgs : EventArgs
	{
		public MessageEventArgs(Message message)
		{
			Message = message;
		}

		public Message Message { get; }
	}
}
