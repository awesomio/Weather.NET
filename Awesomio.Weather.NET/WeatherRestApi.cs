using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Awesomio.Weather.NET
{
    internal class WeatherRestApi
    {
        private const string DefaultApiUrl = "https://api.openweathermap.org/";

        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly HttpClient _client;

        private readonly string _accessKey;


        public WeatherRestApi(string accessKey, int apiTimeoutSec, string overrideBaseAddress = null)
        {
            if (overrideBaseAddress == null)
                overrideBaseAddress = DefaultApiUrl;

            _accessKey = accessKey;

            _client = new HttpClient()
            {
                BaseAddress = new Uri(overrideBaseAddress), // apiv3
                Timeout = TimeSpan.FromSeconds(apiTimeoutSec)
            };
        }

        #region Public Methods

        public async Task<dynamic> CallPublicApiAsync(string relativeUrl)
        {
            var response = await _client.GetAsync(relativeUrl).ConfigureAwait(false);
            var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonConvert.DeserializeObject<dynamic>(resultAsString);
            return json;
        }

        public async Task<dynamic> CallApiAsync<T>(string apiCommand, RequestType requestType,
            Dictionary<string, string> args, bool getAsBinary = false)
        {
            HttpContent httpContent = null;

            if (requestType == RequestType.Post)
            {
                if (args != null && args.Any())
                {

                    httpContent = new FormUrlEncodedContent(args);
                }
            }

            try
            { 
                var relativeUrl = apiCommand;
                if (requestType == RequestType.Get)
                {
                    if (args != null && args.Any())
                    {
                        relativeUrl += "?" + UrlEncodeParams(args);
                    }
                }

                using (var request = new HttpRequestMessage(
                    requestType == RequestType.Get ? HttpMethod.Get : HttpMethod.Post,
                    new Uri(_client.BaseAddress, relativeUrl)
                    ))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                   // request.Headers.Add("Apiauth-Key", _accessKey);
                    request.Content = httpContent;

                    var response = await _client.SendAsync(request).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var json = JsonConvert.DeserializeObject<dynamic>(resultAsString);
                        WeatherException.ThrowException(apiCommand, json);
                    }

                    if (getAsBinary)
                    {
                        var resultAsByteArray = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        return resultAsByteArray;
                    }
                    else
                    {
                        var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var json = JsonConvert.DeserializeObject<T>(resultAsString);
                        return json;
                    }
                }
            }
            finally
            {
                httpContent?.Dispose();
            }
        }

        public async Task<dynamic> CallApiPostFileAsync(string apiCommand, Dictionary<string, string> args, string fileName)
        {
            using (var httpContent = new MultipartFormDataContent())
            {
                if (args != null)
                {
                    foreach (var keyValuePair in args)
                    {
                        httpContent.Add(new StringContent(keyValuePair.Value),
                            string.Format(CultureInfo.InvariantCulture, "\"{0}\"", keyValuePair.Key));
                    }
                }

                if (fileName != null)
                {
                    var fileBytes = File.ReadAllBytes(fileName);
                    httpContent.Add(new ByteArrayContent(fileBytes), "\"document\"",
                        "\"" + Path.GetFileName(fileName) + "\"");
                }

                var bodyAsBytes = await httpContent.ReadAsByteArrayAsync().ConfigureAwait(false);
  

                using (var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri(_client.BaseAddress, apiCommand)
                    ))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Content = httpContent;

                    var response = await _client.SendAsync(request).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var json = JsonConvert.DeserializeObject<dynamic>(resultAsString);
                        WeatherException.ThrowException(apiCommand, json);
                    }

                    {
                        var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var json = JsonConvert.DeserializeObject<dynamic>(resultAsString);
                        return json;
                    }
                }
            }
        }

        #endregion Public methods


        #region Private methods
       
        private static string UrlEncodeString(string text)
        {
            var result = text == null ? "" : Uri.EscapeDataString(text).Replace("%20", "+");
            return result;
        }

        private static string UrlEncodeParams(Dictionary<string, string> args)
        {
            var sb = new StringBuilder();

            var arr =
                args.Select(
                    x =>
                        string.Format(CultureInfo.InvariantCulture, "{0}={1}", UrlEncodeString(x.Key), UrlEncodeString(x.Value))).ToArray();

            sb.Append(string.Join("&", arr));
            return sb.ToString();
        }

        #endregion Private methods
    }
}