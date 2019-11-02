using AppNiZiAPI.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    public class AuthResultModel
    {
        public AuthResultModel(bool result, AuthStatusCode statusCode)
        {
            Result = result;
            StatusCode = statusCode;
        }

        public bool Result { get; set; }
        public AuthStatusCode StatusCode { get; set; }
    }
}
