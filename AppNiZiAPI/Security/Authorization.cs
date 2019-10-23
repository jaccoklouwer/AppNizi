using AppNiZiAPI.Variables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AppNiZiAPI.Security
{
    class Authorization
    {
        public static async Task<bool> CheckAuthorization(IHeaderDictionary headers, int patientId)
        {
            AuthenticationHeaderValue.TryParse(headers[HeaderNames.Authorization], out var authHeader);

            if (authHeader == null)
                return false;

            ClaimsPrincipal claims = await Auth0.ValidateTokenAsync(authHeader);

            if (claims == null)
                return false;

            string tokenGuid = claims.FindFirst("azp").Value;

            if (new AuthorizationRepositories().ChecKUser(patientId, tokenGuid))
                return true;
            return false;
        }
    }
}

