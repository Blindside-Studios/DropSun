using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace DropSun.Model.Geolocation
{
    class GeoLookup
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<List<NominatimGeolocation>> SearchLocationsAsync(string query)
        {
            string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&limit=5";
            try
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"DropSun/{version} (Blindside-Studios; WeatherApp/LocationQuery)");
                Debug.WriteLine(_httpClient.DefaultRequestHeaders.UserAgent);
                //string jsonResponse = await _httpClient.GetStringAsync(url);
                //var results = JsonSerializer.Deserialize<List<NominatimGeolocation>>(jsonResponse);
                return null;//results ?? new List<NominatimGeolocation>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching data: {ex.Message}");
                return new List<NominatimGeolocation>();
            }
        }
    }

    class NominatimGeolocation
    {
        [JsonPropertyName("place_id")]
        public int PlaceId { get; set; }

        [JsonPropertyName("licence")]
        public string Licence { get; set; }

        [JsonPropertyName("osm_type")]
        public string OsmType { get; set; }

        [JsonPropertyName("osm_id")]
        public string OsmId { get; set; }

        [JsonPropertyName("lat")]
        public string Lat { get; set; }

        [JsonPropertyName("lon")]
        public string Lon { get; set; }

        [JsonPropertyName("class")]
        public string LocationClass { get; set; } // Mapped from "class"

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("place_rank")]
        public int PlaceRank { get; set; }

        [JsonPropertyName("importance")]
        public double Importance { get; set; }

        [JsonPropertyName("addresstype")]
        public string Addresstype { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("boundingbox")]
        public string[] BoundingBox { get; set; }
    }
}
