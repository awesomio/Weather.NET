using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Awesomio.NET.Models.CurrentWeather;
using Awesomio.Weather.NET.Models.OneCall;
using System.Collections.Generic;
using System.Linq;
using Awesomio.Weather.NET.Models.OneCallHistory;

namespace Awesomio.Weather.NET.Tests
{
    [TestClass()]
    public class WeatherClientTests
    {
        private const string AccessKey = "<YOUR API KEY HERE>";
        private List<Сoordinates> _citiesCoordinates = new List<Сoordinates>()
        {
            new Сoordinates() {Lat = -35.280937, Lon = 149.130009},
            new Сoordinates() {Lat = 50.850340, Lon = 4.351710},
            new Сoordinates() {Lat = 39.904200, Lon = 116.407396},
            new Сoordinates() {Lat = 51.507351, Lon = -0.127758},
            new Сoordinates() {Lat = 55.755826, Lon = 37.617300},
            new Сoordinates() {Lat = 50.075538, Lon = 14.437800},
            new Сoordinates() {Lat = 59.436961, Lon = 24.753575},
            new Сoordinates() {Lat = 48.856614, Lon = 2.352222},
            new Сoordinates() {Lat = 52.520007, Lon = 13.404954},
            new Сoordinates() {Lat = 64.146582, Lon = -21.942635},
        };

        internal class Сoordinates
        {
            public double Lat { get; set; }
            public double Lon { get; set; }
        }

        [TestMethod()]
        public void GetOneCallApiTest()
        {
            WeatherClient qWMClient = new WeatherClient(AccessKey);
            // OneCallExclude[] oneCallExcludes = { OneCallExclude.Current, OneCallExclude.Daily };

            foreach (Сoordinates coord in _citiesCoordinates)
            {
                OneCallModel data = qWMClient.GetOneCallApiAsync<OneCallModel>(coord.Lat, coord.Lon, "en").Result;
            }
        }

        [TestMethod()]
        public void GetCurrentWeatherTest()
        {
            List<string> cities = new List<string>()
            {
                "Canberra",
                "Brussels",
                "Beijing",
                "London",
                "Moscow",
                "Prague",
                "Tallinn",
                "Helsinki",
                "Paris",
                "Berlin",
                "Reykjavik"
            };

            WeatherClient client = new WeatherClient(AccessKey);

            foreach (string city in cities)
            {
                CurrrentWeatherModel data = client.GetCurrentWeatherAsync<CurrrentWeatherModel>(city, "en").Result;
            }
        }

        [TestMethod()]
        public void GetOneCallHistoricalWeatherTest()
        {
            // https://openweathermap.org/api/one-call-api#history

            var qWMClient = new WeatherClient(AccessKey);

            var dt = DateTime.UtcNow.AddDays(-5);
            foreach (Сoordinates coord in _citiesCoordinates)
            {
                var data = qWMClient.GetOneCallHistoryApiAsync<OneCallHistoryModel>(dt, coord.Lat, coord.Lon, "en").Result;
                foreach (var h in data.Hourly)
                {
                    var dtH = DateTimeOffset.FromUnixTimeSeconds(h.Dt).DateTime;
                    Console.WriteLine($"[{coord.Lat},{coord.Lon}] {dtH:yyyy-MM-dd hh:mm:ss}: temp={h.Temp}");
                }
            }
        }

        /// <summary>
        /// This test requires a Developer or above API key, see https://openweathermap.org/price.
        /// </summary>
        [TestMethod()]
        public void GetHourlyForecastTest()
        {
            // https://openweathermap.org/api/hourly-forecast
            var qWMClient = new WeatherClient(AccessKey, WeatherClient.DefaultApiTimeoutSec, WeatherClient.DefaultProApiUrl);

            foreach (Сoordinates coord in _citiesCoordinates)
            {
                var data = qWMClient.GetHourlyForecastApiAsync<HourlyForecastModel>(coord.Lat, coord.Lon, "en").Result;
                foreach (var h in data.HourlyForecasts)
                {
                    var dtH = DateTimeOffset.FromUnixTimeSeconds(h.Dt).DateTime;
                    Console.WriteLine($"[{coord.Lat},{coord.Lon}] {dtH:yyyy-MM-dd hh:mm:ss}: temp={h.MainForecast.Temp}, weather={h.Weather.First()?.Main}");
                }
            }
        }

        [TestMethod()]
        public void GetHourlyForecastLimitedHoursTest()
        {
            // https://openweathermap.org/api/hourly-forecast
            var qWMClient = new WeatherClient(AccessKey, WeatherClient.DefaultApiTimeoutSec, WeatherClient.DefaultProApiUrl);

            foreach (Сoordinates coord in _citiesCoordinates)
            {
                var data = qWMClient.GetHourlyForecastApiAsync<HourlyForecastModel>(coord.Lat, coord.Lon, "en",6).Result;

                Assert.AreEqual(6,data.NumHourlyForecasts);
                Assert.AreEqual(6, data.HourlyForecasts.Count);
            }
        }
    }
}