using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Awesomio.NET.Models.CurrentWeather;
using Awesomio.Weather.NET.Models.OneCall;
using System.Collections.Generic;
using Awesomio.Weather.NET.Models.OneCallHistory;

namespace Awesomio.Weather.NET.Tests
{
    [TestClass()]
    public class WeatherClientTests
    {
        internal class Сoordinates
        {
            public double Lat { get; set; }
            public double Lon { get; set; }
        }

        [TestMethod()]
        public void GetOneCallApiTest()
        {
            string accessKey = "YOUR_ACCESS_KEY";

            WeatherClient qWMClient = new WeatherClient(accessKey);
            // OneCallExclude[] oneCallExcludes = { OneCallExclude.Current, OneCallExclude.Daily };

            List<Сoordinates> citiesCoordinates = new List<Сoordinates>()
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

            foreach (Сoordinates coord in citiesCoordinates)
            {
                OneCallModel data = qWMClient.GetOneCallApiAsync<OneCallModel>(coord.Lat, coord.Lon, "en").Result;
            }
        }

        [TestMethod()]
        public void GetCurrentWeatherTest()
        {
            string accessKey = "YOUR_ACCESS_KEY";

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

            WeatherClient client = new WeatherClient(accessKey);

            foreach (string city in cities)
            {
                CurrrentWeatherModel data = client.GetCurrentWeatherAsync<CurrrentWeatherModel>(city, "en").Result;
            }
        }

        [TestMethod()]
        public void GetOneCallHistoricalWeatherTest()
        {
            // https://openweathermap.org/api/one-call-api#history

            string accessKey = "YOUR_ACCESS_KEY";
            var qWMClient = new WeatherClient(accessKey);

            List<Сoordinates> citiesCoordinates = new List<Сoordinates>()
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

            var dt = DateTime.UtcNow.AddDays(-5);
            foreach (Сoordinates coord in citiesCoordinates)
            {
                var data = qWMClient.GetOneCallHistoryApiAsync<OneCallHistoryModel>(dt, coord.Lat, coord.Lon, "en").Result;
                foreach (var h in data.Hourly)
                {
                    var dtH = DateTimeOffset.FromUnixTimeSeconds(h.Dt).DateTime;
                    Console.WriteLine($"[{coord.Lat},{coord.Lon}] {dtH:yyyy-MM-dd MM:ss}: temp={h.Temp}");
                }
            }
        }
    }
}