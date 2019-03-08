using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
	public class SignUpRequest
	{
		/// <summary>
		/// Login.
		/// </summary>
		[Required]
		[MinLength(2)]
		[MaxLength(50)]
		public string Login { get; set; }

		/// <summary>
		/// Password.
		/// </summary>
		[Required]
		[MinLength(2)]
		[MaxLength(50)]
		public string Password { get; set; }

		/// <summary>
		/// Name.
		/// </summary>
		[Required]
		[MinLength(2)]
		[MaxLength(50)]
		public string Name { get; set; }
	}
}
