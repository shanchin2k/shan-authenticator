#region USING

using Shan.Authentication.API.Common;
using Shan.Authentication.API.Web.ApiHelper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.Extensions;

#endregion USING


namespace Shan.Authentication.API.Web.Controllers
{
    /// <summary>
    /// Controller which handles all the process related to authentication
    /// </summary>

    [Route("authenticationapi/Authentication")]
    public class AuthenticationController : ControllerBase
    {        

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
                var redirectUrl = Request.GetDisplayUrl();
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
                var redirectUrl = Request.GetDisplayUrl();
                return SignOut(new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = redirectUrl },
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
