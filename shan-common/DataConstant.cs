namespace Shan.Authentication.API.Common
{
    /// <summary>
    /// Contains the constant value to be used for the whole application
    /// </summary>
    public static class DataConstant
    {

        #region MEDIA_TYPE

        /// <summary>
        /// Media type "application/json"
        /// </summary>
        public const string MediaTypeHeaderApplicationJson = "application/json";

        #endregion MEDIA_TYPE

        #region ROUTING_NAME

        /// <summary>
        /// Routing name for SignIn
        /// </summary>
        public const string SignIn = "SignIn";

        /// <summary>
        /// Routing name for SignOut
        /// </summary>
        public const string SignOut = "SignOut";

        #endregion ROUTING_NAME

        #region RESPONSE_CODE

        /// <summary>
        /// Success ResponseCode for successful authentication.
        /// </summary>
        public const string Authentication201 = "AUTHENTICATION_201";

        /// <summary>
        /// Success ResponseCode for successful SignOut
        /// </summary>
        public const string Authentication202 = "AUTHENTICATION_202";

        /// <summary>
        /// ErrorCode for "system/server failure" during authentication.
        /// </summary>
        public const string Authentication003 = "AUTHENTICATION_003";

        #endregion RESPONSE_CODE

        #region ERROR_LOG_DETAILS

        /// <summary>
        /// Constant for "HTTP request type"
        /// </summary>
        public const string HTTPRequestType = "HTTP request type";

        /// <summary>
        /// Constant for "Controller"
        /// </summary>
        public const string Controller = "Controller";

        /// <summary>
        /// Constant for "Exception occurred time"
        /// </summary>
        public const string ExceptionOccurredTime = "Exception occurred time";

        /// <summary>
        /// Response message to be returned for system startup exception
        /// </summary>
        public const string SystemStartupExceptionMessage = "The request cannot be processed at the moment.";

        #endregion ERROR_LOG_DETAILS

        #region WHITE_LIST_HTTP_METHODS

        /// <summary>
        /// Http GET method
        /// </summary>
        public const string HttpGET = "GET";

        #endregion WHITE_LIST_HTTP_METHODS

        #region APP_SETTINGS_AND_B2C_SETTINGS

        /// <summary>
        /// AppSettings
        /// </summary>
        public const string Appsettings = "AppSettings";

        /// <summary>
        /// Azure Active Directory
        /// </summary>
        public const string ApplicationInsights = "ApplicationInsights";

        /// <summary>
        /// Resource file for response messages
        /// </summary>
        public const string ResponseMessage = "ResponseMessage";

        /// <summary>
        /// Resource file for response messages
        /// </summary>
        public const string Name = "Name";

        /// <summary>
        /// The root node in appsettings.json where B2C configurations are stored
        /// </summary>
        public const string AppSettingsAzureAdB2C = "AppSettings:AzureAdB2C";

        #endregion APP_SETTINGS

        #region FILE_NAMES

        /// <summary>
        /// Appsettings file name
        /// </summary>
        public const string AppSettingsFileName = "appsettings.json";

        /// <summary>
        /// Resource file name
        /// </summary>
        public const string ResourceFileName = @"App_GlobalResources\authenticationresources.json";

        #endregion FILE_NAMES       

        #region SPECIAL_CHARACTERS

        /// <summary>
        /// Hyphen
        /// </summary>
        public const char Hyphen = '-';

        /// <summary>
        /// Space
        /// </summary>
        public const char Space = ' ';

        #endregion SPECIAL_CHARACTERS

        #region SESSION_HANDLING

        /// <summary>
        /// Name keyword for IdToken
        /// </summary>
        public const string IdToken = "IdToken";

        /// <summary>
        /// Constant to represent the name of the cache
        /// </summary>
        public const string TokenCache = "_TokenCache";

        #endregion SESSION_HANDLING
    }
}
