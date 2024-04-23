using DropSun.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMeteo;
using System.Diagnostics;

namespace DropSun.Model.Weather
{
    public class ObtainWeather
    {
        public async static Task<WeatherForecast> FromOpenMeteo(string location)
        {
            WeatherForecast forecast = await RunAsync(location);
            return forecast;
        }

        static async Task<WeatherForecast> RunAsync(string location)
        {
            // Before using the library you have to create a new client. Once created you can reuse it for every other api call you are going to make. 
            // There is no need to create multiple clients.
            OpenMeteo.OpenMeteoClient client = new OpenMeteo.OpenMeteoClient();

            // Make a new api call to get the current weather in tokyo
            WeatherForecast weatherData = await client.QueryAsync(location).ConfigureAwait(false);

            return weatherData;
        }
    }
}
