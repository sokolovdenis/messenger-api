using Abstractions.DataSources;
using Abstractions.Models;
using MessengerApi.Helpers;
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

		[Route("{userId}/messages")]
		[HttpGet]
		public async Task<IActionResult> Get(Guid userId, [FromQuery]GetMessagesPager pager)
		{
			var currentUserId = this.GetCurrentUserId();

			var messages = await _dataSource.GetMessagesAsync(currentUserId, userId, pager.From, pager.Count);

			return Ok(messages);
		}

		[Route("public/messages")]
		[HttpGet]
		public async Task<IActionResult> Get([FromQuery]GetMessagesPager pager)
		{
			var messages = await _dataSource.GetPublicMessagesAsync(pager.From, pager.Count);

			return Ok(messages);
		}

		[Route("{userId}/messages")]
		[HttpPost]
		public async Task<IActionResult> Post(Guid userId, PostMessageRequest request)
		{
			var currentUserId = this.GetCurrentUserId();

			var message = await _dataSource.PostMessageAsync(currentUserId, userId, request.Content);

			return Ok(message);
		}

		[Route("public/messages")]
		[HttpPost]
		public async Task<IActionResult> Post(PostMessageRequest request)
		{
			var currentUserId = this.GetCurrentUserId();

			var message = await _dataSource.PostPublicMessageAsync(currentUserId, request.Content);

			return Ok(message);
		}
	}
}
