#region USING

using Shan.Authentication.API.Common;
using Shan.Authentication.API.Entities;
using Microsoft.AspNetCore.Mvc.Filters;

#endregion USING


namespace Shan.Authentication.API.Web.ApiHelper
{
    /// <summary>
    /// Custom exception filter which handles the exception occurred in the application.    
    /// </summary>
    public class ExceptionAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Method to handle un-handled exception
        /// </summary>
        /// <param name="context">The context for the action</param>
        public override void OnException(ExceptionContext context)
        {
            // Log the handle un-handled exception
            DiagnosticLogManager.TrackException(context.Exception);

            // Log the trace information
            DiagnosticLogManager.TrackTrace(Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error, context.HttpContext,
                                            new ApiResponse() { ResponseCode = DataConstant.Authentication003, ResponseMessage = ApiResponseHelper.GetResponseMessage((DataConstant.Authentication003)) });

            // Return system/service failure response with respective error code
            context.Result = ApiResponseHelper.GetApiResponse(DataConstant.Authentication003);
        }
    }
}