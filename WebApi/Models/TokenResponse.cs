using System;

namespace WebApi.Models
{
	/// <summary>
	/// Token Response.
	/// </summary>
	public class TokenResponse
	{
		/// <summary>
		/// Authorization Token you should use as Bearer in request headers.
		/// </summary>
		public string Token { get; set; }

		/// <summary>
		/// Expiration timestamp.
		/// </summary>
		public DateTime Expires { get; set; }
	}
}
