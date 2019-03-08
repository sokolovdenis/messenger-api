using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Swagger
{
	public class AuthResponsesOperationFilter : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			IEnumerable<AuthorizeAttribute> authAttributes = context.ApiDescription
				.ControllerAttributes()
				.Union(context.ApiDescription.ActionAttributes())
				.OfType<AuthorizeAttribute>();

			if (authAttributes.Any())
			{
				operation.Responses.Add("401", new Response());
			}
		}
	}
}
