using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace RizenSoftApiV2.Controllers
{
    public class BaseApiController : ControllerBase
	{
        protected int UserID => int.Parse(FindClaim(ClaimTypes.NameIdentifier));

        private string FindClaim(string claimName)
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var claim = claimsIdentity.FindFirst(claimName);

                if (claim == null)
                    return string.Empty;

                return claim.Value;
            }
            else
                return string.Empty;
        }
    }
}

