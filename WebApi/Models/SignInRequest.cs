using System.ComponentModel.DataAnnotations;

namespace MessengerApi.Models
{
	public class SignInRequest
	{
		[Required]
		public string Login { get; set; }

		[Required]
		public string Password { get; set; }
	}
}
