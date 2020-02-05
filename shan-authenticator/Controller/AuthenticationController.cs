#region USING

using Shan.Authentication.API.Common;
using Shan.Authentication.API.Web.ApiHelper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

#endregion USING


namespace Shan.Authentication.API.Web.Controllers
{
    /// <summary>
    /// Controller which handles all the process related to authentication
    /// </summary>

    [Route("authenticationapi/Authentication")]
    public class AuthenticationController : ControllerBase
    {
        // Public property to hold the B2C configurations
        public AzureAdB2C AzureAdB2C { get; set; }

        /// <summary>
        /// Constructon that copies the values of B2C options from configuration
        /// </summary>
        /// <param name="b2cOptions"> B2C options loaded from configuration </param>
        public AuthenticationController(IOptions<AzureAdB2C> b2cOptions)
        {
            AzureAdB2C = b2cOptions.Value;
        }

        /// <summary>
        /// Request token from B2C
        /// During sign-in flow, this action is called second time when the user is getting authenticated 
        /// </summary>
        /// <returns> 
        ///     Json response that contains Security token in responsMessage if authentication is success.
        ///     Json reponse message with defined user friendly error message in responseMessage in case of failures if any
        /// </returns>
        [HttpGet]
        [Route(DataConstant.SignIn)]
        public IActionResult AcquireToken()
        {
            // Return the authentication token if the user is authenticated. 
            if (HttpContext.User.Identity.IsAuthenticated && !string.IsNullOrWhiteSpace(HttpContext.Session.GetString(DataConstant.IdToken)))
            {
                return ApiResponseHelper.GetApiResponse(DataConstant.Authentication201, HttpContext.Session.GetString(DataConstant.IdToken));
            }
            else
            {
                // Redirect to authentication flow if the user is not signed-in 
                //var redirectUrl = AzureAdB2C.VirtualDirectory + DataConstant.SignInPath;
                var redirectUrl = Request.Host + Request.PathBase + Request.Path;
                return Challenge(
                    new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = redirectUrl },
                    OpenIdConnectDefaults.AuthenticationScheme);
            }
        }

        /// <summary>
        /// Signing out from the current user context        
        /// </summary>
        [HttpGet]
        [Route(DataConstant.SignOut)]
        public IActionResult SignOut()
        {
            // Initiate the SignOut flow if the user is authenticated 
            if (User.Identity.IsAuthenticated)
            {
                //var callbackUrl = AzureAdB2C.VirtualDirectory + DataConstant.SignOutPath;
                var callbackUrl = Request.Host + Request.PathBase + Request.Path;
                return SignOut(new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = callbackUrl },
                    CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
            }
            else
            {
                // If the user is signed out already, reply with the relevant success message
                return ApiResponseHelper.GetApiResponse(DataConstant.Authentication202);
            }
        }
    }
}
