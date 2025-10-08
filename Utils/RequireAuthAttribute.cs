using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace EcbMartService.Utils
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireAuthAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Authentication required" });
                return;
            }

            // Extract and stash the bearer token for downstream consumers
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader?.Split(' ').Last();
            if (!string.IsNullOrWhiteSpace(token))
            {
                context.HttpContext.Items["JwtToken"] = token;
            }
        }
    }
} 