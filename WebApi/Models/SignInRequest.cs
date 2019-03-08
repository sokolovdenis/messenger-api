using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
	public class SignInRequest
	{
		/// <summary>
		/// Login.
		/// </summary>
		[Required]
		public string Login { get; set; }

		/// <summary>
		/// Password.
		/// </summary>
		[Required]
		public string Password { get; set; }
	}
}
