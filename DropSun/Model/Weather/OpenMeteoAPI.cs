using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Windows.Web.Http;
using System.Collections;
using DropSun.Model.Geolocation;
using System.Text.Json;
using System.Diagnostics;
using System.Net.Http;

namespace DropSun.Model.Weather
{
    internal class OpenMeteoAPI
    {
        public static async Task<OpenMeteoWeatherOverview> GetWeatherAsync(double latitude, double longitude)
        {
            string latitudeString = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string longitudeString = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string url = $"https://api.open-meteo.com/v1/forecast?latitude={latitudeString}&longitude={longitudeString}&current=temperature_2m,relative_humidity_2m,apparent_temperature,is_day,precipitation,rain,showers,snowfall,weather_code,cloud_cover,pressure_msl,surface_pressure,wind_speed_10m,wind_direction_10m,wind_gusts_10m&hourly=temperature_2m,apparent_temperature,precipitation_probability,weather_code,visibility,wind_speed_10m,is_day&daily=weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min,precipitation_sum&timezone=Europe%2FBerlin&forecast_hours=6";
            try
            {
                System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"DropSun/{version} (Blindside-Studios; WeatherApp/OverviewQuery)");
                string jsonResponse = await _httpClient.GetStringAsync(url);
                var results = JsonSerializer.Deserialize<OpenMeteoWeatherOverview>(jsonResponse);
                return results;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching data: {ex.Message}");
                return null;
            }
        }

