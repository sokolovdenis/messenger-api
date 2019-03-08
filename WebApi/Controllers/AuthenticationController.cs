using Abstractions.DataSources;
using WebApi.Models;
using WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
	[Route("api/authentication")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		IdentityService _identityService;
		JwtAuthenticationService _jwtAuthenticationService;
		IIdentityDataSource _identityDataSource;
		IUserDataSource _userDataSource;

		public AuthenticationController(
			IdentityService identityService, 
			JwtAuthenticationService jwtAuthenticationService, 
			IIdentityDataSource identityDataSource, 
			IUserDataSource userDataSource)
		{
			_identityService = identityService;
			_jwtAuthenticationService = jwtAuthenticationService;
			_identityDataSource = identityDataSource;
			_userDataSource = userDataSource;
		}

		/// <summary>
		/// Sign up and get API token.
		/// </summary>
		/// <param name="request">Sign Up Request.</param>
		[HttpPost]
		[Route("signup")]
		[ProducesResponseType(typeof(TokenResponse), 200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
		{
			var id = _identityService.GenerateIdentityHash(request.Login);

			_identityService.GeneratePasswordHashAndSalt(
				request.Password, out byte[] hash, out byte[] salt);

			var user = await _userDataSource.CreateAsync(request.Name);

			var identity = await _identityDataSource.CreateAsync(id, user.Id, salt, hash);
			if (identity == null)
			{
				// delete user
				return StatusCode((int)HttpStatusCode.Conflict, "Login is already in use.");
			}

			var tokenResponse = _jwtAuthenticationService.CreateTokenResponse(identity);

			return Ok(tokenResponse);
		}

		/// <summary>
		/// Sign in and get API token.
		/// </summary>
		/// <param name="request">Sign In Request.</param>
		[HttpPost]
		[Route("signin")]
		[ProducesResponseType(typeof(TokenResponse), 200)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
		{
			var id = _identityService.GenerateIdentityHash(request.Login);

			var identity = await _identityDataSource.ReadAsync(id);

			if (identity == null ||
				!_identityService.IsPasswordValid(request.Password, identity.Hash, identity.Salt))
			{
				return BadRequest("Wrong login or password.");
			}

			var tokenResponse = _jwtAuthenticationService.CreateTokenResponse(identity);

			return Ok(tokenResponse);
		}
	}
}
