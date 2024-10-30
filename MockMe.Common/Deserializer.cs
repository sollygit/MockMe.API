using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace MockMe.Common
{
    public static class Deserializer
    {
        public static T FromJson<T>(string json) where T : class
        {
            var item = JsonConvert.DeserializeObject<T>(json);
            var jsonObj = item.ToJson();

            // Ensure JSON is valid and output to debug console
            Debug.WriteLine(JToken.Parse(jsonObj).ToString(Formatting.Indented));

            return item;
        }

        public static IEnumerable<T> FromJsonList<T>(string json) where T : class
        {
            var items = JsonConvert.DeserializeObject<IEnumerable<T>>(json);
            var jsonObj = items.ToJson();

            // Ensure JSON is valid and output to debug console
            Debug.WriteLine(JToken.Parse(jsonObj).ToString(Formatting.Indented));

            return items;
        }

        public static Dictionary<int, T> FromJsonDictionary<T>(string json) where T : class
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };
            var items = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, T>>(json, jsonOptions);

            return items;
        }
    }
}
