using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
	public class PostMessageRequest
	{
		/// <summary>
		/// Message content.
		/// </summary>
		[Required]
		public string Content { get; set; }
	}
}
