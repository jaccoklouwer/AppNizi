using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using AppNiZiAPI.Infrastructure;
using AppNiZiAPI.Models.AccountModels;
using AppNiZiAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppNiZiAPI.Security
{
    class Authorization : IAuthorization
    {
        /**
         * Authorization Check for every call
        */

        private readonly int UnauthorizedStatusCode = 401;
        private readonly int ForbiddenStatusCode = 403;

        public async Task<AuthResultModel> CheckAuthorization(HttpRequest req, int userId = 0)
        {
            bool isDoctor = false;

            // Get AuthentificationHeader from request
            AuthenticationHeaderValue.TryParse(req.Headers[HeaderNames.Authorization], out var authHeader);

            if (authHeader == null)
                return new AuthResultModel(false, UnauthorizedStatusCode);

            // Token validation with Auth0 servers
            ClaimsPrincipal claims = await Auth0.ValidateTokenAsync(authHeader);

            if (claims == null)
                return new AuthResultModel(false, UnauthorizedStatusCode);

            // Get Token Guid for Authorization
            string tokenGuid = claims.FindFirst("azp").Value;

            // Get the role from the body
            try
            {
                var content = await new StreamReader(req.Body).ReadToEndAsync();
                var jsonParsed = JObject.Parse(content);
                if (jsonParsed["Role"].ToString() == "Doctor")
                    isDoctor = true;
            }
            catch (Exception){}

            IAuthorizationRepository authRepository = DIContainer.Instance.GetService<IAuthorizationRepository>();

            // If userId needs to come from token, only calls the method GetAccountId if userId is zero
            if (userId == 0 && authRepository.GetAccountId(tokenGuid, isDoctor) == 0)
                return new AuthResultModel(false, ForbiddenStatusCode);

            // When a call is from a Doctor that needs info about a patient, the following method will be called
            // UserId is here patientId
            if (isDoctor && !authRepository.CheckDoctorAcces(userId, tokenGuid))
                return new AuthResultModel(false, ForbiddenStatusCode);

            // When a call is from a patient of doctor and only ask for information about the same user the following method will be called
            if (userId != 0 && !authRepository.PatientAuth(userId, tokenGuid, isDoctor))
                return new AuthResultModel(false, ForbiddenStatusCode);

            return new AuthResultModel(true);
        }

        public async Task<AuthLogin> LoginAuthAsync(HttpRequest req)
        {
            AuthenticationHeaderValue.TryParse(req.Headers[HeaderNames.Authorization], out var authHeader);

            if (authHeader == null)
                return null;

            ClaimsPrincipal claims = await Auth0.ValidateTokenAsync(authHeader);

            Token token = new Token
            {
                Scheme = authHeader.Scheme,
                AccesCode = authHeader.Parameter
            };

            AuthLogin authLogin = new AuthLogin
            {   Token = token,
                Guid = claims.FindFirst("azp").Value
            };

            return authLogin;
        }
    }

    public interface IAuthorization
    {
        Task<AuthLogin> LoginAuthAsync(HttpRequest req);
        Task<AuthResultModel> CheckAuthorization(HttpRequest req, int userId = 0);
    }
}

