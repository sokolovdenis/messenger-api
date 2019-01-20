using Abstractions.DataSources;
using MessengerApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
			var id = this.GetCurrentUserId();

			var user = await _userDataSource.ReadAsync(id);

			return Ok(user);
		}
	}
}
