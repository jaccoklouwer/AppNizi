using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AppNiZiAPI.Services.Serializer
{
    public interface IMessageSerializer
    {
        T Deserialize<T>(string message);
        string Serialize(object obj);

        Task<T> Deserialize<T>(Stream stream);
    }

    public class JsonMessageSerializer : IMessageSerializer
    {
        /// <summary>
        /// JSON to Object.
        /// </summary>
        public T Deserialize<T>(string message)
        {
            var obj = JsonConvert.DeserializeObject<T>(message);
            return obj;
        }

        /// <summary>
        /// Data stream to Object.
        /// </summary>
        public async Task<T> Deserialize<T>(Stream stream)
        {
            StreamReader streamReader = new StreamReader(stream);
            string message = await streamReader.ReadToEndAsync();
            streamReader.Dispose();

            return Deserialize<T>(message);
        }

        /// <summary>
        /// Object to JSON.
        /// </summary>
        public string Serialize(object obj)
        {
            var message = JsonConvert.SerializeObject(obj);
            return message;
        }
    }
}
