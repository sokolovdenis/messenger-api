using MessengerApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MessengerApi.Services
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

		public static void AddJwtAuthentication(IServiceCollection services, string secret)
		{
			byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

			services
				.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(configureOptions =>
				{
					configureOptions.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateAudience = false,
						ValidateIssuer = false,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(secretBytes)
					};
				});
		}
	}
}
