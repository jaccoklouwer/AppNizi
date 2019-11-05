using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace AppNiZiAPI.Functions
{
    public static class GetToken
    {
        [FunctionName("GetToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //var client = new RestClient("https://appnizi.eu.auth0.com/oauth/token");
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("content-type", "application/x-www-form-urlencoded");
            //request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials&" +
            //    "client_id=BEWe620aN1psUCDMdtyymzpKvM7gmKJE" +
            //    "&client_secret=GWbroX4DQJEWBr0rZAKFOtj_qBJaFWCLG6M6arLkG4D7rEdN_He21gTYIC2TRbxR" +
            //    "&audience=https%3A%2F%2F%24%7Baccount.namespace%7D%2Fapi%2Fv2%2F", ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);

            var client = new RestClient("https://appnizi.eu.auth0.com/api/v2/clients/lyvNV89UXHNVDC7D8XFdv35HIpPNzFum");
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik5ERkdPRFUxTnpJNFJEZ3lNakkxUmtFMU5EZ3dRMEUxTkVJM05UTTBSRGRFUTBFNE5FWkdNZyJ9");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/json", "{ \"grant_types\": \"password \" }", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            client = new RestClient("https://appnizi.eu.auth0.com/oauth/token");
            request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=password&username=doktor@doktor.com&password=ProjectCloud!&audience=appnizi.nl/api&scope=openid&client_id=lyvNV89UXHNVDC7D8XFdv35HIpPNzFum&client_secret=9sieaPoIz42CxkelXM8jk_izfyyoAzpOkPyXs_ceRW5KS5slO0phSS_CShFXcaGu", ParameterType.RequestBody);
            response = client.Execute(request);
            log.LogInformation(response.Content);














            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
