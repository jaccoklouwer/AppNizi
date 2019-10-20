using AppNiZiAPI.Variables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppNiZiAPI.Security
{
    class Authorization
    {
        public static async Task<bool> CheckAuthorization(IHeaderDictionary headers)
        {
            AuthenticationHeaderValue.TryParse(headers[HeaderNames.Authorization], out var authHeader);
            return await Auth0.ValidateTokenAsync(authHeader) == null ? true : false;
        }
    }
}

