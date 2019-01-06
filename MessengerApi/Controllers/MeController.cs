using MessengerApi.DataSources.Contracts;
using MessengerApi.Helpers;
using MessengerApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MessengerApi.Controllers
{
	[Authorize]
	public class MeController : ControllerBase
	{
		private readonly IUserDataSource _userDataSource;

		public MeController(IUserDataSource userDataSource)
		{
			_userDataSource = userDataSource;
		}

		[Route("api/me")]
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			Guid id = this.GetCurrentUserId();

			User user = await _userDataSource.Read(id);

			return Ok(user);
		}
	}
}
