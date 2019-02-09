using Abstractions.DataSources;
using Abstractions.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Redis
{
	public class ConversationDataSource : DataSourceBase, IConversationDataSource
	{
		public ConversationDataSource(RedisConnection connection) 
			: base(connection)
		{
			connection.ConnectionMultiplexer.GetSubscriber().Subscribe("feed", RaiseMessageEvent);
		}

		private void RaiseMessageEvent(RedisChannel channel, RedisValue value)
		{
			var message = JsonConvert.DeserializeObject<Message>(value);
			OnMessage(this, new MessageEventArgs(message));
		}

		public event EventHandler<MessageEventArgs> OnMessage;

		public async Task<List<Conversation>> GetConversationsAsync(Guid userId)
		{
			var dbKey = getUserConversationsListKey(userId);

			var conversationsJson = await Database.HashGetAllAsync(dbKey);

			var conversations = conversationsJson.Select(c => new Conversation()
			{
				Id = c.Name,
				LastMessage = JsonConvert.DeserializeObject<Message>(c.Value),
				Participant = getParticipantId(c.Name, userId)
			});

			return conversations.ToList();
		}

		public async Task<Conversation> GetPublicConversationAsync()
		{
			var dbKey = getConversationKey("public", "messages");

			var lastMessageJson = await Database.ListGetByIndexAsync(dbKey, -1);

			var conversation = new Conversation()
			{
				Id = "public",
				LastMessage = JsonConvert.DeserializeObject<Message>(lastMessageJson)
			};

			return conversation;
		}

		public async Task<List<Message>> GetMessagesAsync(Guid fromUser, Guid toUser, long from, long count)
		{
			var conversationId = getConversationId(fromUser, toUser);

			var dbKey = getConversationKey(conversationId, "messages");

			var dbValues = await Database.ListRangeAsync(dbKey, from, from + count - 1);

			var messages = dbValues.Select(dbVal => JsonConvert.DeserializeObject<Message>(dbVal));

			return messages.ToList();
		}

		public async Task<List<Message>> GetPublicMessagesAsync(long from, long count)
		{
			var dbKey = getConversationKey("public", "messages");

			var dbValues = await Database.ListRangeAsync(dbKey, from, from + count - 1);

			var messages = dbValues.Select(dbVal => JsonConvert.DeserializeObject<Message>(dbVal));

			return messages.ToList();
		}

		public async Task<Message> PostMessageAsync(Guid fromUser, Guid toUser, string content)
		{
			const string script = @"
				local nextId = redis.call('INCR', KEYS[1]);
				local msg = cjson.decode(ARGV[1]);
				msg[""Id""] = nextId - 1;
				local json = cjson.encode(msg);
				redis.call('RPUSH', KEYS[2], json);
				redis.call('HSET', KEYS[3], ARGV[2], json);
				redis.call('HSET', KEYS[4], ARGV[2], json);
				redis.call('PUBLISH', 'feed', json);
				return json;
			";

			var conversationId = getConversationId(fromUser, toUser);

			var messageJson = await Database.ScriptEvaluateAsync(script,
				new RedisKey[] {
					getConversationKey(conversationId, "nextId"),
					getConversationKey(conversationId, "messages"),
					getUserConversationsListKey(fromUser),
					getUserConversationsListKey(toUser)
				}, 
				new RedisValue[] {
					JsonConvert.SerializeObject(new Message
					{
						ConversationId = conversationId,
						User = fromUser,
						Timestamp = DateTime.UtcNow,
						Content = content
					}),
					conversationId
				});

			var message = JsonConvert.DeserializeObject<Message>(messageJson.ToString());

			return message;
		}

		public async Task<Message> PostPublicMessageAsync(Guid fromUser, string content)
		{
			const string script = @"
				local nextId = redis.call('INCR', KEYS[1]);
				local msg = cjson.decode(ARGV[1]);
				msg[""Id""] = nextId - 1;
				local json = cjson.encode(msg);
				redis.call('RPUSH', KEYS[2], json);
				redis.call('PUBLISH', 'feed', json);
				return json;
			";

			var conversationId = "public";

			var messageJson = await Database.ScriptEvaluateAsync(script,
				new RedisKey[] {
					getConversationKey(conversationId, "nextId"),
					getConversationKey(conversationId, "messages")
				},
				new RedisValue[] {
					JsonConvert.SerializeObject(new Message
					{
						ConversationId = conversationId,
						User = fromUser,
						Timestamp = DateTime.UtcNow,
						Content = content
					}),
					conversationId
				});

			var message = JsonConvert.DeserializeObject<Message>(messageJson.ToString());

			return message;
		}

		private Guid getParticipantId(string conversationId, Guid userId)
		{
			return conversationId
				.Split('_')
				.Select(id => Guid.Parse(id))
				.Except(new[] { userId })
				.Single();
		}

		private static string getUserConversationsListKey(Guid userId)
		{
			return $"user:{userId}:conversations";
		}

		private static string getConversationKey(string conversationId, string subkey)
		{
			return $"conversations:{conversationId}:{subkey}";
		}

		private static string getConversationId(Guid fromUser, Guid toUser)
		{
			return string.Join('_', new[] { fromUser, toUser }.Select(id => $"{id}").OrderBy(id => id));
		}
	}
}
