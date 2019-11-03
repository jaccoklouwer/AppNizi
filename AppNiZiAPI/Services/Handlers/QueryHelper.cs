using AppNiZiAPI.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models.Handler
{
    public interface IQueryHelper
    {
        int ExtractIntegerFromRequestQuery(string variableName, HttpRequest req);

        bool IsValidId(Dictionary<ServiceDictionaryKey, object> dictionary, string idText);

        bool IsValidInteger(Dictionary<ServiceDictionaryKey, object> dictionary, string text);
    }

    public class QueryHelper : IQueryHelper
    {
        /// <summary>
        /// Tries to extract an integer from a query for a given variable. 
        /// If unsuccesful for some reason, it will simply return 0.
        /// </summary>
        public int ExtractIntegerFromRequestQuery(string variableName, HttpRequest req)
        {
            int data = 0;

            // Get the data from the query for the given variable
            string dataFromQuery = req.Query[variableName];

            // If there's data set and it's a valid integer, set it.
            if (dataFromQuery != null)
                if (Int32.TryParse(dataFromQuery, out int integerValue))
                    data = integerValue;

            return data;
        }

        /// <summary>
        /// Checks if a string can be converted to integer, and logs it if it doesn't as an error.
        /// </summary>
        public bool IsValidInteger(Dictionary<ServiceDictionaryKey, object> dictionary, string text)
        {
            bool success = Int32.TryParse(text, out int integerValue);

            if (!success)
                dictionary.Add(ServiceDictionaryKey.ERROR, "Malformed ID passed. Please pass a single number.");

            return success;
        }

        /// <summary>
        /// Checks if a passed 'id' is considered valid, meaning: can be converted to int and is higher than 0.
        /// Logs it if it doesn't for feedback to API user.
        /// </summary>
        public bool IsValidId(Dictionary<ServiceDictionaryKey, object> dictionary, string idText)
        {
            if (!IsValidInteger(dictionary, idText))
                return false;

            int id = Int32.Parse(idText);

            if (id <= 0)
            {
                dictionary.Add(ServiceDictionaryKey.ERROR, "Id is invalid. Please insert a single positive number.");
                return false;
            }

            return true;
        }
    }
}
