#region USING

using Shan.Authentication.API.Common;
using Shan.Authentication.API.Entities;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

#endregion USING

namespace Shan.Authentication.API.Web.ApiHelper
{
    /// <summary>
    /// Class contains the methods used to construct the Api response 
    /// </summary>
    public class ApiResponseHelper
    {
        /// <summary>
        /// Iconfiguration to get configuration from service in order to access app settings/response message added in json file. 
        /// </summary>
        internal static IConfiguration Configuration { get; private set; }

        /// <summary>
        /// Api Response Helper constructor
        /// </summary>
        /// <param name="configuration"> Configuration</param>
        public ApiResponseHelper(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// To generate Api response with the input response and responseMessage
        /// </summary>
        /// <param name="responseCode"> response code input</param>
        /// <param name="responseMessage"> response message</param>
        /// <returns> Json result which contains the ApiResponse object</returns>
        public static JsonResult GetApiResponse(string responseCode, string responseMessage = null)
        {
            ApiResponse apiResponse = new ApiResponse
            {
                // Set the reponseCode to ApiResponse.ResponseCode
                ResponseCode = responseCode,

                // Get the response message by using the response code
                ResponseMessage = string.Format(GetResponseMessage(responseCode), responseMessage),

                // Get the status code by using the response code
                StatusCode = GetStatusCodeByResponseCode(responseCode),
            };

            return new JsonResult(apiResponse)
            {
                ContentType = DataConstant.MediaTypeHeaderApplicationJson,
                StatusCode = (int)apiResponse.StatusCode
            };
        }

        /// <summary>
        /// To get response message from json file using response code.
        /// </summary>
        /// <param name="responseCode"> The response code</param>
        /// <param name="testConfigurations"> The IConfiguration to get response messages.</param>
        /// <returns> Response message of the given response code</returns>
        public static string GetResponseMessage(string responseCode, IConfiguration testConfigurations = null)
        {
            string responseMessage;

            // Get response messages from configuration from Api test project if existingConfiguration is null.
            if (Configuration == null)
            {
                responseMessage = testConfigurations.GetSection(DataConstant.ResponseMessage)[responseCode].ToString();
            }
            // Get response messages from existing configuration 
            else
            {
                responseMessage = Configuration.GetSection(DataConstant.ResponseMessage)[responseCode].ToString();
            }

            return responseMessage;
        }

        /// <summary>
        /// To get the response StatusCode by using ResponseCode
        /// </summary>
        /// <param name="reponseCode">ReponseCode contains the Success/Error code</param>
        /// <returns>HttpStatusCode for Api response</returns>
        private static HttpStatusCode GetStatusCodeByResponseCode(string reponseCode)
        {
            HttpStatusCode statusCode = new HttpStatusCode();

            // If reponseCode has value then get its corresponding StatusCode
            if (!string.IsNullOrWhiteSpace(reponseCode))
            {
                switch (reponseCode)
                {
                    case DataConstant.Authentication201:
                    case DataConstant.Authentication202:
                        statusCode = HttpStatusCode.OK;
                        break;
                    default:
                        statusCode = HttpStatusCode.InternalServerError;
                        break;
                }
            }
            return statusCode;
        }
    }
}