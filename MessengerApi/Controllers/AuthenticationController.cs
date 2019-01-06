using MessengerApi.DataSources.Contracts;
using MessengerApi.Models;
using MessengerApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MessengerApi.Controllers
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
			Guid id = _identityService.GenerateIdentityHash(request.Login);

			_identityService.GeneratePasswordHashAndSalt(
				request.Password, out byte[] hash, out byte[] salt);

			User user = await _userDataSource.Create(Guid.NewGuid(), request.Name);

			Identity identity = await _identityDataSource.Create(id, user.Id, salt, hash);
			if (identity == null)
			{
				// delete user
				return StatusCode((int)HttpStatusCode.Conflict, "Login is already in use.");
			}

			TokenResponse tokenResponse = _jwtAuthenticationService.CreateTokenResponse(identity);

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
			Guid id = _identityService.GenerateIdentityHash(request.Login);

			Identity identity = await _identityDataSource.Read(id);

			if (identity == null ||
				!_identityService.IsPasswordValid(request.Password, identity.Hash, identity.Salt))
			{
				return BadRequest("Wrong login or password.");
			}

			TokenResponse tokenResponse = _jwtAuthenticationService.CreateTokenResponse(identity);

			return Ok(tokenResponse);
		}
	}
}