        public static OpenMeteoWeatherOverview GetWeatherFromJSON(string json)
        {
            return JsonSerializer.Deserialize<OpenMeteoWeatherOverview>(json);
        }
    }

    public class OpenMeteoWeatherOverview
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("generationtime_ms")]
        public double GenerationtimeMs { get; set; }

        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; set; }

        [JsonPropertyName("elevation")]
        public double Elevation { get; set; }

        [JsonPropertyName("current_units")]
        public CurrentUnits CurrentUnits { get; set; }

        [JsonPropertyName("current")]
        public CurrentWeather Current { get; set; }

        [JsonPropertyName("hourly_units")]
        public HourlyUnits HourlyUnits { get; set; }

        [JsonPropertyName("hourly")]
        public Hourly Hourly { get; set; }

        [JsonPropertyName("daily_units")]
        public DailyUnits DailyUnits { get; set; }

        [JsonPropertyName("daily")]
        public Daily Daily { get; set; }
        
        public WeatherDescriptions GetWeatherDescription()
        {
            switch (Current.WeatherCode)
            {
                case 0: return WeatherDescriptions.ClearSky;
                case 1: return WeatherDescriptions.MainlyClear;
                case 2: return WeatherDescriptions.PartlyCloudy;
                case 3: return WeatherDescriptions.Overcast;
                case 45: return WeatherDescriptions.Fog;
                case 48: return WeatherDescriptions.DepositingRimeFog;
                case 51: return WeatherDescriptions.DrizzleLight;
                case 53: return WeatherDescriptions.DrizzleModerate;
                case 55: return WeatherDescriptions.DrizzleDense;
                case 56: return WeatherDescriptions.FreezingDrizzleLight;
                case 57: return WeatherDescriptions.FreezingDrizzleDense;
                case 61: return WeatherDescriptions.RainSlight;
                case 63: return WeatherDescriptions.RainModerate;
                case 65: return WeatherDescriptions.RainHeavy;
                case 66: return WeatherDescriptions.FreezingRainLight;
                case 67: return WeatherDescriptions.FreezingRainHeavy;
                case 71: return WeatherDescriptions.SnowFallSlight;
                case 73: return WeatherDescriptions.SnowFallModerate;
                case 75: return WeatherDescriptions.SnowFallHeavy;
                case 77: return WeatherDescriptions.SnowGrains;
                case 80: return WeatherDescriptions.RainShowersSlight;
                case 81: return WeatherDescriptions.RainShowersModerate;
                case 82: return WeatherDescriptions.RainShowersViolent;
                case 85: return WeatherDescriptions.SnowShowersSlight;
                case 86: return WeatherDescriptions.SnowShowersHeavy;
                case 95: return WeatherDescriptions.ThunderstormSlightOrModerate;
                case 96: return WeatherDescriptions.ThunderstormWithHailSlight;
                case 99: return WeatherDescriptions.ThunderstormWithHailHeavy;
                default: throw new ArgumentOutOfRangeException(nameof(Current.WeatherCode), "Invalid weather code");
            }
        }
    }

    public class CurrentUnits
    {
        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("interval")]
        public string Interval { get; set; }

        [JsonPropertyName("temperature_2m")]
        public string Temperature2M { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public string RelativeHumidity2M { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public string ApparentTemperature { get; set; }

        [JsonPropertyName("is_day")]
        public string IsDay { get; set; }

        [JsonPropertyName("precipitation")]
        public string Precipitation { get; set; }

        [JsonPropertyName("rain")]
        public string Rain { get; set; }

        [JsonPropertyName("showers")]
        public string Showers { get; set; }

        [JsonPropertyName("snowfall")]
        public string Snowfall { get; set; }

        [JsonPropertyName("weather_code")]
        public string WeatherCode { get; set; }

        [JsonPropertyName("cloud_cover")]
        public string CloudCover { get; set; }

        [JsonPropertyName("pressure_msl")]
        public string PressureMsl { get; set; }

        [JsonPropertyName("surface_pressure")]
        public string SurfacePressure { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public string WindSpeed10M { get; set; }

        [JsonPropertyName("wind_direction_10m")]
        public string WindDirection10M { get; set; }

        [JsonPropertyName("wind_gusts_10m")]
        public string WindGusts10M { get; set; }
    }

    public class CurrentWeather
    {
        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("interval")]
        public int Interval { get; set; }

        [JsonPropertyName("temperature_2m")]
        public double Temperature2M { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public int RelativeHumidity2M { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public double ApparentTemperature { get; set; }

        [JsonPropertyName("is_day")]
        public int IsDay { get; set; }

        [JsonPropertyName("precipitation")]
        public double Precipitation { get; set; }

        [JsonPropertyName("rain")]
        public double Rain { get; set; }

        [JsonPropertyName("showers")]
        public double Showers { get; set; }

        [JsonPropertyName("snowfall")]
        public double Snowfall { get; set; }

        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; set; }

        [JsonPropertyName("cloud_cover")]
        public int CloudCover { get; set; }

        [JsonPropertyName("pressure_msl")]
        public double PressureMsl { get; set; }

        [JsonPropertyName("surface_pressure")]
        public double SurfacePressure { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed10M { get; set; }

        [JsonPropertyName("wind_direction_10m")]
        public int WindDirection10M { get; set; }

        [JsonPropertyName("wind_gusts_10m")]
        public double WindGusts10M { get; set; }
    }

    public class HourlyUnits
    {
        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public string Temperature2M { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public string ApparentTemperature { get; set; }

        [JsonPropertyName("precipitation_probability")]
        public string PrecipitationProbability { get; set; }

        [JsonPropertyName("weather_code")]
        public string WeatherCode { get; set; }

        [JsonPropertyName("visibility")]
        public string Visibility { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public string WindSpeed10M { get; set; }

        [JsonPropertyName("is_day")]
        public string IsDay { get; set; }
    }

    public class Hourly
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public List<double> Temperature2M { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public List<double> ApparentTemperature { get; set; }

        [JsonPropertyName("precipitation_probability")]
        public List<int> PrecipitationProbability { get; set; }

        [JsonPropertyName("weather_code")]
        public List<int> WeatherCode { get; set; }

        [JsonPropertyName("visibility")]
        public List<double> Visibility { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public List<double> WindSpeed10M { get; set; }

        [JsonPropertyName("is_day")]
        public List<int> IsDay { get; set; }
    }

    public class DailyUnits
    {
        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("weather_code")]
        public string WeatherCode { get; set; }

        [JsonPropertyName("temperature_2m_max")]
        public string Temperature2MMax { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public string Temperature2MMin { get; set; }

        [JsonPropertyName("apparent_temperature_max")]
        public string ApparentTemperatureMax { get; set; }

        [JsonPropertyName("apparent_temperature_min")]
        public string ApparentTemperatureMin { get; set; }

        [JsonPropertyName("precipitation_sum")]
        public string PrecipitationSum { get; set; }
    }

    public class Daily
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; }

        [JsonPropertyName("weather_code")]
        public List<int> WeatherCode { get; set; }

        [JsonPropertyName("temperature_2m_max")]
        public List<double> Temperature2MMax { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public List<double> Temperature2MMin { get; set; }

        [JsonPropertyName("apparent_temperature_max")]
        public List<double> ApparentTemperatureMax { get; set; }

        [JsonPropertyName("apparent_temperature_min")]
        public List<double> ApparentTemperatureMin { get; set; }

        [JsonPropertyName("precipitation_sum")]
        public List<double> PrecipitationSum { get; set; }
    }

    public enum WeatherDescriptions
    {
        ClearSky = 0,
        MainlyClear = 1,
        PartlyCloudy = 2,
        Overcast = 3,
        Fog = 45,
        DepositingRimeFog = 48,
        DrizzleLight = 51,
        DrizzleModerate = 53,
        DrizzleDense = 55,
        FreezingDrizzleLight = 56,
        FreezingDrizzleDense = 57,
        RainSlight = 61,
        RainModerate = 63,
        RainHeavy = 65,
        FreezingRainLight = 66,
        FreezingRainHeavy = 67,
        SnowFallSlight = 71,
        SnowFallModerate = 73,
        SnowFallHeavy = 75,
        SnowGrains = 77,
        RainShowersSlight = 80,
        RainShowersModerate = 81,
        RainShowersViolent = 82,
        SnowShowersSlight = 85,
        SnowShowersHeavy = 86,
        ThunderstormSlightOrModerate = 95,
        ThunderstormWithHailSlight = 96,
        ThunderstormWithHailHeavy = 99
    }
}
