using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    public class AuthResultModel
    {
        public AuthResultModel(bool result, int statusCode = 200)
        {
            Result = result;
            StatusCode = statusCode;
        }

        public bool Result { get; set; }
        public int StatusCode { get; set; }
    }
}
