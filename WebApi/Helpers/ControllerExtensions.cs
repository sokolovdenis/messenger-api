using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace WebApi.Helpers
{
	public static class ControllerExtensions
	{
		public static Guid GetCurrentUserId(this ControllerBase controller)
		{
			return controller.User.GetCurrentUserId();
		}

		public static Guid GetCurrentUserId(this HttpContext context)
		{
			return context.User.GetCurrentUserId();
		}

		public static Guid GetCurrentUserId(this ClaimsPrincipal claimsPrincipal)
		{
			return Guid.Parse(claimsPrincipal.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
		}
	}
}
