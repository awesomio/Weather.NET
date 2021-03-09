using Microsoft.VisualStudio.TestTools.UnitTesting;
using Awesomio.Weather.NET;
using System;
using System.Collections.Generic;
using System.Text;
using Awesomio.Weather.NET.Enums;
using Awesomio.NET.Models.CurrentWeather;
using Awesomio.Weather.NET.Models.OneCall;

namespace Awesomio.Weather.NET.Tests
{
    [TestClass()]
    public class WeatherClientTests
    {
        [TestMethod()]
        public void GetOneCallApiTest()
        {
            string accessKey = "YOUR_ACCESS_KEY";
            
            WeatherClient qWMClient = new WeatherClient(accessKey);
            OneCallExclude[] oneCallExcludes = { OneCallExclude.Current, OneCallExclude.Daily };
            OneCallModel data = qWMClient.GetOneCallApi<OneCallModel>(40.12, 96.66, "en", oneCallExcludes).Result;

        }

        [TestMethod()]
        public void GetCurrentWeatherTest()
        {
            string accessKey = "YOUR_ACCESS_KEY";
           
            WeatherClient client = new WeatherClient(accessKey);
            CurrrentWeatherModel data = client.GetCurrentWeather<CurrrentWeatherModel>("London", "en").Result;
        }
    }
}