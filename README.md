# Weather.NET
OpenWeatherMap based .NET API

Simple .NET Weather API, currently gets the current weather, One Call API and One Call API historical weather data
 
Please, see https://openweathermap.org/api to start easy :)

EXAMPLE:
```c#
            string accessKey = "YOUR_ACCESS_KEY";
            WeatherClient client = new WeatherClient(accessKey);
            
            CurrrentWeatherModel data = client.GetCurrentWeatherAsync<CurrrentWeatherModel>("London", "en", "metric").Result;  
            
            double feelsLike = data.Main.FeelsLike;
            double tempMin = data.Main.TempMin;
            double tempMax = data.Main.TempMax;
                       
            OneCallExclude[] oneCallExcludes = { OneCallExclude.Current, OneCallExclude.Daily };
            OneCallModel dataOneCall = client.GetOneCallApiAsync<OneCallModel>(40.12, 96.66, "en", oneCallExcludes).Result;
                
```
