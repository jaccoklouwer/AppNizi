using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AppNiZiAPI.Services.Handlers
{
    public interface IResponseHandler
    {
        IActionResult ForgeResponse(Dictionary<ServiceDictionaryKey, object> dictionary);
    }

    public class ResponseHandler : IResponseHandler
    {
        /// <summary>
        /// Forges an API response based on a given dictionary and returns it.
        /// </summary>
        public IActionResult ForgeResponse(Dictionary<ServiceDictionaryKey, object> dictionary)
        {
            // Default if no errors caught
            IActionResult actionResult = dictionary.ContainsKey(ServiceDictionaryKey.VALUE)
            ? (ActionResult)new OkObjectResult(dictionary[ServiceDictionaryKey.VALUE])
            : new BadRequestResult();

            if (dictionary.ContainsKey(ServiceDictionaryKey.ERROR))
                actionResult = ForgeErrorResponse(dictionary);

            return actionResult;
        }

        /// <summary>
        /// Forges an API error response based on a given dictionary and returns it. 
        /// Make sure you check if the dictionary contains an error first.
        /// </summary>
        private IActionResult ForgeErrorResponse(Dictionary<ServiceDictionaryKey, object> dictionary)
        {
            IActionResult actionResult = new BadRequestObjectResult(dictionary[ServiceDictionaryKey.ERROR]);

            // HttpStatusCode provided, so we handle that too
            if (dictionary.ContainsKey(ServiceDictionaryKey.HTTPSTATUSCODE))
            {
                switch (dictionary[ServiceDictionaryKey.HTTPSTATUSCODE])
                {
                    case HttpStatusCode.NotFound:
                        actionResult = new NotFoundObjectResult(dictionary[ServiceDictionaryKey.ERROR]);
                        break;
                    case HttpStatusCode.Forbidden:
                        actionResult = new BadRequestObjectResult("(403) Forbidden: " + dictionary[ServiceDictionaryKey.ERROR]);
                        break;
                    default:
                        actionResult = new BadRequestObjectResult(dictionary[ServiceDictionaryKey.ERROR]);
                        break;
                }
            }

            return actionResult;
        }
    }
}