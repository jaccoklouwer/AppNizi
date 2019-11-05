using AppNiZiAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AppNiZiAPI.Models.Handlers
{
    public interface IMessageHandler
    {
        string BuildErrorMessage(Exception ex);
    }

    public class MessageHandler : IMessageHandler
    {
        /// <summary>
        /// Builds a feedback message for an error caught.
        /// </summary>
        public string BuildErrorMessage(Exception ex)
        {
            // Provide the user with some feedback as to the cause of the error
            string callbackMessage = "";
            if (ex.InnerException != null)
                callbackMessage = ex.InnerException.Message;
            else if (ex.Message != null)
                callbackMessage = ex.Message;

            // Only get first part
            callbackMessage = callbackMessage.Split('.')[0];
            callbackMessage += ". ";

            // Extra feedback for datetime
            if (callbackMessage.ToLower().Contains("datetime"))
                callbackMessage += "Please use format YYYY-MM-DD.";

            // Due to security
            if (callbackMessage.ToLower().Contains("stacktrace"))
                callbackMessage = "An error occurred.";

            return callbackMessage;
        }
    }
}

