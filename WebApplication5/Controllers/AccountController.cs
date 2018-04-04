using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebApplication5.Models;
using WebApplication5.Providers;
using WebApplication5.Results;

namespace WebApplication5.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Authorize(Roles = "ADMIN")]
        [Route("AssignRole")]
        [HttpPost]
        public async Task<IHttpActionResult> AssignRole([FromBody] RoleUserModel user)
        {
            string email = user.Email;
            string roleToAssign = user.Role;

            if (roleToAssign == null)
            {
                return this.BadRequest("No roles are specified to this user.");
            }

            //find the user we want to assign roles to 
            var ExistingUser = await this.UserManager.FindByEmailAsync(email);
            if (ExistingUser == null)
            {
                return NotFound();
            }

            //check if the user currently has any roles 
            var currentRoles = this.UserManager.GetRoles(ExistingUser.Id);

            //remove user from current roles, if any 
            if (currentRoles.Count != 0)
            {
                IdentityResult removeResult = await this.UserManager.RemoveFromRolesAsync(ExistingUser.Id, currentRoles[0]);
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to remove user roles"); return BadRequest(ModelState);
                }
            }

            
            //assign user to the new roles 
            IdentityResult addResult = await this.UserManager.AddToRolesAsync(ExistingUser.Id, roleToAssign);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add user roles");
                return BadRequest(ModelState);
            }
            return Ok(new { userId = ExistingUser.Id, rolesAssigned = roleToAssign });
        }

        // GET api/Account/UserInfo
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public async Task<IHttpActionResult> GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            //check if the user currently has any roles 
            var currentRoles = this.UserManager.GetRoles(User.Identity.GetUserId());
            string Role = "";
            //remove user from current roles, if any 
            if (currentRoles.Count != 0)
            {
                Role = currentRoles[0];
            }

            return Ok(new {
                Email = User.Identity.GetUserName(),
                //HasRegistered = externalLogin == null,
                //LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null,
                Role = Role
            });
        }


        // POST api/Account/Register
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Authorize(Roles = "ADMIN")]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok("USer was successfully added");
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("Login")]
        public Task Login(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }



        #region Helpers

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest("Error occured");
                }

                return BadRequest(ModelState);
            }

            return BadRequest("Error occured");
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }
        #endregion
    }
}
