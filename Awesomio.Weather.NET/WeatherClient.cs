using Awesomio.Weather.NET.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Awesomio.Weather.NET
{
    public class WeatherClient
    {
        private const int DefaultApiTimeoutSec = 10;
        private readonly WeatherRestApi _restApi;
        private string m_accessKey;
        private class NameValueDictionary : Dictionary<string, string> { }


        public WeatherClient(string accessKey, int apiTimeoutSec = DefaultApiTimeoutSec, string overrideBaseAddress = null)
        {
            _restApi = new WeatherRestApi(accessKey, apiTimeoutSec, overrideBaseAddress);
            m_accessKey = accessKey;
        }

        protected virtual async Task<dynamic> CallApiAsync<T>(string apiCommand, RequestType requestType = RequestType.Get, Dictionary<string, string> args = null)
        {
            return await _restApi.CallApiAsync<T>(apiCommand, requestType, args).ConfigureAwait(false);
        }
    
        public async Task<T> GetCurrentWeatherAsync<T>(string cityName, string lang, string units = "metric", string accessToken = null, string version = "2.5")
        {
            var args = new NameValueDictionary
            {
                {"q", cityName},
                {"lang", lang},
                {"units", units},
            };

            if (accessToken == null)
            {
                args.Add("appid", m_accessKey.ToString(CultureInfo.InvariantCulture));
            }
            else if (accessToken != null)
            {
                args.Add("appid", accessToken.ToString(CultureInfo.InvariantCulture));
            }

            return await CallApiAsync<T>($"/data/{version}/weather/", RequestType.Get, args).ConfigureAwait(false);
        }

        public async Task<T> GetOneCallApiAsync<T>(double lat, double lon, string lang, OneCallExclude[] exclude = null, string units = "metric", string accessToken = null, string version = "2.5")
        {
            var args = new NameValueDictionary
            {
                {"lat", lat.ToString(CultureInfo.InvariantCulture)},
                {"lon", lon.ToString(CultureInfo.InvariantCulture)},
                {"lang", lang},
                {"units", units},
            };
            if (exclude != null)
            {
                string exludeStr = String.Empty;
                foreach (OneCallExclude exlud in exclude)
                {
                    exludeStr += exlud.ToString() + ",";
                }

                args.Add("exclude", exludeStr.ToString());
            }

            if (accessToken == null)
            {
                args.Add("appid", m_accessKey.ToString(CultureInfo.InvariantCulture));
            }
            else if (accessToken != null)
            {
                args.Add("appid", accessToken.ToString(CultureInfo.InvariantCulture));
            }

            return await CallApiAsync<T>($"/data/{version}/onecall", RequestType.Get, args).ConfigureAwait(false);
        }
    }
}
