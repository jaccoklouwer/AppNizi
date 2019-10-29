using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models.AccountModels
{
    public class AuthLogin
    {
        public string Guid { get; set; }
        public Token Token { get; set; }
    }
}
