using Abstractions.Models;
using WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Services
{
	public class JwtAuthenticationService
	{
		public class Options
		{
			public string Secret { get; set; }

			public TimeSpan Expire { get; set; }
		}

		private readonly Options _options;

		public JwtAuthenticationService(IOptions<Options> options)
		{
			_options = options.Value;
		}

		public TokenResponse CreateTokenResponse(Identity identity)
		{
			Claim[] claims = {
				new Claim(ClaimTypes.NameIdentifier, identity.UserId.ToString())
			};

			SymmetricSecurityKey key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_options.Secret));
			SigningCredentials creds = new SigningCredentials(
				key, SecurityAlgorithms.HmacSha256);

			DateTime expires = DateTime.UtcNow + _options.Expire;

			JwtSecurityToken token = new JwtSecurityToken(
				claims: claims,
				expires: expires,
				signingCredentials: creds);

			return new TokenResponse()
			{
				Token = new JwtSecurityTokenHandler().WriteToken(token),
				Expires = expires
			};
		}

		public ClaimsPrincipal ValidateToken(string token, out SecurityToken validatedToken)
		{
			var handler = new JwtSecurityTokenHandler();
			var validationParameters = GetTokenValidationParameters(_options.Secret);

			try
			{
				return handler.ValidateToken(token, validationParameters, out validatedToken);
			}
			catch (SecurityTokenException)
			{
				validatedToken = null;
				return null;
			}
		}

		public static void AddJwtAuthentication(IServiceCollection services, string secret)
		{
			services
				.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(configureOptions =>
				{
					configureOptions.TokenValidationParameters = GetTokenValidationParameters(secret);
				});
		}

		private static TokenValidationParameters GetTokenValidationParameters(string secret)
		{
			var secretBytes = Encoding.UTF8.GetBytes(secret);

			return new TokenValidationParameters
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(secretBytes)
			};
		}
	}
}
