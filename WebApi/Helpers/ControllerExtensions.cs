using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace MessengerApi.Helpers
{
	public static class ControllerExtensions
	{
		public static Guid GetCurrentUserId(this ControllerBase controller)
		{
			return Guid.Parse(controller.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
		}
	}
}
