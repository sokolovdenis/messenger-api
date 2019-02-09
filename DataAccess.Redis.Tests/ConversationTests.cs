using Abstractions.DataSources;
using Abstractions.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Redis.Tests
{
	[TestClass]
	public class ConversationTests
	{
		private static RedisConnection _connection;
		private static IConversationDataSource _dataSource;

		[ClassInitialize]
		public static void Init(TestContext ctx)
		{
			_connection = new RedisConnection(Options.Create(new RedisConnection.Options()
			{
				Address = "localhost:6379",
				Database = 777
			}));
			_dataSource = new ConversationDataSource(_connection);
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			_connection?.Dispose(true);
		}

		[TestMethod]
		public async Task PostMessage()
		{
			var user1 = Guid.NewGuid();
			var user2 = Guid.NewGuid();
			var content = "Hello! ;-)";

			var msg = await _dataSource.PostMessageAsync(user1, user2, content);

			Assert.AreEqual(user1, msg.User);
			Assert.AreEqual(content, msg.Content);
		}

		[TestMethod]
		public async Task GetConversations()
		{
			var user1 = Guid.NewGuid();
			var user2 = Guid.NewGuid();
			var user3 = Guid.NewGuid();

			await _dataSource.PostMessageAsync(user1, user2, $"{user1} {user2}");
			await _dataSource.PostMessageAsync(user3, user1, $"{user3} {user1}");

			var conversations1 = await _dataSource.GetConversationsAsync(user1);
			var conversations2 = await _dataSource.GetConversationsAsync(user2);
			var conversations3 = await _dataSource.GetConversationsAsync(user3);

			Assert.AreEqual(2, conversations1.Count);
			Assert.IsTrue(conversations1.Any(c => c.Participant == user2));
			Assert.IsTrue(conversations1.Any(c => c.Participant == user3));

			Assert.AreEqual(1, conversations2.Count);
			Assert.AreEqual(user1, conversations2.Single().Participant);

			Assert.AreEqual(1, conversations3.Count);
			Assert.AreEqual(user1, conversations3.Single().Participant);
		}

		[TestMethod]
		public async Task GetMessages()
		{
			var user1 = Guid.NewGuid();
			var user2 = Guid.NewGuid();

			for (var i = 0; i < 100; i++)
			{
				if (i % 2 == 0)
				{
					var msg = await _dataSource.PostMessageAsync(user1, user2, $"Msg {i}");
				} else
				{
					var msg = await _dataSource.PostMessageAsync(user2, user1, $"Msg {i}");
				}
			}

			var conversations1 = await _dataSource.GetConversationsAsync(user1);
			Assert.AreEqual(1, conversations1.Count);
			Assert.AreEqual(user2, conversations1.Single().Participant);

			var conversations2 = await _dataSource.GetConversationsAsync(user2);
			Assert.AreEqual(1, conversations2.Count);
			Assert.AreEqual(user1, conversations2.Single().Participant);

			var messagesFrom33To34 = await _dataSource.GetMessagesAsync(user1, user2, 33, 1);
			Assert.AreEqual(1, messagesFrom33To34.Count);
			var message = messagesFrom33To34.Single();
			Assert.AreEqual(33, message.Id);
			Assert.AreEqual("Msg 33", message.Content);

			var messagesFrom0To9 = await _dataSource.GetMessagesAsync(user2, user1, 0, 10);
			Assert.AreEqual(10, messagesFrom0To9.Count);

			var messagesFrom90To199 = await _dataSource.GetMessagesAsync(user2, user1, 90, 110);
			Assert.AreEqual(10, messagesFrom90To199.Count);
		}

		[TestMethod]
		public async Task PublicMessages()
		{
			var user = Guid.NewGuid();

			for (var i = 0; i < 10; i++)
			{
				var msg = await _dataSource.PostPublicMessageAsync(user, $"Msg {i}");
			}

			var conversation = await _dataSource.GetPublicConversationAsync();

			Assert.AreEqual("public", conversation.Id);
			Assert.IsNull(conversation.Participant);
			Assert.AreEqual(9, conversation.LastMessage.Id);
			Assert.AreEqual(user, conversation.LastMessage.User);
			Assert.AreEqual("Msg 9", conversation.LastMessage.Content);

			var messages = await _dataSource.GetPublicMessagesAsync(0, 5);
			Assert.AreEqual(5, messages.Count);
		}

		[TestMethod]
		public async Task PubSub()
		{
			_dataSource.OnMessage += _dataSource_ReceivedMessage;

			var user1 = Guid.NewGuid();
			var user2 = Guid.NewGuid();

			for (var i = 0; i < 100; i++)
			{
				if (i % 2 == 0)
				{
					var msg = await _dataSource.PostMessageAsync(user1, user2, $"Msg {i}");
				}
				else
				{
					var msg = await _dataSource.PostMessageAsync(user2, user1, $"Msg {i}");
				}
			}

			await Task.Delay(1000);

			Assert.AreEqual(100, _msgs.Count);

			_dataSource.OnMessage -= _dataSource_ReceivedMessage;
		}

		private void _dataSource_ReceivedMessage(object sender, MessageEventArgs e)
		{
			_msgs.Add(e.Message);
		}

		private List<Message> _msgs = new List<Message>();
	}
}
