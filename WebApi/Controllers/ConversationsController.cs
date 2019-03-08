using Abstractions.DataSources;
using Abstractions.Models;
using WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
	[Authorize]
	[Route("api/conversations")]
	public class ConversationsController : ControllerBase
	{
		private readonly IConversationDataSource _dataSource;

		public ConversationsController(IConversationDataSource dataSource)
		{
			_dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
		}

		/// <summary>
		/// Get all conversation you participate in.
		/// </summary>
		/// <returns>Conversations.</returns>
		[ProducesResponseType(typeof(IEnumerable<Conversation>), 200)]
		[Route("")]
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var currentUserId = this.GetCurrentUserId();

			var dialogueConversationsTask = _dataSource.GetConversationsAsync(currentUserId);
			var publicConversationTask = _dataSource.GetPublicConversationAsync();

			await Task.WhenAll(dialogueConversationsTask, publicConversationTask);

			var conversations = new List<Conversation>();
			conversations.AddRange(dialogueConversationsTask.Result);
			conversations.Add(publicConversationTask.Result);

			return Ok(conversations);
		}

		/// <summary>
		/// Get messages from private conversation with specified user.
		/// </summary>
		/// <param name="userId">User ID.</param>
		/// <param name="pager">Pagination parameters.</param>
		/// <returns>Messages.</returns>
		[ProducesResponseType(typeof(IEnumerable<Message>), 200)]
		[Route("{userId}/messages")]
		[HttpGet]
		public async Task<IActionResult> Get(Guid userId, [FromQuery]GetMessagesPager pager)
		{
			var currentUserId = this.GetCurrentUserId();

			var messages = await _dataSource.GetMessagesAsync(currentUserId, userId, pager.From, pager.Count);

			return Ok(messages);
		}

		/// <summary>
		/// Get messages from public conversation.
		/// </summary>
		/// <param name="pager">Pagination parameters.</param>
		/// <returns>Messages.</returns>
		[Route("public/messages")]
		[HttpGet]
		public async Task<IActionResult> Get([FromQuery]GetMessagesPager pager)
		{
			var messages = await _dataSource.GetPublicMessagesAsync(pager.From, pager.Count);

			return Ok(messages);
		}

		/// <summary>
		/// Post message to private conversation with specified user.
		/// </summary>
		/// <param name="userId">User ID.</param>
		/// <param name="request">Request with message content.</param>
		/// <returns>Posted message.</returns>
		[ProducesResponseType(typeof(Message), 200)]
		[Route("{userId}/messages")]
		[HttpPost]
		public async Task<IActionResult> Post(Guid userId, [FromBody]PostMessageRequest request)
		{
			var currentUserId = this.GetCurrentUserId();

			var message = await _dataSource.PostMessageAsync(currentUserId, userId, request.Content);

			return Ok(message);
		}

		/// <summary>
		/// Post message to public conversation.
		/// </summary>
		/// <param name="request">Request with message content.</param>
		/// <returns>Posted message.</returns>
		[ProducesResponseType(typeof(Message), 200)]
		[Route("public/messages")]
		[HttpPost]
		public async Task<IActionResult> Post([FromBody]PostMessageRequest request)
		{
			var currentUserId = this.GetCurrentUserId();

			var message = await _dataSource.PostPublicMessageAsync(currentUserId, request.Content);

			return Ok(message);
		}
	}
}
