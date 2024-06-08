using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace technicalResolution.Attributes
{
    public class AuthorizeRoleAttribute : TypeFilterAttribute
    {
        public AuthorizeRoleAttribute(params string[] roles) : base(typeof(AuthorizeRoleFilter))
        {
            Arguments = new object[] { roles };
        }
    }

    public class AuthorizeRoleFilter : IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeRoleFilter(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);  // Assuming "Index" action in "Login" controller
                return;
            }

            var userRole = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (userRole == null || !_roles.Contains(userRole))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);  // Ensure this route exists or modify as needed
            }
        }
    }
}


