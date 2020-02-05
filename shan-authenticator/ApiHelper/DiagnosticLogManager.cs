#region USING

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Shan.Authentication.API.Common;
using Shan.Authentication.API.Entities;
using System;
using System.Collections.Generic;

#endregion USING

namespace Shan.Authentication.API.Web.ApiHelper
{
    /// <summary>
    /// This class will be containing the implementation of diagnostic logging 
    /// </summary>
    public static class DiagnosticLogManager
    {
        // Create new TelemetryClient object which will be used diagnostic logging.
        private static readonly TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
        private static readonly TelemetryClient telemetryClient = new TelemetryClient(configuration);

        /// <summary>
        /// Method to log the un-handled exception
        /// </summary>
        /// <param name="exception">Unhandled exception object</param>
        public static void TrackException(Exception exception)
        {
            // Log the exception in Application insights.
            telemetryClient.TrackException(exception);
        }

        /// <summary>
        /// Method to log the trace information of the un-handled exception in the ApiResponse object
        /// </summary>
        /// <param name="severityLevel"> Severity level of the trace</param>
        /// <param name="actionContext"> User request action from which the user request details being read for logging purpose</param>
        /// <param name="apiResponse"> ApiResponse object</param>
        public static void TrackTrace(Microsoft.ApplicationInsights.DataContracts.SeverityLevel severityLevel, HttpContext actionContext, ApiResponse apiResponse)
        {
            // Create a new Dictionary object to store the trace properties to be logged
            var traceProperties = new Dictionary<string, string>
            {
                // Requested HTTP method type
                { DataConstant.HTTPRequestType, actionContext.Request.Method },
                
                // Requested service/controller name
                { DataConstant.Controller, actionContext.Request.Path },
               
                // Exception occurred time
                { DataConstant.ExceptionOccurredTime, DateTime.Now.ToString() }
            };

            // log the error details from the DB as well. 
            TrackTrace(string.Concat(apiResponse.ResponseCode, DataConstant.Hyphen, apiResponse.ResponseMessage),
                   severityLevel, traceProperties);
        }

        /// <summary>
        /// Method to log the trace information of the un-handled exception
        /// </summary>
        /// <param name="logMessage"> Message to be logged</param>
        /// <param name="severityLevel"> Severity level of the trace</param>
        /// <param name="logProperties"> Properties to be logged</param>
        public static void TrackTrace(string logMessage, Microsoft.ApplicationInsights.DataContracts.SeverityLevel severityLevel, IDictionary<String, String> logProperties)
        {
            // Check the severityLevel of the log is greater than or equal to the configured minimumLoggingSeverityLevel in Azure app settings
            // If yes, log the information
            if ((int)severityLevel >= Convert.ToInt32(Common.ConfigHelper.appSettings.ApplicationInsights.MinimumLoggingSeverityLevel))
            {
                telemetryClient.TrackTrace(logMessage, severityLevel, logProperties);
            }
        }
    }
}
