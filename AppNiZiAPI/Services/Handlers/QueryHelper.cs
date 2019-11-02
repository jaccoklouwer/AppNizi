using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models.Handler
{
    public interface IQueryHelper
    {
        int ExtractIntegerFromRequestQuery(string variableName, HttpRequest req);
    }

    public class QueryHelper : IQueryHelper
    {
        public int ExtractIntegerFromRequestQuery(string variableName, HttpRequest req)
        {
            int data = 0;

            string dataFromQuery = req.Query[variableName];
            if (dataFromQuery != null)
                data = Convert.ToInt32(dataFromQuery);

            return data;
        }
    }
}
