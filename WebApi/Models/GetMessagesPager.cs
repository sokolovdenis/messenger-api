using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
	public class GetMessagesPager
	{
		/// <summary>
		/// First message to return.
		/// </summary>
		[Required]
		public long From { get; set; }

		/// <summary>
		/// Number of messages to return.
		/// </summary>
		[Required]
		public long Count { get; set; }
	}
}
