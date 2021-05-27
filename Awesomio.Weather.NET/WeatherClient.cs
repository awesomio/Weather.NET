using Awesomio.Weather.NET.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Awesomio.Weather.NET
{
    public class WeatherClient
    {
        public const string DefaultApiUrl = "https://api.openweathermap.org/";
        public const string DefaultProApiUrl = "https://pro.openweathermap.org/";
        public const int DefaultApiTimeoutSec = 10;
        private readonly WeatherRestApi _restApi;
        private string m_accessKey;
        private class NameValueDictionary : Dictionary<string, string> { }

        /// <summary>
        /// The defaults work well for free tier or the API but the developer and up tiers require a different URL.
        /// </summary>
        /// <param name="accessKey"></param>
        /// <param name="apiTimeoutSec"></param>
        /// <param name="overrideBaseAddress">Leave as default for free tier and use DefaultProApiUrl for pro and higher tiers.</param>
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

        public async Task<T> GetOneCallHistoryApiAsync<T>(DateTime dt, double lat, double lon, string lang, string units = "metric", string accessToken = null, string version = "2.5")
        {
            var args = new NameValueDictionary
            {
                {"lat", lat.ToString(CultureInfo.InvariantCulture)},
                {"lon", lon.ToString(CultureInfo.InvariantCulture)},
                {"lang", lang},
                {"units", units},
                {"dt", "" + new DateTimeOffset(dt).ToUnixTimeSeconds()},
            };

            if (accessToken == null)
            {
                args.Add("appid", m_accessKey.ToString(CultureInfo.InvariantCulture));
            }
            else if (accessToken != null)
            {
                args.Add("appid", accessToken.ToString(CultureInfo.InvariantCulture));
            }

            return await CallApiAsync<T>($"/data/{version}/onecall/timemachine", RequestType.Get, args).ConfigureAwait(false);
        }


        /// <summary>
        /// Requires at least 'Developer' plan API subscription.
        /// https://openweathermap.org/api/hourly-forecast
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="lang"></param>
        /// <param name="numHours">Default or <=0 to get the max, >0 to get a set number of hourly forecasts.</param>
        /// <param name="units"></param>
        /// <param name="accessToken"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public async Task<T> GetHourlyForecastApiAsync<T>(double lat, double lon, string lang, int numHours = 0, string units = "metric", string accessToken = null, string version = "2.5")
        {
            var args = new NameValueDictionary
            {
                {"lat", lat.ToString(CultureInfo.InvariantCulture)},
                {"lon", lon.ToString(CultureInfo.InvariantCulture)},
                {"lang", lang},
                {"units", units},
            };
            if (numHours > 0)
            {
                args["cnt"] = numHours.ToString();
            }

            if (accessToken == null)
            {
                args.Add("appid", m_accessKey.ToString(CultureInfo.InvariantCulture));
            }
            else if (accessToken != null)
            {
                args.Add("appid", accessToken.ToString(CultureInfo.InvariantCulture));
            }

            return await CallApiAsync<T>($"/data/{version}/forecast/hourly", RequestType.Get, args).ConfigureAwait(false);
        }
    }
}
