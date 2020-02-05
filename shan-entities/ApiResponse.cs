#region USING

using System.Net;

#endregion USING

namespace Shan.Authentication.API.Entities
{
    /// <summary>
    /// Entity contains basic ApiResponse properties, this entity will be returned in Api response
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Response StatusCode
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// ResponseCode contains the success/error code
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Response Error Message 
        /// </summary>
        public string ResponseMessage { get; set; }
    }
}
