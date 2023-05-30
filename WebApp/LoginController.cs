using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace WebApp
{
    [Route("api")]
    public class LoginController : Controller
    {
        private readonly IAccountDatabase _db;

        public LoginController(IAccountDatabase db)
        {
            _db = db;
        }

        [HttpPost("sign-in/{userName}")]
        public async Task<IActionResult> Login(string userName)
        {
            var account = await _db.FindByUserNameAsync(userName);
            if (account != null)
            {
                //TODO 1: Generate auth cookie for user 'userName' with external id
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, account.ExternalId),
                    new Claim(ClaimTypes.Name, account.UserName),
                    new Claim(ClaimTypes.Role, account.Role),
                };
                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(new ClaimsPrincipal(identity));
                return Ok();
            }
            //TODO 2: return 404 if user not found
            return NotFound();
        }
    }
}