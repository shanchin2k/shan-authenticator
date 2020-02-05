#region USING

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

#endregion USING

namespace Shan.Authentication.API.Common
{
    /// <summary>
    /// Class contains the common helper methods 
    /// </summary>
    public class CommonHelper
    {
        /// <summary>
        /// IConfiguration to get configuration from service in order to access app settings/response message added in json file. 
        /// </summary>
        internal static IConfiguration Configuration { get; private set; }

        /// <summary>
        /// Common Helper constructor
        /// </summary>
        /// <param name="configuration"> Configuration</param>
        public CommonHelper(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// To read the key value from Resource file(authenticationresources.json) 
        /// </summary>
        /// <param name="key"> Key name of the message</param>
        /// <returns> Message value for the Key</returns>
        public static string ReadResourceValue(string key)
        {
            if (Configuration != null)
            {
                var resourceObject = Configuration.GetSection(DataConstant.ResponseMessage)[key];

                // If resourceObject value is null then return the Key name 
                if (resourceObject == null)
                {
                    return key;
                }
                // Return the resource key value 
                else
                {
                    return resourceObject.ToString();
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// To Serialize an object to JSON
        /// </summary>
        /// <param name="objectToSerialize"> Object to be serialized</param>
        /// <returns>Serialized object in JSON format</returns>
        public static object ConvertObjectToJsonString(object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize);
        }
    }
}
