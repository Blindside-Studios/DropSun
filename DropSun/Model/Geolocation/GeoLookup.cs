using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;
using Windows.Storage;
using Windows.ApplicationModel.Resources;
using System.ComponentModel.Design;

namespace DropSun.Model.Geolocation
{
    class GeoLookup
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private static readonly string _databasePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Model/Geolocation/locations.sqlite3");

        public static List<InternalGeolocation> SearchLocations(string query)
        {
            var results = new List<InternalGeolocation>();

            // Construct the connection string
            string connectionString = $"Data Source={_databasePath}";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Define your priority countries
                var priorityCountries = new List<string> { "DE", "GB", "FR", "US" };

                // Generate SQL CASE statement dynamically based on priorityCountries
                string priorityCase = string.Join(" ",
                    priorityCountries.Select((country, index) => $"WHEN country_code = '{country}' THEN {index + 1}")
                );

                // SQL query to filter cities by name and sort by priority countries
                string sqlQuery = $@"
        SELECT id, name, state_code, country_code, latitude, longitude 
        FROM cities
        WHERE name LIKE @query
        ORDER BY 
            CASE {priorityCase} ELSE {priorityCountries.Count + 1} END, -- Sort by priority countries
            name ASC; -- Then sort alphabetically";

                using (var command = new SqliteCommand(sqlQuery, connection))
                {
                    // Add parameter to prevent SQL injection
                    command.Parameters.AddWithValue("@query", $"%{query}%");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var city = new InternalGeolocation
                            {
                                id = reader.GetInt32(0),
                                name = reader.GetString(1),
                                state_code = reader.GetString(2),
                                country_code = GetCountryName(reader.GetString(3)),
                                latitude = reader.GetDouble(4),
                                longitude = reader.GetDouble(5)
                            };
                            results.Add(city);
                        }
                    }
                }

                connection.Close();
            }


            return results;
        }

        public static async Task<List<NominatimGeolocation>> SearchLocationsAsync(string query)
        {
            string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&limit=5";
            try
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"DropSun/{version} (Blindside-Studios; WeatherApp/LocationQuery)");
                Debug.WriteLine(_httpClient.DefaultRequestHeaders.UserAgent);
                string jsonConfirmation = await _httpClient.GetStringAsync("https://blindside-studios.github.io/DropSun/config.json");
                var confirmation = JsonSerializer.Deserialize<ConfigCall>(jsonConfirmation);
                if (confirmation.AreAPICallsAllowed == true)
                {
                    Debug.WriteLine("received confirmation");
                    string jsonResponse = await _httpClient.GetStringAsync(url);
                    var results = JsonSerializer.Deserialize<List<NominatimGeolocation>>(jsonResponse);
                    return results ?? new List<NominatimGeolocation>();
                }
                else
                {
                    Debug.WriteLine("denied");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching data: {ex.Message}");
                return new List<NominatimGeolocation>();
            }
        }

        public static string GetCountryName(string countryCode)
        {
            // TODO: Fix loading resources! This loads empty strings!
            ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse("Countries");
            string countryName = _resourceLoader.GetString(countryCode);
            if (!string.IsNullOrEmpty(countryName)) return countryName;
            else return countryCode;
        }
    }

    public class InternalGeolocation
    {
        public int id { get; set; }
        public string name { get; set; }
        public string state_code { get; set; }
        public string country_code { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
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
        public int OsmId { get; set; }

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

    class ConfigCall
    {
        [JsonPropertyName("allow_calls_to_nominatim")]
        public bool AreAPICallsAllowed { get; set; }
    }
}
