#region USING

using Shan.Authentication.API.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

#endregion USING

namespace Shan.Authentication.API.Web.ApiHelper
{
    // Extension methods for AzureAdB2C access 
    public static class AzureAdB2CAuthenticationBuilderExtensions
    {
        /// <summary>
        /// Adds AzureAdB2C configurations to the AuthenticationBuilder
        /// </summary>
        /// <param name="builder"> The AuthenticationBuilder that needs to get updated with B2C configurations </param>
        /// <param name="configureOptions"> AzureAdB2C configurations to be applied to the AuthenticationBuilder </param>
        /// <returns> The updated AuthenticationBuilder with B2C configurations </returns>
        public static AuthenticationBuilder AddAzureAdB2C(this AuthenticationBuilder builder, Action<AzureAdB2C> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsSetup>();
            builder.AddOpenIdConnect();
            return builder;
        }

        /// <summary>
        /// Setup OpenIdConnect options
        /// </summary>
        public class OpenIdConnectOptionsSetup : IConfigureNamedOptions<OpenIdConnectOptions>
        {

            /// <summary>
            /// Constructon that copies the values of B2C options from configuration
            /// </summary>
            /// <param name="b2cOptions"> B2C options loaded from configuration </param>
            public OpenIdConnectOptionsSetup(IOptions<AzureAdB2C> b2cOptions)
            {
                AzureAdB2C = b2cOptions.Value;
            }

            // Public property to hold the B2C configurations
            public AzureAdB2C AzureAdB2C { get; set; }

            /// <summary>
            /// Configure the OpenIdConnect options with the configured B2C values
            /// </summary>
            /// <param name="name"> "OpenIdConnect" received implicitly by the base class </param>
            /// <param name="options"> OpenIdConnect optiosn that is getting updated with B2C values </param>
            public void Configure(string name, OpenIdConnectOptions options)
            {
                // Set the OpenIdConnect options
                options.ClientId = AzureAdB2C.ClientId;
                options.Authority = AzureAdB2C.Authority;
                options.UseTokenLifetime = true;
                options.TokenValidationParameters = new TokenValidationParameters() { NameClaimType = DataConstant.Name };
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.CallbackPath = AzureAdB2C.RedirectUri;

                // Register the event handlers to capture OpenIdConnect events
                options.Events = new OpenIdConnectEvents()
                {
                    //OnRedirectToIdentityProvider = OnRedirectToIdentityProvider,
                    OnRemoteFailure = OnRemoteFailure,
                    OnAuthorizationCodeReceived = OnAuthorizationCodeReceived
                };
            }

            /// <summary>
            /// Configure the OpenIdConnect options with the configured B2C values
            /// This is overloaded constructor that is expected by the base class OpenIdConnectOptions  
            /// </summary>            
            /// <param name="options"> OpenIdConnect optiosn that is getting updated with B2C values </param>
            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }

            /// <summary>
            /// Method to handle the remote failures if any from authentication server
            /// </summary>
            /// <param name="context"> The RemoteFailureContext that contains the failure error message </param>
            /// <returns> Throw the exception with the received failure message from remote </returns>
            public Task OnRemoteFailure(RemoteFailureContext context)
            {
                context.HandleResponse();

                // Throw the exception to log the failure message in ApplicationInsights AND respond user with system failure message
                // This is further handled by ExceptionAttribute class
                throw (new Exception(context.Failure.Message));
            }

            /// <summary>
            /// Method to handle the event upon successful login and authorization code is received
            /// This method further acquires the access token and store it in the user context in order to get it accessed by the login action
            /// </summary>
            /// <param name="context"> The context that contains authorization code received from Azure AD B2C </param>
            /// <returns></returns>
            public async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
            {
                // Use MSAL to swap the code for an access token
                // Extract the code from the response notification
                var code = context.ProtocolMessage.Code;

                // Get the signed in user's Object ID
                string signedInUserID = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

                // Creating client application withe the required configuration to request the token
                IConfidentialClientApplication cca = ConfidentialClientApplicationBuilder.Create(AzureAdB2C.ClientId)
                    .WithB2CAuthority(AzureAdB2C.Authority)
                    .WithClientSecret(AzureAdB2C.ClientSecret)
                    .Build();

                // Static cache that sign-in flow uses on transit intermediately on callback
                new MSALStaticCache(signedInUserID, context.HttpContext).EnablePersistence(cca.UserTokenCache);

                try
                {
                    // Acquire the token based on authorization code received
                    AuthenticationResult result = await cca.AcquireTokenByAuthorizationCode(AzureAdB2C.ApiScopes.Split(DataConstant.Space), code)
                        .ExecuteAsync();

                    // Setting the context that the user is signed-in
                    context.HandleCodeRedemption(result.AccessToken, result.IdToken);

                    // Store the received Id Token to make it available for the controller to return to the request
                    context.HttpContext.Session.SetString(DataConstant.IdToken, result.IdToken);

                }
                catch (Exception ex)
                {
                    // Throw the exception to log the failure message in ApplicationInsights AND respond user with system failure message
                    // This is further handled by ExceptionAttribute class
                    throw ex;
                }
            }
        }
    }
}
