using System.ComponentModel.DataAnnotations;

namespace MessengerApi.Models
{
	public class SignUpRequest
	{
		[Required]
		[MinLength(2)]
		[MaxLength(50)]
		public string Login { get; set; }

		[Required]
		[MinLength(2)]
		[MaxLength(50)]
		public string Password { get; set; }

		[Required]
		[MinLength(2)]
		[MaxLength(50)]
		public string Name { get; set; }
	}
}
