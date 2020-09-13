using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Awesomio.Weather.NET
{
    [Serializable]
    public class WeatherException : Exception
    {
        public string RequestMethod { get; }
        public dynamic DataJson { get; private set; }

        public WeatherException()
        { }

        public WeatherException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public WeatherException(string message)
            : base(message)
        {
        }

        public WeatherException(string callerMethod, string message)
            : base(message)
        {
            RequestMethod = callerMethod;
        }

        public static void ThrowException(string callerMethod, dynamic json)
        {
            var ex = new WeatherException(callerMethod, FormatMessage(callerMethod, json))
            {
                DataJson = json
            };

            throw ex;
        }

        private static string FormatMessage(string callerMethod, dynamic json)
        {
            if (json == null)
                return string.Format(CultureInfo.InvariantCulture, "Failed request {0}. Message: Null", callerMethod);
   
            var result =
                string.Format(CultureInfo.InvariantCulture, "Failed request {0}. Message: {1}. Error Code: {2}.", callerMethod, (string)json.message, (int)json.cod);
            //Console.WriteLine(result);
            //if (json.error.error_lists != null)
            //{
            //    result += " Details: " + json.error.error_lists.ToString();
            //}

            return result;
        }

        protected WeatherException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}