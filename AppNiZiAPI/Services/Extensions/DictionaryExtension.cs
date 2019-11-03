using AppNiZiAPI.Models.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Services.Helpers
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// Generates an error message for feedback, and adds it to the dictionary.
        /// </summary>
        /// <typeparam name="ServiceDictionaryKey"></typeparam>
        /// <param name="key">Key to add it under.</param>
        /// <param name="ex">Exception.</param>
        /// <param name="feedbackHandler">IFeedbackHandler for processing the error into an error message.</param>
        public static void AddErrorMessage<ServiceDictionaryKey>(this Dictionary<ServiceDictionaryKey, object> dictionary,
                ServiceDictionaryKey key,
                Exception ex,
                IMessageHandler feedbackHandler)
        {
            string callbackMessage = feedbackHandler.BuildErrorMessage(ex);
            dictionary.Add(key, callbackMessage);
        }
    }
}
