
namespace Shan.Authentication.API.Common
{
    /// <summary>
    /// Class contains Application settings keys
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Appsettings for ApplicationInsights
        /// </summary>
        public ApplicationInsights ApplicationInsights { get; set; }

        /// <summary>
        /// AzureAdB2C settings
        /// </summary>
        public AzureAdB2C AzureAdB2C { get; set; }
    }

    /// <summary>
    /// Class for ApplicationInsights keys
    /// </summary>
    public class ApplicationInsights
    {
        /// <summary>
        /// Minimum Logging Severity Level
        /// </summary>
        public string MinimumLoggingSeverityLevel { get; set; }
    }

    /// <summary>
    /// Options for AzureAdB2C
    /// </summary>
    public class AzureAdB2C
    {
        /// <summary>
        /// AzureAD B2C Instance for B2C directory
        /// Usually in the form of 'https://[B2CDirectoryName].b2clogin.com/tfp'
        /// </summary>        
        public string AadInstance { get; set; }

        /// <summary>
        /// Tenant name of the B2C directory
        /// Usually in the form of '[B2CDirectoryName].onmicrosoft.com'
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Client Id for B2C directory
        /// Usually in the form of GUID
        /// </summary>        
        public string ClientId { get; set; }

        /// <summary>
        /// SignIn policy name of B2C directory
        /// Usually in the form of 'B2C_1_[PolicyName]
        /// </summary>
        public string SignInPolicyId { get; set; }

        /// <summary>
        /// ReplyUrl path registered in the B2C application settings 
        /// This can be any url registered under B2C application settings. 
        /// The purpose of this url is to validate whether the authentication request brings the Callback path as registered in B2C
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Authority of B2C
        /// </summary>
        public string Authority => $"{AadInstance}/{TenantId}/{SignInPolicyId}/v2.0";

        /// <summary>
        /// Client secret to get acquire access token
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// API scopes defined in B2C app registration
        /// Usually in the form of 'https://[B2CDirectoryName].onmicrosoft.com/[AppName]/read_access'
        /// If there are different scopes specified, those will be mentioned with space 
        /// </summary>        
        public string ApiScopes { get; set; }
    }
}
