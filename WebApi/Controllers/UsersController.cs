using Abstractions.DataSources;
using Abstractions.Models;
using WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
	[Authorize]
	[Route("api/users")]
	public class UsersController : ControllerBase
	{
		private readonly IUserDataSource _userDataSource;

		public UsersController(IUserDataSource userDataSource)
		{
			_userDataSource = userDataSource;
		}

		/// <summary>
		/// Get self user.
		/// </summary>
		/// <returns>User.</returns>
		[ProducesResponseType(typeof(User), 200)]
		[Route("me")]
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var id = this.GetCurrentUserId();

			var user = await _userDataSource.ReadAsync(id);

			return Ok(user);
		}

		/// <summary>
		/// Get user by ID.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <returns>User.</returns>
		[ProducesResponseType(typeof(User), 200)]
		[ProducesResponseType(404)]
		[Route("{id}")]
		[HttpGet]
		public async Task<IActionResult> Get(Guid id)
		{
			var user = await _userDataSource.ReadAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			return Ok(user);
		}

		/// <summary>
		/// Find user by name.
		/// </summary>
		/// <param name="query">Name or a part of name.</param>
		/// <returns></returns>
		[ProducesResponseType(typeof(IEnumerable<User>), 200)]
		[ProducesResponseType(400)]
		[Route("")]
		[HttpGet]
		public async Task<IActionResult> Get([Required]string query)
		{
			var users = await _userDataSource.SearchAsync(query);

			return Ok(users);
		}
	}
}
